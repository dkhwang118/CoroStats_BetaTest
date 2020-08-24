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

        public Int16[] GetLatestDate_CSV(string _csvFilePath)
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


            using (TextFieldParser parser = new TextFieldParser(@_csvFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

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

                // Determine date structure - first pass of actual row data
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
            }

            return new Int16[] { month, day, year };
        }

        public int GetTotalCases_CSV(string _csvFilePath)
        {
            string[] fields;
            int countryCases_previous = 0;
            //int countryCases_current = 0;
            //int currentCountryCases = 0;
            int totalCumulativeCases = 0;
            string country_previous = "";

            using (TextFieldParser parser = new TextFieldParser(@_csvFilePath))
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

                    }
                    country_previous = fields[2];
                }
            }
            return totalCumulativeCases;
        }

        public Int16[] GetLatestDate_Excel(string _excelFilePath)
        {
            // create COM objects
            // helping credit to: https://coderwall.com/p/app3ya/read-excel-file-in-c
            Microsoft.Office.Interop.Excel.Application xlApp = new Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook wkbk = xlApp.Workbooks.Open(@_excelFilePath);
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
    }
}
