///
///     ViewModel_AddDataFromSpreadsheet.cs
///     author: David K. Hwang    
/// 
/// 
///     Contains Data/Variables to be shown on the Add Data From Spreadhseet page
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

        RelayCommand _command_openSpreadsheetFile;
        private bool _canOpenFile;
        private string _spreadsheetFilePath;
        private string _latestDataEntryDate;
        private string _totalCumulativeCases;
        private string _totalDeaths;

        #endregion // Fields

        #region Dispatcher Delegates

        private delegate void NoArgDelegate();
        private delegate void LoadLatestDateDelegate(string filePath);

        #endregion // Dispatcher Delegates

        #region Constructor

        public ViewModel_AddDataFromSpreadsheet()
        {
            this.DisplayName = "Add Data From WHO Spreadsheet";
            _canOpenFile = true;
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
                        param => this.OpenSpreadsheetFileAndLoadDate(),
                        param => this._canOpenFile
                        );
                }
                return _command_openSpreadsheetFile;
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
                _spreadsheetFilePath = value;
                base.OnPropertyChanged("SpreadsheetFilePath");

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
                _latestDataEntryDate = value;
                base.OnPropertyChanged("LatestDataEntryDate");
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
                _totalCumulativeCases = value;
                base.OnPropertyChanged("TotalCumulativeCases");
            }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Summons the OpenFileDialog to open the excel spreadsheet and loads the latest data entry date
        /// </summary>
        public void OpenSpreadsheetFileAndLoadDate()
        {
            _spreadsheetFilePath = OpenSpreadsheetFileDialogue();
            LatestDataEntryDate = "Loading Latest Data Entry Date...";
            GetLatestDateHandler();
        }

        public string OpenSpreadsheetFileDialogue()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == true) _spreadsheetFilePath = fileDialog.FileName;

            return _spreadsheetFilePath;

            // help from https://www.wpf-tutorial.com/dialogs/the-openfiledialog/
        }

        public void GetLatestDateHandler()
        {
            Thread beginLoadingDateThread = new Thread(new ThreadStart(GetLatestDateThreadStartingPoint));
            beginLoadingDateThread.SetApartmentState(ApartmentState.STA);
            beginLoadingDateThread.IsBackground = true;
            beginLoadingDateThread.Start();
        }

        private void GetLatestDateThreadStartingPoint()
        {
            GetLatestDateOnFile_CSV();
            GetTotalCasesFromFile_CSV();
            System.Windows.Threading.Dispatcher.Run();
        }

        /// <summary>
        /// Gets the latest known date of input data from the WHO csv file; CSV file reading format
        /// Credit for original structure goes to: https://stackoverflow.com/a/3508572
        /// </summary>
        private void GetLatestDateOnFile_CSV()
        {
            // instantiate parser service object
            ExcelFileParsingService parser = new ExcelFileParsingService();

            // call method to get latest date 
            Int16[] returnValues = parser.GetLatestDate_CSV(_spreadsheetFilePath);

            // output latest date to UI TextBox
            LatestDataEntryDate = String.Format("{0}/{1}/{2}", returnValues[0], returnValues[1], returnValues[2]);
        }


        /// <summary>
        /// Gets the lastest known date of input data from the excel spreadsheet; Excel sheet reading format
        /// </summary>
        private void GetLatestDateOnFile_Excel()
        {
            // instantiate parser service object
            ExcelFileParsingService parser = new ExcelFileParsingService();

            // call method to get latest date 
            Int16[] returnValues = parser.GetLatestDate_Excel(_spreadsheetFilePath);


            LatestDataEntryDate = String.Format("{0}/{1}/{2}", returnValues[0], returnValues[1], returnValues[2]);
        }

        /// <summary>
        /// Gets the total number of cases from the CSV spreadsheet
        /// </summary>
        private void GetTotalCasesFromFile_CSV()
        {
            // instantiate parser service object
            ExcelFileParsingService parser = new ExcelFileParsingService();

            // call method to get latest date 
            int returnValue = parser.GetTotalCases_CSV(_spreadsheetFilePath);

            TotalCumulativeCases = String.Format("{0}", returnValue);
        }

        #endregion // Public Methods

    }
}
