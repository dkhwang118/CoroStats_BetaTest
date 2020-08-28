///
///     DatabaseService.cs
///     Author: David K. Hwang
/// 
/// 
///     Class to hold methods related to database functionality for the ViewModels
/// 
///     Acts as the Model for the application
///

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CoroStats_BetaTest.Services
{
    public class DatabaseService
    {
        #region Fields

        private SqlConnectionService _connService;
        private DatabaseIntegrityService _integrityService;
        private DatabaseQueryService _qService;
        private DatabaseModificationService _modService;
        private ExcelFileParsingService _parser;


        #endregion // Fields

        #region Constructor

        public DatabaseService()
        {
            _connService = new SqlConnectionService();
            _qService = new DatabaseQueryService(_connService);
            _modService = new DatabaseModificationService(_connService);
            _integrityService = new DatabaseIntegrityService(_connService, _qService, _modService);
        }

        #endregion // Constructor

        #region Public Methods

        public void DatabaseIntegrityCheck()
        {
            _integrityService.DatabaseCheckOnStartup();
        }

        public void InitializeDatabaseConnection()
        {
            _connService.InitializeDatabaseConnection();
        }

        public Dictionary<string, int> GetTotalCasesDeathsRecoveries()
        {
            return _qService.GetTotalCasesDeathsRecoveries();
        }

        public void AddToDB_WHO_CSV_FileData(string csvFilePath)
        {
            // reused variables
            string date = "";
            string countryCode = "";
            string countryName = "";
            string WHO_region = "";
            int newCases = 0;
            int cumulativeCases = 0;
            int newDeaths = 0;
            int cumulativeDeaths = 0;


            // Prep ExcelFileParsingService
            _parser = new ExcelFileParsingService(csvFilePath);
            _parser.SetParserToCSV();

            // Get first line to define columns
            string[] cols = _parser.GetFields();

            // Read in and insert rest of data
            while (!_parser.EndOfFile)
            {
                // Get line of data
                (date, countryCode, countryName, 
                    WHO_region, newCases, 
                    cumulativeCases, newDeaths, cumulativeDeaths) = _parser.GetFields_Formatted();


                
            }

        }

        public void AddCountryDataToDB(string countryName, string WHO_countryCode,
                                        string WHO_region, string totalCases, string totalDeaths)
        {
            // variables
            int WHO_regionId;
            int _totalCases = Int32.Parse(totalCases);
            int _totalDeaths = Int32.Parse(totalDeaths);


            // Instantiate DB connection
            _connService.InitializeDatabaseConnection();

            // Check if Country already exists in database
            if (_qService.IsCountryPresentInDB(countryName))
            {
                MessageBox.Show(String.Format("Country {0} is already present in Database!", countryName), "CoronaStats Database Helper");
                return;
            }

            // Check if region is in database
            WHO_regionId = _qService.GetWHO_Region(WHO_region);
            if (WHO_regionId == 0) // If region is not in database => add row and return regionId
            {
                WHO_regionId = _modService.AddToDB_WHO_Region_ReturnRegionId(WHO_region);
            }

            // Add country info to DB w/ WHO_RegionId
            _modService.AddToDB_NewCountryInfo_NoPopulation(WHO_countryCode, countryName, WHO_regionId, _totalCases, _totalDeaths);

            // Dispose of DB connection
            _connService.Conn.Dispose();
        }



        #endregion // Public methods
    }
}
