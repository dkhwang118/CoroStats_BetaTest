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
            using (TextFieldParser parser = new TextFieldParser(@_spreadsheetFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                string[] fields;
                Int16 month = 0;
                Int16 cellMonth = 0;
                Int16 day = 0;
                Int16 cellDay = 0;
                Int16 year = 0;
                Int16 cellYear = 0;
                string[] checkingDate = { "0", "0", "0" };
                bool date_YearMonthDay = false;
                bool date_MonthDayYear = false;
                char delim = ' ';


                // read first row to negate column titles
                parser.ReadLine();


                // Determine delimiter
                fields = parser.ReadFields();
                if (fields[0].Contains("/"))
                {
                    delim = '/';
                    checkingDate = fields[0].Split("/");
                }
                else if (fields[0].Contains("-"))
                {
                    delim = '-';
                    checkingDate = fields[0].Split("-");
                }

                // Determine date structure
                if (checkingDate[0].Length == 2)
                {
                    date_MonthDayYear = true;
                    month = Int16.Parse(checkingDate[0]);
                    day = Int16.Parse(checkingDate[1]);
                    day = Int16.Parse(checkingDate[2]);
                }
                else if (checkingDate[0].Length == 4)
                {
                    date_YearMonthDay = true;
                    year = Int16.Parse(checkingDate[0]);
                    month = Int16.Parse(checkingDate[1]);
                    day = Int16.Parse(checkingDate[2]);
                }

                // parse through rest of date column
                while (!parser.EndOfData)
                {
                    //Processing row
                    fields = parser.ReadFields();
                    checkingDate = fields[0].Split(delim);

                    if (date_MonthDayYear)
                    {
                        cellMonth = Int16.Parse(checkingDate[0]);
                        cellDay = Int16.Parse(checkingDate[1]);
                        cellYear = Int16.Parse(checkingDate[2]);
                    }
                    else if (date_YearMonthDay)
                    {
                        cellYear = Int16.Parse(checkingDate[0]);
                        cellMonth = Int16.Parse(checkingDate[1]);
                        cellDay = Int16.Parse(checkingDate[2]);
                    }

                    // if date is later than currently held last date, change
                    if (cellYear > year)
                    {
                        year = cellYear;
                        month = cellMonth;
                        day = cellDay;
                        continue;
                    }
                    else if (cellMonth > month)
                    {
                        month = cellMonth;
                        day = cellDay;
                        continue;
                    }
                    else if (cellMonth == month)
                    {
                        if (cellDay > day)
                        {
                            day = cellDay;
                        }
                    }
                }
                
                // output latest date to UI TextBox
                LatestDataEntryDate = String.Format("{0}/{1}/{2}", month, day, year);
            }
        }


        /// <summary>
        /// Gets the lastest known date of input data from the excel spreadsheet; Excel sheet reading format
        /// </summary>
        private void GetLatestDateOnFile_Excel()
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
            Int16 month = 0;
            Int16 cellMonth = 0;
            Int16 day = 0;
            Int16 cellDay = 0;
            Int16 year = 0;
            Int16 cellYear = 0;

            // read through rows and find the last known date
            for (int i = 3; i <= rowCount; i++)
            {
                cell = (Excel.Range)xlRange.Cells[i, 1];
                string[] dateSplit1 = cell.Value.ToString().Split(" ");
                string[] dateSplit2 = dateSplit1[0].Split("/");
                cellMonth = Int16.Parse(dateSplit2[0]);
                cellDay = Int16.Parse(dateSplit2[1]);
                cellYear = Int16.Parse(dateSplit2[2]);

                // if date is later than currently held last date, change
                if (cellYear > year)
                {
                    year = cellYear;
                    month = cellMonth;
                    day = cellDay;
                    continue;
                }
                else if (cellMonth > month)
                {
                    month = cellMonth;
                    day = cellDay;
                    continue;
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

        /// <summary>
        /// Gets the total number of cases from the CSV spreadsheet
        /// </summary>
        private void GetTotalCasesFromFile_CSV()
        {
            using (TextFieldParser parser = new TextFieldParser(@_spreadsheetFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                string[] fields;
                int countryCases_previous = 0;
                //int countryCases_current = 0;
                //int currentCountryCases = 0;
                int totalCumulativeCases = 0;
                string country_previous = "";
                string country_current = "";

                // read first row to negate column titles
                parser.ReadLine();

                // read-in first entry
                fields = parser.ReadFields();
                country_previous = fields[2];

                // parse through rest of date column
                while (!parser.EndOfData)
                {
                    //Processing row

                    // find highest number of cumulative cases reported for country
                    fields = parser.ReadFields();

                    // if previous data entry country is the same as the current
                    if (country_previous == fields[2])
                    {
                        country_previous = fields[2];
                        countryCases_previous = Int32.Parse(fields[5]);
                    }
                    else // hit end of country entries on the last pass
                    {
                        totalCumulativeCases += countryCases_previous;
                        countryCases_previous = Int32.Parse(fields[5]);
                        TotalCumulativeCases = String.Format("{0}", totalCumulativeCases);
                    }
                    country_previous = fields[2];
                }
            }
        }

        #endregion // Public Methods

    }
}
