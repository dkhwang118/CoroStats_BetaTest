///
///     Service_ExcelFileHandling.cs
///     Author: David K. Hwang
///     
///     Class housing all methods needed to handle the excel spreadsheet
/// 
///     Threading credit goes to: https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/threading-model
/// 
///



using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;

namespace CoroStats_BetaTest.Services
{
    class Service_ExcelFileHandling
    {
        #region Fields

        private string _spreadsheetFilePath;

        #endregion // Fields


        public string OpenSpreadsheetFileDialogue()
        {
            string spreadsheetFilePath = "";
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
        public void GetLatestDateOnFile()
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

            // show latest date
            MessageBox.Show("Latest Date is: " + month.ToString() + "/" + day.ToString() + "/" + year.ToString());

            // always dispose
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            wkbk.Close();
            Marshal.ReleaseComObject(wkbk);

            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
        }
    }
}
