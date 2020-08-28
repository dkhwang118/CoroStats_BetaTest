///
///     ExcelFileParsingService.cs
///     Author: David K. Hwang
/// 
///     Methods and Logic for parsing .csv and .xlms files 
///     
/// 
///


using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic.FileIO;

namespace CoroStats_BetaTest.Services
{
    public class ExcelFileParsingService
    {
        #region Fields

        private string _filePath;
        private TextFieldParser _parser;

        #endregion // Fields

        #region Properties

        public bool EndOfFile{ get => _parser.EndOfData; }


        #endregion // Properties

        #region Constructor

        /// <summary>
        /// ExcelFileParsingService Class Constructor
        /// </summary>
        /// <param name="filePath">csv or excel file path</param>
        public ExcelFileParsingService(string filePath)
        {
            _filePath = filePath;
            _parser = new TextFieldParser(_filePath);
        }

        #endregion // Constructor

        #region Destructor

        ~ExcelFileParsingService()
        {
            _parser.Close();
        }

        #endregion // Destructor

        #region Public Methods

        public void SetParserToCSV()
        {
            _parser.TextFieldType = FieldType.Delimited;
            _parser.SetDelimiters(",");
        }

        public string GetLine()
        {
            return _parser.ReadLine();
        }

        public string[] GetFields()
        {
            return _parser.ReadFields();
        }

        public void CloseParser()
        {
            _parser.Close();
        }

        /// <summary>
        /// Gets the next row of values from the currently opened file 
        /// with a pre-formatted date string for database input
        /// and cases converted to ints
        /// </summary>
        public (string, string, string, string, int, int, int, int) GetFields_Formatted()
        {
            // variables
            string[] fields;
            
            // read-in row
            fields = _parser.ReadFields();

            // return values
            return (GetFormattedDate_MonthDayYear(fields[0]),   // Date
                        fields[1],                              // Country Code
                        fields[2],                              // Country Name
                        fields[3],                              // WHO Region
                        Int32.Parse(fields[4]),                 // New Cases
                        Int32.Parse(fields[5]),                 // Cumulative Cases
                        Int32.Parse(fields[6]),                 // New Deaths
                        Int32.Parse(fields[7]));                // Cumulative Deaths
        }

        public Int16[] GetLatestDate_CSV()
        {
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


            using (TextFieldParser parser = new TextFieldParser(@_filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // read first row to negate column titles
                parser.ReadLine();

                // read first row
                fields = parser.ReadFields();

                // determine date structure
                (delim, date_YearMonthDay, date_MonthDayYear, checkingDate) = GetDateStructure(fields[0]);

                if (date_MonthDayYear)
                {
                    month = Int16.Parse(checkingDate[0]);
                    day = Int16.Parse(checkingDate[1]);
                    year = Int16.Parse(checkingDate[2]);
                }
                else if (date_YearMonthDay)
                {
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
            }

            return new Int16[] { month, day, year };
        }

        public int GetTotalCases_CSV()
        {
            string[] fields;
            int countryCases_previous = 0;
            int totalCumulativeCases = 0;
            string country_previous = "";

            using (TextFieldParser parser = new TextFieldParser(@_filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

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

                    // if previous data entry for country name is the same as the current
                    if (country_previous == fields[2])
                    {
                        // Get newest total cumulative cases value
                        countryCases_previous = Int32.Parse(fields[5]);
                    }
                    else // hit end of country entries on the last pass
                    {
                        // Add previous line's cumulative cases to total cumulative cases
                        totalCumulativeCases += countryCases_previous;

                        // Get newest total cumulative cases value for new country
                        countryCases_previous = Int32.Parse(fields[5]);
                        
                        // Get newest country name
                        country_previous = fields[2];
                    }
                }
            }
            return totalCumulativeCases;
        }

        public Int16[] GetLatestDate_Excel()
        {
            // create COM objects
            // helping credit to: https://coderwall.com/p/app3ya/read-excel-file-in-c
            Microsoft.Office.Interop.Excel.Application xlApp = new Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook wkbk = xlApp.Workbooks.Open(@_filePath);
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

            return new Int16[] { month, day, year };
        }

        #endregion // Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the date structure found within the file
        /// </summary>
        /// <param name="date">date string read from file</param>
        /// <returns>(delimiter char, bool if date is YearMonthDay format, bool if date is MonthDayYear format, a string[] of the date itself</returns>
        private (char, bool, bool, string[]) GetDateStructure(string date)
        {
            bool date_YearMonthDay = false;
            bool date_MonthDayYear = false;
            char delim = ' ';
            string[] date_separated = { "0", "0", "0" };

            // Determine delimiter
            if (date.Contains("/"))
            {
                delim = '/';
                date_separated = date.Split("/");
            }
            else if (date.Contains("-"))
            {
                delim = '-';
                date_separated = date.Split("-");
            }

            // Determine date structure - first pass of actual row data
            if (date_separated[0].Length == 2)
            {
                date_MonthDayYear = true;
            }
            else if (date_separated[0].Length == 4)
            {
                date_YearMonthDay = true;
            }

            return (delim, date_YearMonthDay, date_MonthDayYear, date_separated);
        }

        /// <summary>
        /// Given a arbitrary date string, returns a formatted string in the form of Month/Day/Year
        /// </summary>
        /// <param name="date">date string</param>
        /// <returns>formatted date string</returns>
        private string GetFormattedDate_MonthDayYear(string date)
        {
            // variables
            string[] date_separated = { "0", "0", "0" };
            string returnDate = "";

            // Determine delimiter
            if (date.Contains("/"))
            {
                date_separated = date.Split("/");
            }
            else if (date.Contains("-"))
            {
                date_separated = date.Split("-");
            }

            // Determine date structure
            if (date_separated[0].Length == 2) // if first part of date separated is only 2 chars long
            {
                returnDate = String.Format("{0}/{1}/{2}", date_separated[0], date_separated[1], date_separated[2]); // date format is MonthDayYear
            }
            else if (date_separated[0].Length == 4) // if first part of date is 4 chars long
            {
                returnDate = String.Format("{0}/{1}/{2}", date_separated[1], date_separated[2], date_separated[0]); // date format is YearMonthDay
            }

            return returnDate;
        }

        #endregion // Private Methods
    }
}
