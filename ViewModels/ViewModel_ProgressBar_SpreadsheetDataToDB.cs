using CoroStats_BetaTest.Services;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace CoroStats_BetaTest.ViewModels { 

    public class ViewModel_ProgressBar_SpreadsheetDataToDB : ViewModelBase
    {
        #region Fields

        private RelayCommand _cmd_closeWindow;
        private bool _canCloseWindow;

        string _loadingLabel;
        string _buttonLabel;
        int _minValue;
        int _maxValue;
        public int _currentValue;
        DatabaseService _db;
        private Action _windowClose;

        #endregion // Fields

        #region Constructors

        public ViewModel_ProgressBar_SpreadsheetDataToDB()
        {

        }

        public ViewModel_ProgressBar_SpreadsheetDataToDB( int maxValue, DatabaseService db, Action closeWindow)
        {
            _minValue = 0;
            _maxValue = maxValue;
            _db = db;
            _windowClose = closeWindow;
            ThreadPool.QueueUserWorkItem(UpdateCurrentValues);
        }

        #endregion // Constructors

        #region Presentation Properties

        public string LoadingLabel
        {
            get
            {
                if (_loadingLabel == null) _loadingLabel = "Loading Database Entries ...";
                return _loadingLabel;
            }
            set => SetProperty<string>(ref _loadingLabel, value);
        }

        public string ButtonLabel
        {
            get
            {
                if (_buttonLabel == null) _buttonLabel = "Loading ...";
                return _buttonLabel;
            }
            set => SetProperty<string>(ref _buttonLabel, value);
        }

        public int MinValue
        {
            get => _minValue;
        }

        public int MaxValue
        {
            get => _maxValue;
        }

        public int CurrentValue
        {
            get => _currentValue;
            set => SetProperty<int>(ref _currentValue, value);
        }

        public ICommand Command_CloseWindow
        {
            get
            {
                if (_cmd_closeWindow == null)
                {
                    _cmd_closeWindow = new RelayCommand(
                        param => this.CloseWindow(),
                        param => this._canCloseWindow);
                }
                return _cmd_closeWindow;
            }
        }



        #endregion

        #region Public Methods



        #endregion // Public Methods

        #region Update Methods

        private void UpdateCurrentValues(Object stateInfo)
        {
            while (_currentValue != _maxValue)
            {
                CurrentValue = _db.TotalEntriesChecked;
            }
            _canCloseWindow = true;

            LoadingLabel = "Loading Complete!" +
                "\nEntries Checked: " + _db.TotalEntriesChecked +
                "\nEntries Added: " + _db.TotalEntriesLoaded;
            ButtonLabel = "OK";
        }

        #endregion // Update Methods

        #region Window Handling Methods

        private void CloseWindow()
        {
            _windowClose();
        }

        #endregion // Window Handling Methods



    }
}

