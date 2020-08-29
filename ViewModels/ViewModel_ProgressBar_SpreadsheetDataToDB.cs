using CoroStats_BetaTest.Services;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace CoroStats_BetaTest.ViewModels
{
    public class ViewModel_ProgressBar_SpreadsheetDataToDB : ViewModelBase
    {
        #region Fields

        int _minValue;
        int _maxValue;
        public int _currentValue;
        DatabaseService _db;

        #endregion // Fields

        #region Constructors

        public ViewModel_ProgressBar_SpreadsheetDataToDB()
        {

        }

        public ViewModel_ProgressBar_SpreadsheetDataToDB(int minValue, int maxValue, DatabaseService db)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _db = db;
        }

        #endregion // Constructors

        #region Presentation Properties

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
            get => _db.TotalEntriesChecked;
        }

       

        #endregion

        #region Public Methods


        #endregion // Public Methods


        #region Update Methods

        private void ProgressBarUpdateThread_Start()
        {
            Thread beginProgressBarUpdateThread = new Thread(new ThreadStart(UpdateThreadStartingPoint));
            beginProgressBarUpdateThread.SetApartmentState(ApartmentState.STA);
            beginProgressBarUpdateThread.IsBackground = true;
            beginProgressBarUpdateThread.Start();
        }

        private void UpdateThreadStartingPoint()
        {
            // Do work
            UpdateCurrentValue();
            System.Windows.Threading.Dispatcher.Run();
        }

        private void UpdateCurrentValue()
        {
            _currentValue = _db.TotalEntriesChecked;
        }

        #endregion // Update Methods



    }
}

