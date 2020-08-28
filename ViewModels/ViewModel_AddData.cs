///
///     ViewModel_AddData.cs
///     author: David K. Hwang    
/// 
/// 
///     Contains Data/Variables to be shown on the Add Data page
/// 
///     Contains methods/logic to only have one data entry open at a time
///



using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CoroStats_BetaTest.Pages;
using CoroStats_BetaTest.Services;

namespace CoroStats_BetaTest.ViewModels
{
    class ViewModel_AddData : ViewModelBase
    {
        #region Fields

        private RelayCommand _command_AddDataManually;
        private RelayCommand _command_AddDataFromSpreadsheet;
        private Window _window_currentDataEntryWindow;
        private bool _canOpenNewView;
        private DatabaseService _db;

        #endregion // Fields

        #region Constructor

        public ViewModel_AddData(DatabaseService db)
        {
            _db = db;
            this.DisplayName = "Add Data";
            _canOpenNewView = true;
        }

        #endregion // Constructor

        #region Presentation Properties

        /// <summary>
        /// Command to open View_AddDataManually when the button is clicked
        /// </summary>
        public ICommand Command_AddDataManually
        {
            get
            {
                if (_command_AddDataManually == null)
                {
                    _command_AddDataManually = new RelayCommand(
                        param => this.OpenManualDataEntryView(),
                        param => this._canOpenNewView
                        );
                    
                }
                return _command_AddDataManually;
            }
        }

        /// <summary>
        /// Command to open View_AddDataFromSpreadsheet when the AddDataFromSpreadsheet button is clicked
        /// </summary>
        public ICommand Command_AddDataFromSpreadsheet
        {
            get
            {
                if (_command_AddDataFromSpreadsheet == null)
                {
                    _command_AddDataFromSpreadsheet = new RelayCommand(
                        param => this.OpenSpreadsheetDataEntryView(),
                        param => this._canOpenNewView
                        );

                }
                return _command_AddDataFromSpreadsheet;
            }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Method that opens the View_AddDataManually window
        /// </summary>
        public void OpenManualDataEntryView()
        {
            _window_currentDataEntryWindow = new View_AddDataManually(OnWindowClose);
            _canOpenNewView = false;
            _window_currentDataEntryWindow.Show();
        }

        /// <summary>
        /// Method to open the View_AddDataFromSpreadsheet window
        /// </summary>
        public void OpenSpreadsheetDataEntryView()
        {
            _window_currentDataEntryWindow = new View_AddDataFromSpreadsheet(OnWindowClose);
            _canOpenNewView = false;
            _window_currentDataEntryWindow.Show();
        }

        /// <summary>
        /// Method to be passed to newly opened windows, and called when the window closes
        /// </summary>
        public void OnWindowClose()
        {
            _canOpenNewView = true;
        }

        #endregion // Public Methods
    }
}
