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

        public Dictionary<string, int> GetTotalCasesDeathsRecoveries()
        {
            return _qService.GetTotalCasesDeathsRecoveries();
        }

        public void AddToDB_WHO_CSV_FileData(string csvFilePath)
        {
            // reused variables
            string month = "";
            string day = "";
            string year = "";


            // Prep ExcelFileParsingService
            _parser = new ExcelFileParsingService(csvFilePath);
            _parser.SetParserToCSV();

            // Get first line to define columns
            string[] cols = _parser.GetFields();

            // Read in and insert rest of data
            while (!_parser.EndOfFile)
            {
                // Get line of data
                string[] fields = _parser.GetFields();

                // Separate Date from fields
                
            }

        }



        #endregion // Public methods
    }
}
