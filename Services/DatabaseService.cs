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

        public int AddToDB_WHO_CSV_FileData(string csvFilePath)
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

            int countryId = 0;
            int dateId = 0;

            int totalEntriesAdded = 0;


            // Prep ExcelFileParsingService
            _parser = new ExcelFileParsingService(csvFilePath);
            _parser.SetParserToCSV();

            // Instantiate DB Connection
            _connService.InitializeDatabaseConnection();

            // Get first line to define columns
            string[] cols = _parser.GetFields();

            // Read in and insert rest of data
            while (!_parser.EndOfFile)
            {


                // Get line of data
                (date, countryCode, countryName, 
                    WHO_region, newCases, 
                    cumulativeCases, newDeaths, cumulativeDeaths) = _parser.GetFields_Formatted();

                // Check if country is already in DB
                if ((countryId = _qService.FindCountryInDB_ReturnCountryId(countryName)) == -1)
                {
                    // Add country to DB; no adding of Total Cases or Total Deaths
                    countryId = AddCountryDataToDB_Spreadsheet(countryName, countryCode, WHO_region);
                }

                // Check if CoronavirusDate already in DB
                if ((dateId = _qService.FindDateInDB_ReturnDateId(date)) == -1)
                {
                    // if not, add to db and get date
                    dateId = _modService.AddToDB_Date_ReturnDateId(date);
                }

                // Check if Database already has an entry for this country at this date
                if (_qService.GetNewCases_SingleCountrySingleDate(countryId, dateId) != -1) continue;

                // Add Data to NewCoronavirusCasesByDate
                _modService.AddToDB_NewCoronavirusCasesDate(countryId, dateId, newCases);

                // Add Data to NewCoronavirusDeathsByDate
                _modService.AddToDB_NewCoronavirusDeathsDate(countryId, dateId, newDeaths);

                totalEntriesAdded++;

            }

            return totalEntriesAdded;

        }

        /// <summary>
        /// Method to add user-defined country information to the application database
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="WHO_countryCode"></param>
        /// <param name="WHO_region"></param>
        /// <param name="totalCases"></param>
        /// <param name="totalDeaths"></param>
        public void AddCountryDataToDB_Manual(string countryName, string WHO_countryCode,
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
            if (WHO_regionId == -1) // If region is not in database => add row and return regionId
            {
                WHO_regionId = _modService.AddToDB_WHO_Region_ReturnRegionId(WHO_region);
            }

            // Add country info to DB w/ WHO_RegionId
            _modService.AddToDB_NewCountryInfo_NoPopulation(WHO_countryCode, countryName, WHO_regionId, _totalCases, _totalDeaths);

            // Dispose of DB connection
            _connService.Conn.Dispose();
        }

        #endregion // Public methods

        #region Private Methods

        /// <summary>
        /// Helper method to add Country Data to database from WHO spreadsheet
        /// </summary>
        /// <param name="countryName">Country Name</param>
        /// <param name="WHO_countryCode">WHO Country Code</param>
        /// <param name="WHO_region">WHO Region</param>
        /// <returns>newly added CoutnryId</returns>
        private int AddCountryDataToDB_Spreadsheet(string countryName, string WHO_countryCode,
                                        string WHO_region)
        {
            int WHO_regionId;
            int countryId;

            // Check if region is in database
            WHO_regionId = _qService.GetWHO_Region(WHO_region);
            if (WHO_regionId == -1) // If region is not in database => add row and return regionId
            {
                WHO_regionId = _modService.AddToDB_WHO_Region_ReturnRegionId(WHO_region);
            }

            // Add country info to DB w/ WHO_RegionId; return newly added Country's countryId
            countryId = _modService.AddToDB_NewCountryInfo_NoPopulation_ReturnCountryId(WHO_countryCode, countryName, WHO_regionId, 0, 0);

            return countryId;
        }

        #endregion // Private Methods
    }
}
