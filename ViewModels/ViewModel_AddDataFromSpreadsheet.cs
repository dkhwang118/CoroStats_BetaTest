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

namespace CoroStats_BetaTest.ViewModels
{
    public class ViewModel_AddDataFromSpreadsheet : ViewModelBase
    {
        #region Fields

        RelayCommand _command_openSpreadsheetFile;
        private bool _canOpenFile;
        private string _spreadsheetFilePath;

        #endregion // Fields

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
                        param => this.OpenSpreadsheetFileDialogue(),
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
            set => SetProperty(ref _spreadsheetFilePath, value);     
        }

        #endregion // Presentation Properties

        #region Public Methods

        public void OpenSpreadsheetFileDialogue()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == true) SpreadsheetFilePath = fileDialog.FileName;

            GetLatestDateOnFile();

            // help from https://www.wpf-tutorial.com/dialogs/the-openfiledialog/
        }

        #endregion // Public Methods

        #region Excel File Parsing Methods

        /// <summary>
        /// Gets the lastest known date of input data from the excel spreadsheet
        /// </summary>
        public void GetLatestDateOnFile()
        {
            // create COM objects
            // helping credit to: https://coderwall.com/p/app3ya/read-excel-file-in-c
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook wkbk = xlApp.Workbooks.Open(@_spreadsheetFilePath);
            Excel._Worksheet xlWorksheet = (Excel.Worksheet)wkbk.Worksheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            // get ranges
            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            // test print of first row
            MessageBox.Show(xlRange.Cells[1, 1].ToString());
        }

        #endregion // excel parsing methods

    }
}
