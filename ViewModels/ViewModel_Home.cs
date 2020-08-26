///
///     ViewModel_Home.cs
///     author: David K. Hwang    
/// 
/// 
///     Contains Data/Variables to be shown on the CoronaStatsHome page
/// 
///



using CoroStats_BetaTest.Services;
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

        private long _totalCases;

        private int _totalDeaths;

        #endregion // Fields

        #region Properties
        public DateTime DateToday { get { return DateTime.Today; } }
        public DateTime DateNow { get { return DateTime.Now; } }

        public long TotalCases
        {
            get => _totalCases;
            set => SetProperty(ref _totalCases, value);
        }

        public int TotalDeaths
        {
            get => _totalDeaths;
            set => SetProperty(ref _totalDeaths, value);
        }

        #endregion // Properties

        #region Constructor

        public ViewModel_Home(DatabaseQueryService qService)
        {
            // declare Display Name
            base.DisplayName = "Corona Stats - Home";

            // On Init, get total cases from db


        }

        #endregion // Constructor

        #region Helper Methods



        #endregion




    }


}
