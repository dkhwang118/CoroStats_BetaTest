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
                // Check if stored procedures are present
                if (_qService.StoredProceduresLoaded())
                {
                    int numTables = _qService.GetNumberOfTablesInDatabase();
                    // Check if stored procedures have created the tables
                    if (numTables == 8)
                    {
                        // if here. DB is initialized and tables are present
                    }
                    else if (numTables > 0)
                    {
                        // Creation of tables went wrong => delete all tables and create new ones
                        _modService.DeleteDatabaseTables();
                        _modService.CreateDatabseTables();
                    }
                    else
                    {
                        _modService.CreateDatabseTables();
                    }
                }
                else
                {
                    _modService.LoadStoredProceduresToDatabase();
                    _modService.CreateDatabseTables();
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
        #endregion // Helper Methods
    }
}
