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
    class ViewModel_AddData : ViewModelBase
    {
        #region Fields

        RelayCommand _command_AddDataManually;
        RelayCommand _command_AddDataFromSpreadsheet;
        Window _window_currentDataEntryWindow;
        bool _canOpenNewView;

        #endregion // Fields

        #region Constructor

        public ViewModel_AddData()
        {
            this.DisplayName = "Add Data";
            _canOpenNewView = true;
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
                return _command_AddDataFromSpreadsheet;
            }
        }

        #endregion // Presentation Properties

        #region Public Methods

        public void OpenManualDataEntryView()
        {
            _window_currentDataEntryWindow = new View_AddDataManually(OnWindowClose);
            _canOpenNewView = false;
            _window_currentDataEntryWindow.Show();
        }

        public void OpenSpreadsheetDataEntryView()
        {
            _window_currentDataEntryWindow = new View_AddDataFromSpreadsheet(OnWindowClose);
            _canOpenNewView = false;
            _window_currentDataEntryWindow.Show();
        }

        public void OnWindowClose()
        {
            _canOpenNewView = true;
        }

        bool CanOpenNewView
        {
            get { return _canOpenNewView; }
        }

        #endregion // Public Methods
    }
}
