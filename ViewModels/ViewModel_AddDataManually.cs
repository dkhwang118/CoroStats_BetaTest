using CoroStats_BetaTest.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CoroStats_BetaTest.ViewModels
{
    public class ViewModel_AddDataManually : ViewModelBase
    {
        #region Fields

        private RelayCommand _command_addCountryDataToDB;
        private string _countryName;
        private string _WHOcountryCode;
        private string _WHOregion;
        private string _totalCases;
        private string _totalDeaths;

        private SqlConnectionService _connService;
        private DatabaseModificationService _modService;
        private DatabaseQueryService _qService;
        private DatabaseService _db;
        

        #endregion // Fields

        #region Constructor

        public ViewModel_AddDataManually()
        {
            _db = new DatabaseService();
            _connService = new SqlConnectionService();
            _qService = new DatabaseQueryService(_connService);
            _modService = new DatabaseModificationService(_connService);
            this.DisplayName = "Add Data Manually To Database";
        }

        #endregion // Constructor

        #region Presentation Properties

        public ICommand Command_AddCountryDataToDB
        {
            get
            {
                if (_command_addCountryDataToDB == null)
                {
                    _command_addCountryDataToDB = new RelayCommand(
                        param => this.AddCountryDataToDB(),
                        param => this.InformationFieldsFilled()
                        ); ;
                }
                return _command_addCountryDataToDB;
            }
        }
        
        public string CountryName
        {
            get
            {
                if (_countryName == null) _countryName = "";
                return _countryName;
            }
            set
            {
                SetProperty<string>(ref _countryName, value);
            }
        }

        public string WHO_CountryCode
        {
            get
            {
                if (_WHOcountryCode == null) _WHOcountryCode = "";
                return _WHOcountryCode;
            }
            set
            {
                SetProperty<string>(ref _WHOcountryCode, value);
            }
        }

        public string WHO_Region
        {
            get
            {
                if (_WHOregion == null) _WHOregion = "";
                return _WHOregion;
            }
            set
            {
                SetProperty<string>(ref _WHOregion, value);
            }
        }

        public string TotalCoronavirusCases
        {
            get
            {
                if (_totalCases == null) _totalCases = "";
                return _totalCases;
            }
            set
            {
                SetProperty<string>(ref _totalCases, value);
            }
        }

        public string TotalCoronavirusDeaths
        {
            get
            {
                if (_totalDeaths == null) _totalDeaths = "";
                return _totalDeaths;
            }
            set
            {
                SetProperty<string>(ref _totalDeaths, value);
            }
        }

        /// <summary>
        /// Method to check if properties are correctly filled before sending data to database
        /// </summary>
        /// <returns></returns>
        private bool InformationFieldsFilled()
        {
            if (CheckField(CountryName, "Country Name")
                && CheckField(WHO_CountryCode, "WHO Country Code")
                && CheckField(WHO_Region, "WHO Region")
                && CheckField(TotalCoronavirusCases, "Total Coronavirus Cases")
                && CheckField(TotalCoronavirusDeaths, "Total Coronavirus Deaths"))
            {
                return true;
            }
            else return false;
        }

        #endregion // Presentaiton Properties

        #region Helper Methods

        private void AddCountryDataToDB()
        {
            // Check fields
            if (!(CheckField(CountryName, "Country Name")
                && CheckField(WHO_CountryCode, "WHO Country Code")
                && CheckField(WHO_Region, "WHO Region")
                && CheckField(TotalCoronavirusCases, "Total Coronavirus Cases")
                && CheckField(TotalCoronavirusDeaths, "Total Coronavirus Deaths")))
            {
                return;
            }

            // Add data to DB
            _db.AddCountryDataToDB(_countryName, _WHOcountryCode, _WHOregion, _totalCases, _totalDeaths);
        }

        /// <summary>
        /// Method to check if a field is filled with text. Displays messagebox if field is not filled
        /// </summary>
        /// <param name="field">string to be checked</param>
        /// <param name="FieldDisplayName">Field name to be shown in messagebox</param>
        /// <returns>field.Length > 0 => true, else false</returns>
        private bool CheckField(string field, string FieldDisplayName)
        {
            if (field.Length > 0) return true;
            else
            {
                return false;
            }
        }

        #endregion // Helper Methods


    }
}