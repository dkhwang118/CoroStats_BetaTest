///
///     ViewModel_Home.cs
///     author: David K. Hwang    
/// 
/// 
///     Contains Data/Variables to be shown on the CoronaStatsHome page
/// 
///



using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace CoroStats_BetaTest.ViewModels
{
    public class ViewModel_Home : ViewModelBase
    {
        #region Fields

        public DateTime DateToday { get { return DateTime.Today; } }
        public DateTime DateNow { get { return DateTime.Now; } }

        private long _totalCases;
        public long TotalCases
        {
            get => _totalCases;
            set => SetProperty(ref _totalCases, value);
        }

        private int _totalDeaths;

        public int TotalDeaths
        {
            get => _totalDeaths;
            set => SetProperty(ref _totalDeaths, value);
        }

        private bool _dbIsInitialized;
        public bool DbIsInitialized
        {
            get => _dbIsInitialized;
            set => SetProperty(ref _dbIsInitialized, value);
        }

        #endregion // Fields

        #region Constructor

        public ViewModel_Home()
        {
            // declare Display Name
            base.DisplayName = "Corona Stats - Home";

            // On Init, get total cases from db

        }

        #endregion // Constructor

        #region Helper Methods

        private void getTotalCases()
        {

        }

        #endregion




    }


}
