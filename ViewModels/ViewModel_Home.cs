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
using System.Text;

namespace CoroStats_BetaTest.ViewModels
{
    class ViewModel_Home : ViewModelBase
    {
        public DateTime DateToday { get { return DateTime.Today; } }
        public DateTime DateNow { get { return DateTime.Now; } }

        private long _totalCases;
        public long TotalCases
        {
            get => _totalCases;
            set => SetProperty(ref _totalCases, value);
        }

        private bool _dbIsInitialized;
        public bool DbIsInitialized
        {
            get => _dbIsInitialized;
            set => SetProperty(ref _dbIsInitialized, value);
        }
    }
}
