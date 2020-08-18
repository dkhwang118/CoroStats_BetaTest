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

namespace CoroStats_BetaTest.ViewModels
{
    public class ViewModel_AddDataFromSpreadsheet : ViewModelBase
    {
        #region Fields

        RelayCommand _command_openSpreadsheetFile;
        private bool _canOpenFile;
        private string _spreadsheetFilePath;
        private string _latestDataEntryDate;

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
            GetLatestDateOnFile();
            System.Windows.Threading.Dispatcher.Run();
        }

        /// <summary>
        /// Gets the lastest known date of input data from the excel spreadsheet
        /// </summary>
        private void GetLatestDateOnFile()
        {
            // create COM objects
            // helping credit to: https://coderwall.com/p/app3ya/read-excel-file-in-c
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook wkbk = xlApp.Workbooks.Open(@_spreadsheetFilePath);
            Excel.Worksheet xlWorksheet = (Excel.Worksheet)wkbk.Worksheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            // get ranges
            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            // parameters for calculations
            Excel.Range cell;
            int year = 0;
            int month = 0;
            int day = 0;

            // read through rows and find the last known date
            for (int i = 2; i <= rowCount; i++)
            {
                cell = (Excel.Range)xlRange.Cells[i, 1];
                string[] dateSplit1 = cell.Value.ToString().Split(" ");
                string[] dateSplit2 = dateSplit1[0].Split("/");
                int cellMonth = Int16.Parse(dateSplit2[0]);
                int cellDay = Int16.Parse(dateSplit2[1]);
                int cellYear = Int16.Parse(dateSplit2[2]);

                // if date is later than currently held last date, change
                if (cellYear > year)
                {
                    year = cellYear;
                    month = cellMonth;
                    day = cellDay;
                }
                else if (cellMonth > month)
                {
                    month = cellMonth;
                    day = cellDay;
                }
                else if (cellMonth == month)
                {
                    if (cellDay > day)
                    {
                        day = cellDay;
                    }
                }
            }


            // always dispose
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            wkbk.Close();
            Marshal.ReleaseComObject(wkbk);

            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            LatestDataEntryDate = String.Format("{0}/{1}/{2}", month, day, year);
        }


        #endregion // Public Methods

    }
}
