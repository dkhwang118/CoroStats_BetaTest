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
        int _currentValue;

        #endregion // Fields

        #region Constructors

        public ViewModel_ProgressBar_SpreadsheetDataToDB()
        {

        }

        public ViewModel_ProgressBar_SpreadsheetDataToDB(int minValue, int maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
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
            get => _currentValue;
            set => SetProperty<int>(ref _currentValue, value);
        }

        #endregion

    }
}
