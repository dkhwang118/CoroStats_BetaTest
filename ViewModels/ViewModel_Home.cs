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

        private int _totalCases;

        private int _totalDeaths;

        private int _totalRecoveries;

        private DatabaseQueryService _qService;

        #endregion // Fields

        #region Properties
        public DateTime DateToday { get { return DateTime.Today; } }
        public DateTime DateNow { get { return DateTime.Now; } }

        public int TotalCases
        {
            get => _totalCases;
            set => SetProperty(ref _totalCases, value);
        }

        public int TotalDeaths
        {
            get => _totalDeaths;
            set => SetProperty(ref _totalDeaths, value);
        }

        public int TotalRecoveries
        {
            get => _totalRecoveries;
            set => SetProperty(ref _totalRecoveries, value);
        }

        #endregion // Properties

        #region Constructor

        public ViewModel_Home(DatabaseQueryService qService)
        {
            _qService = qService;

            // declare Display Name
            base.DisplayName = "Corona Stats - Home";

            // On Init, get total cases from db
            updateTotalCasesDeathsRecoveries();

        }

        #endregion // Constructor

        #region Helper Methods

        private void updateTotalCasesDeathsRecoveries()
        {
            Dictionary<string, int> values = _qService.GetTotalCasesDeathsRecoveries();
            TotalCases = values["TotalCoronavirusCases"];
            TotalDeaths = values["TotalCoronavirusDeaths"];
            TotalRecoveries = values["TotalCoronavirusRecoveries"];
        }

        #endregion




    }


}
