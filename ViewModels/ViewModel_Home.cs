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
    class ViewModel_Home : ViewModel_SelectedWindow
    {
        public DateTime DateToday { get { return DateTime.Today; } }
        public DateTime DateNow { get { return DateTime.Now; } }

        public ViewModel_Home()
        {
            base.DisplayName = "Corona Stats - Home";

        }

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
