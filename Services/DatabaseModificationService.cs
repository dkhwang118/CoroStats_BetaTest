///
///     DatabaseModificationService.cs
///     Author: David K. Hwang
/// 
///     Houses Methods to modify the database .mdf file
/// 
/// 
///

using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Windows;
using System.IO;
using Microsoft.Office.Core;

namespace CoroStats_BetaTest.Services
{
    public class DatabaseModificationService
    {
        #region Fields

        SqlConnectionService _connService;

        #endregion // Fields

        #region Constructor

        public DatabaseModificationService(SqlConnectionService connService)
        {
            this._connService = connService;
        }

        #endregion // Constructor

        #region Public Methods

        /// <summary>
        /// Creates the database .mdf file
        /// </summary>
        public void CreateDatabase()
        {
            // Credit for database creation code: https://support.microsoft.com/en-us/help/307283/how-to-create-a-sql-server-database-programmatically-by-using-ado-net
            // Credit for finding the working and current directory: https://stackoverflow.com/a/11882118

            String str;

            // Open SqlConnection with specific database creation string
            _connService.OpenCreateDatabaseConnection();

            str = "CREATE DATABASE CoronaStatsDB ON PRIMARY " +
                "(NAME = CoronaStatsDB_Data, " +
                "FILENAME = '" + _connService.ProjectDirectory + "\\CoronaStatsDB.mdf', " +
                "SIZE = 10MB, MAXSIZE = 512MB, FILEGROWTH = 10%) " +
                "LOG ON (NAME = CoronaStatsDB_log, " +
                "FILENAME = '" + _connService.ProjectDirectory + "\\CoronaStatsDB_log.ldf', " +
                "SIZE = 1MB, " +
                "MAXSIZE = 5MB, " +
                "FILEGROWTH = 10%)";

            SqlCommand myCommand = new SqlCommand(str, _connService.Conn);
            try
            {
                myCommand.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Corona Statistics Database Helper");
            }

            // Close database SqlConnection
            _connService.CloseConnection();
        }

        /// <summary>
        /// Deletes all tables from the database
        /// </summary>
        public void DeleteDatabaseTables()
        {
            // command string to remove all constraints
            string str = "EXEC sp_msforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"";

            // Open connection
            _connService.OpenConnection();

            SqlCommand myCommand = new SqlCommand(str, _connService.Conn);
            try
            {
                // Execute command
                myCommand.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Corona Statistics Database Helper");
            }

            // Delete all tables
            try
            {
                using (var command = new SqlCommand("Procedure_DeleteAllTables", _connService.Conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    _connService.Conn.InfoMessage += new SqlInfoMessageEventHandler(_connService.ConnectionInfoMessage);
                    command.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Corona Statistics Database Helper");
            }

            // Close Connection
            _connService.CloseConnection();
        }

        /// <summary>
        /// Creates all necessary database tables in .mdf file for application
        /// </summary>
        public void CreateDatabseTables()
        {
            // Open Connection
            _connService.OpenConnection();

            try
            {
                using (var command = new SqlCommand("Procedure_InitializeDB", _connService.Conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    _connService.Conn.InfoMessage += new SqlInfoMessageEventHandler(_connService.ConnectionInfoMessage);
                    command.ExecuteNonQuery();
                }

                _connService.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Corona Statistics Database Helper");
            }

            // Close Connection
            _connService.CloseConnection();

        }

        /// <summary>
        /// Loads all stored procedures from files in DatabaseScripts directory
        /// </summary>
        public void LoadStoredProceduresToDatabase()
        {
            // help on creating the stored procedure: https://stackoverflow.com/a/34973237
            // help creating the stored procedure to delete all tables: https://stackoverflow.com/a/43128914

            // Open Connection
            _connService.OpenConnection();

            foreach (string fileName in Directory.GetFiles(_connService.StoredProcedureDirectory))
            {
                // read text from stored procedure file
                StringBuilder sbStoredProcedure = new StringBuilder();
                StreamReader stream = File.OpenText(fileName);
                string s = "";

                // Create string using stored procedure text
                while ((s = stream.ReadLine()) != null)
                {
                    sbStoredProcedure.Append(s);
                }

                // try creation of stored procedure
                try
                {
                    using (SqlCommand cmd = new SqlCommand(sbStoredProcedure.ToString(), _connService.Conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();

                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString());
                }
            }

            // Close Connection
            _connService.CloseConnection();

        }

        public void AddToDB_CountryInfo_SingleDate()
        {
            throw new NotImplementedException();
        }

        public void AddToDB_CountryInfo_MultipleDates()
        {
            throw new NotImplementedException();
        }


        #endregion // Public Methods
    }
}
