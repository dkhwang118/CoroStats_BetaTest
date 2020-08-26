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

        SqlConnectionService _connService;

        DatabaseModificationService _modService;

        #endregion // Fields

        #region Constructor

        public DatabaseIntegrityService(SqlConnectionService connService, DatabaseModificationService modService)
        {
            this._connService = connService;
            this._modService = modService;
        }

        #endregion // Constructor

        public void DatabaseCheckOnStartup()
        {
            // Check if database exists
            if (databaseIsPresent())
            {
                // Open connection
                _connService.OpenConnection();

                // Check if stored procedures are present
                if (StoredProceduresLoaded())
                {
                    int numTables = numberOfTablesInDatabase();
                    // Check if stored procedures have created the tables
                    if (numTables == 8)
                    {
                        // if here. DB is initialized and tables are present

                    }
                    else if (numTables > 0)
                    {
                        // Creation of tables went wrong => delete all tables and create new ones
                        deleteDatabaseTables();
                        createDatabseTables();
                    }
                    else
                    {
                        createDatabseTables();
                    }
                }
                else
                {
                    loadStoredProceduresToDatabase();
                    createDatabseTables();
                }
            }
            else // if database does not exist => inform user that it will be created and initialized
            {
                MessageBox.Show("Database not found. \n\n Database Initialization will begin.", "Corona Statistics Database Helper");

                // Create DB
                CreateDatabase();
                loadStoredProceduresToDatabase();
                createDatabseTables();
            }
        }

        #region Helper Methods

        private bool databaseIsPresent()
        {
            if (File.Exists(_connService.DatabaseFilePath))
            {
                return true;
            }
            else return false;
        }

        private bool storedProceduresLoaded()
        {
            // help on checking if the stored procedure exists: https://stackoverflow.com/a/13797842

            string query = "select * from sysobjects where type='P'";
            int storedProcedureCount = Directory.GetFiles(_connService.StoredProcedureDirectory).Length;
            int numOfFiles = 0;

            using (SqlCommand command = new SqlCommand(query, _connService.Conn))
            {
                _conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        numOfFiles++;
                    }
                }
                _conn.Close();
            }

            if (storedProcedureCount == numOfFiles) return true;
            else return false;
        }

        #endregion // Helper Methods
    }
}
