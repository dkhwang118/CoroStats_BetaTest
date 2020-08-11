///
///     ViewModel_AddData.cs
///     author: David K. Hwang    
/// 
/// 
///     Contains Data/Variables to be shown on the Add Data page
/// 
///



using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CoroStats_BetaTest.Pages;

namespace CoroStats_BetaTest.ViewModels
{
    class ViewModel_AddData : ViewModel_SelectedWindow
    {
        #region Fields

        RelayCommand _command_AddDataManually;
        RelayCommand _command_AddDataFromSpreadsheet;
        Window _window_currentDataEntryWindow;

        #endregion // Fields

        #region Constructor

        public ViewModel_AddData()
        {
            this.DisplayName = "Add Data";
        }

        #endregion // Constructor

        #region Presentation Properties

        public ICommand Command_AddDataManually
        {
            get
            {
                if (_command_AddDataManually == null)
                {
                    _command_AddDataManually = new RelayCommand(
                        param => this.OpenManualDataEntryView(),
                        param => this.CanOpenNewView
                        );
                    
                }
                return _command_AddDataManually;
            }
        }

        public ICommand Command_AddDataFromSpreadsheet
        {
            get
            {
                if (_command_AddDataFromSpreadsheet == null)
                {
                    _command_AddDataFromSpreadsheet = new RelayCommand(
                        param => this.OpenSpreadsheetDataEntryView(),
                        param => this.CanOpenNewView
                        );

                }
                return _command_AddDataManually;
            }
        }

        #endregion // Presentation Properties

        #region Public Methods

        public void OpenManualDataEntryView()
        {
            _window_currentDataEntryWindow = new View_AddDataManually();
            _window_currentDataEntryWindow.Show();
        }

        public void OpenSpreadsheetDataEntryView()
        {
            throw new NotImplementedException();
        }

        bool CanOpenNewView
        {
            get { return _window_currentDataEntryWindow == null; }
        }

        #endregion // Public Methods
    }
}
