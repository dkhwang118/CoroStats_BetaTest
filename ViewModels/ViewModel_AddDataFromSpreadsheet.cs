///
///     ViewModel_AddDataFromSpreadsheet.cs
///     author: David K. Hwang    
/// 
/// 
///     Contains Data/Variables to be shown on the Add Data From Spreadhseet page
///
///     Multithreading help credit: https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/threading-model
/// 
///

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CoroStats_BetaTest.Pages;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Threading;
using Microsoft.VisualBasic.FileIO;
using CoroStats_BetaTest.Services;

namespace CoroStats_BetaTest.ViewModels
{
    public class ViewModel_AddDataFromSpreadsheet : ViewModelBase
    {
        #region Fields

        private RelayCommand _command_openSpreadsheetFile;
        private RelayCommand _command_addSpreadsheetDataToDB;

        private bool _canOpenFile;
        private bool _canGetDataFromFile;
        private string _spreadsheetFilePath;
        private string _latestDataEntryDate;
        private string _totalCumulativeCases;
        private string _totalCumulativeDeaths;
        private ExcelFileParsingService _parser;
        private DatabaseService _db;
        private int _totalDatafileEntries;

        #endregion // Fields

        #region Dispatcher Delegates

        private delegate void NoArgDelegate();
        private delegate void LoadLatestDateDelegate(string filePath);

        #endregion // Dispatcher Delegates

        #region Constructor

        public ViewModel_AddDataFromSpreadsheet()
        {
            _totalDatafileEntries = -1;
            _db = new DatabaseService();
            this.DisplayName = "Add Data From WHO Spreadsheet";
            _canOpenFile = true;
            _canGetDataFromFile = false;
        }

        #endregion // Constructor

        #region Presentation Properties

        public ICommand Command_OpenSpreadsheetFile
        {
            get
            {
                if (_command_openSpreadsheetFile == null)
                {
                    _command_openSpreadsheetFile = new RelayCommand(
                        param => this.GetMetaData_StartWork(),
                        param => this._canOpenFile
                        );
                }
                return _command_openSpreadsheetFile;
            }
        }

        public ICommand Command_AddSpreadsheetDataToDB
        {
            get
            {
                if (_command_addSpreadsheetDataToDB == null)
                {
                    _command_addSpreadsheetDataToDB = new RelayCommand(
                        param => this.AddFileDataToDB_StartWork(),
                        param => this._canGetDataFromFile
                        );
                }
                return _command_addSpreadsheetDataToDB;
            }
        }

        public int TotalDataFileEntries_int
        {
            get
            {
                if (_totalDatafileEntries == -1) return 0;
                else return _totalDatafileEntries;
            }
            set
            {
                SetProperty<int>(ref _totalDatafileEntries, value);
            }
        }

        public string TotalDatafileEntries_string
        {
            get
            {
                if (_totalDatafileEntries == -1) return "...";
                else return _totalDatafileEntries.ToString();
            }
            set
            {
                SetProperty<int>(ref _totalDatafileEntries, Int32.Parse(value));
            }
        }

        public string SpreadsheetFilePath
        {
            get
            {
                if (_spreadsheetFilePath == null) _spreadsheetFilePath = "Please select the WHO Coronavirus Spreadsheet File";
                return _spreadsheetFilePath;
            }
            set
            {
                SetProperty<string>(ref _spreadsheetFilePath, value);
            }
        }
        public string LatestDataEntryDate
        {
            get
            {
                if (_latestDataEntryDate == null) _latestDataEntryDate = "...";
                return _latestDataEntryDate;
            }
            set 
            {
                SetProperty<string>(ref _latestDataEntryDate, value);
            } 
        }

        public string TotalCumulativeCases
        {
            get
            {
                if (_totalCumulativeCases == null) _totalCumulativeCases = "...";
                return _totalCumulativeCases;
            }
            set
            {
                SetProperty<string>(ref _totalCumulativeCases, value);
            }
        }

        public string TotalCumulativeDeaths
        {
            get
            {
                if (_totalCumulativeDeaths == null) _totalCumulativeDeaths = "...";
                return _totalCumulativeDeaths;
            }
            set
            {
                SetProperty<string>(ref _totalCumulativeDeaths, value);
            }
        }

        #endregion // Presentation Properties

        #region Multithreading Methods

        /// <summary>
        /// Summons the OpenFileDialog to open the excel spreadsheet and loads the latest data entry date
        /// </summary>
        private void GetSpreadsheetFileAndMetadata(Object stateInfo)
        {
            _spreadsheetFilePath = OpenSpreadsheetFileDialogue();
            if (_spreadsheetFilePath == "Please select the WHO Coronavirus Spreadsheet File") return;
            else _canGetDataFromFile = true;

            _parser = new ExcelFileParsingService(_spreadsheetFilePath);
            LatestDataEntryDate = "Loading Latest Data Entry Date...";
            TotalCumulativeCases = "Calculating Total Cases To Date...";

            GetLatestDateOnFile_CSV();
            GetTotalCasesOnFile_CSV();
            _totalDatafileEntries = _parser.GetTotalDataEntriesOnFile();
        }

        private void GetMetaData_StartWork()
        {
            ThreadPool.QueueUserWorkItem(GetSpreadsheetFileAndMetadata);
        }

        private void AddDataToDb(Object stateInfo)
        {
            //

            int totalEntriesAdded = _db.AddToDB_WHO_CSV_FileData(_spreadsheetFilePath);
            MessageBox.Show(String.Format("{0} Entries Added to Database!", totalEntriesAdded),
                                "CoronaStats Database Helper Service");
        }

        private void OpenProgressBarWindow(Object stateInfo)
        {
            View_ProgressBar_SpreadsheetDataToDB window = new View_ProgressBar_SpreadsheetDataToDB();
            var viewModel = new ViewModel_ProgressBar_SpreadsheetDataToDB(_totalDatafileEntries, _db, window.Close);
            window.DataContext = viewModel;
            window.ShowDialog();
        }

        private void AddFileDataToDB_StartWork()
        {
            Thread progressBarThread = new Thread(OpenProgressBarWindow);
            progressBarThread.SetApartmentState(ApartmentState.STA);
            progressBarThread.Start();
            ThreadPool.QueueUserWorkItem(AddDataToDb);
        }

        #endregion // Multithreading methods

        #region Public Methods

        public string OpenSpreadsheetFileDialogue()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == true) 
            {
                _spreadsheetFilePath = fileDialog.FileName;
            } 

            return _spreadsheetFilePath;

            // help from https://www.wpf-tutorial.com/dialogs/the-openfiledialog/
        }

        /// <summary>
        /// Gets the latest known date of input data from the WHO csv file; CSV file reading format
        /// Credit for original structure goes to: https://stackoverflow.com/a/3508572
        /// </summary>
        private void GetLatestDateOnFile_CSV()
        {

            // call method to get latest date 
            Int16[] returnValues = _parser.GetLatestDate_CSV();

            // output latest date to UI TextBox
            LatestDataEntryDate = String.Format("{0}/{1}/{2}", returnValues[0], returnValues[1], returnValues[2]);
        }


        /// <summary>
        /// Gets the lastest known date of input data from the excel spreadsheet; Excel sheet reading format
        /// </summary>
        private void GetLatestDateOnFile_Excel()
        {

            // call method to get latest date 
            Int16[] returnValues = _parser.GetLatestDate_Excel();


            LatestDataEntryDate = String.Format("{0}/{1}/{2}", returnValues[0], returnValues[1], returnValues[2]);
        }

        /// <summary>
        /// Gets the total number of cases from the CSV spreadsheet
        /// </summary>
        private void GetTotalCasesOnFile_CSV()
        {
            // call method to get latest date 
            int returnValue = _parser.GetTotalCases_CSV();

            TotalCumulativeCases = String.Format("{0}", returnValue);
        }

        #endregion // Public Methods

    }
}
