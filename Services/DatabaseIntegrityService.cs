///
///     DatabaseIntegrityService.cs
///     Author: David K. Hwang
/// 
///     Houses methods to check presence and integrity of .mdf file
///
///

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Windows;
using System.IO;
using Microsoft.Office.Core;

namespace CoroStats_BetaTest.Services
{
    public class DatabaseIntegrityService
    {
        #region Fields

        private SqlConnectionService _connService;

        private DatabaseQueryService _qService;

        private DatabaseModificationService _modService;

        #endregion // Fields

        #region Constructor

        public DatabaseIntegrityService(SqlConnectionService connService, DatabaseQueryService _qService, DatabaseModificationService modService)
        {
            this._connService = connService;
            this._qService = _qService;
            this._modService = modService;
        }

        #endregion // Constructor

        #region Public Methods

        public void DatabaseCheckOnStartup()
        {
            // Check if database exists
            if (databaseIsPresent())
            {
                // Initialize Database Connection
                _connService.InitializeDatabaseConnection();

                // Check if stored procedures are present
                if (_qService.StoredProceduresLoaded())
                {
                    checkDatabaseTables();
                }
                else // Stored Procedures are partially or not loaded
                {
                    _modService.LoadStoredProceduresToDatabase();
                    checkDatabaseTables();
                }
            }
            else // if database does not exist => inform user that it will be created and initialized
            {
                MessageBox.Show("Database not found. \n\nDatabase Initialization will begin.", "Corona Statistics Database Helper");

                // Create DB
                _modService.CreateDatabase();
                _modService.LoadStoredProceduresToDatabase();
                _modService.CreateDatabseTables();
            }
        }

        #endregion // Public Methods

        #region Helper Methods

        private bool databaseIsPresent()
        {
            if (File.Exists(_connService.DatabaseFilePath))
            {
                return true;
            }
            else return false;
        }

        private void checkDatabaseTables()
        {
            switch (_qService.DatabaseTablesLoaded())
            {
                case "Full":
                    // if here. DB is initialized and tables are present
                    break;
                case "Partial":
                    _modService.DeleteDatabaseTables();
                    _modService.CreateDatabseTables();
                    break;
                case "None":
                    _modService.CreateDatabseTables();
                    break;
            }
        }

        #endregion // Helper Methods
    }
}
