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
using CoroStats_BetaTest.Services;
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
        private Service_ExcelFileHandling _excelFileHandle;

        #endregion // Fields

        #region Dispatcher Delegates

        private delegate void NoArgDelegate();
        private delegate void LoadLatestDateDelegate(string filePath);

        #endregion // Dispatcher Delegates

        #region Constructor

        public ViewModel_AddDataFromSpreadsheet()
        {
            this.DisplayName = "Add Data From WHO Spreadsheet";
            this._excelFileHandle = new Service_ExcelFileHandling();
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
            set => SetProperty(ref _spreadsheetFilePath, value);     
        }

        public string LatestDataEntryDate
        {
            get
            {
                if (_latestDataEntryDate == null) _latestDataEntryDate = "...";
                return _latestDataEntryDate;
            }
            set => SetProperty(ref _latestDataEntryDate, value);
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Summons the OpenFileDialog to open the excel spreadsheet and loads the latest data entry date
        /// </summary>
        public void OpenSpreadsheetFileAndLoadDate()
        {
            _spreadsheetFilePath = _excelFileHandle.OpenSpreadsheetFileDialogue();
            LatestDataEntryDate = "Loading Latest Data Entry Date...";

            _excelFileHandle.GetLatestDateHandler();
        }

        #endregion // Public Methods

    }
}
