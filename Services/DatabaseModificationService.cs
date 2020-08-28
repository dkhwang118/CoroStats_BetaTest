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

        public void AddToDB_NewCountryInfo_NoPopulation(string countryCode, string countryName,
                                                        int WHO_regionId, int totalCoronaCases, 
                                                        int totalCoronaDeaths)
        {
            _connService.OpenConnection();

            // Parameterized query string
            try
            {
                using (var command = new SqlCommand("Procedure_AddCountryInfo_SingleRow_NoPop", _connService.Conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.AddWithValue("@CountryCode", countryCode);
                    command.Parameters.AddWithValue("@Name", countryName);
                    command.Parameters.AddWithValue("@WHO_RegionId", WHO_regionId);
                    command.Parameters.AddWithValue("@TotalCoronavirusCases", totalCoronaCases);
                    command.Parameters.AddWithValue("@TotalCoronavirusDeaths", totalCoronaDeaths);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

        }

        /// <summary>
        /// Adds Country Information to Database and Returns the newly add Country's countryId
        /// </summary>
        /// <param name="countryCode">WHO country code</param>
        /// <param name="countryName">Country Name</param>
        /// <param name="WHO_regionId">WHO Region ID</param>
        /// <param name="totalCoronaCases">Total Coronavirus Cases for Country</param>
        /// <param name="totalCoronaDeaths">Total Coronavirus Deaths for Country</param>
        /// <returns>CountryID from table</returns>
        public int AddToDB_NewCountryInfo_NoPopulation_ReturnCountryId(string countryCode, string countryName,
                                                        int WHO_regionId, int totalCoronaCases,
                                                        int totalCoronaDeaths)
        {
            // variables
            string sQuery = "INSERT INTO [dbo].CountryInfo (CountryCode, [Name], WHO_RegionId, TotalCoronavirusCases, TotalCoronavirusDeaths) "
                          + "output INSERTED.CountryId "
                          + "VALUES(@CountryCode, @Name, @WHO_RegionId, @TotalCoronavirusCases, @TotalCoronavirusDeaths)";
            int countryId = -1;

            _connService.OpenConnection();

            // Parameterized query string
            try
            {
                SqlCommand cmd = new SqlCommand(sQuery, _connService.Conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@CountryCode", countryCode);
                cmd.Parameters.AddWithValue("@Name", countryName);
                cmd.Parameters.AddWithValue("@WHO_RegionId", WHO_regionId);
                cmd.Parameters.AddWithValue("@TotalCoronavirusCases", totalCoronaCases);
                cmd.Parameters.AddWithValue("@TotalCoronavirusDeaths", totalCoronaDeaths);

                countryId = (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return countryId;

        }

        public int AddToDB_WHO_Region_ReturnRegionId(string WHO_region)
        {
            _connService.OpenConnection();
            int regionId = 0;

            try
            {
                using (var command = new SqlCommand("Procedure_AddWHO_Region_Return_RegionId", _connService.Conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.AddWithValue("@WHO_Region", WHO_region);
                    regionId = (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return regionId;
        }

        public int AddToDB_Date_ReturnDateId(string date)
        {
            // variables
            int dateId = -1;
            string qString = "INSERT INTO [dbo].CoronavirusDate (Date) output INSERTED.DateId VALUES(@DATE)";

            // Open SqlConnection
            _connService.OpenConnection();

            try
            {
                // Build SqlCommand
                SqlCommand cmd = new SqlCommand(qString, _connService.Conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DATE", date);

                // Execute command; return dateId
                dateId = (int)cmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return dateId;
        }

        public void AddToDB_NewCoronavirusCasesDate(int countryId, int dateId, int numNewCases)
        {
            // variables
            string qString = "INSERT INTO [dbo].NewCoronavirusCasesByDate (CountryId, DateId, NewCases) "
                           + "VALUES(@COUNTRYID, @DATEID, @NEWCASES)";

            // open connection
            _connService.OpenConnection();

            try
            {
                // Build SqlCommand
                SqlCommand cmd = new SqlCommand(qString, _connService.Conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@COUNTRYID", countryId);
                cmd.Parameters.AddWithValue("@DATEID", dateId);
                cmd.Parameters.AddWithValue("@NEWCASES", numNewCases);

                // Execute command
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            // close connection
            _connService.CloseConnection();
        }

        public void AddToDB_NewCoronavirusDeathsDate(int countryId, int dateId, int numNewDeaths)
        {
            // variables
            string qString = "INSERT INTO [dbo].NewCoronavirusDeathsByDate (CountryId, DateId, NewDeaths) "
                           + "VALUES(@COUNTRYID, @DATEID, @NEWDEATHS)";

            // open connection
            _connService.OpenConnection();

            try
            {
                // Build SqlCommand
                SqlCommand cmd = new SqlCommand(qString, _connService.Conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@COUNTRYID", countryId);
                cmd.Parameters.AddWithValue("@DATEID", dateId);
                cmd.Parameters.AddWithValue("@NEWDEATHS", numNewDeaths);

                // Execute command
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            // close connection
            _connService.CloseConnection();
        }




        public void AddToDB_CountryInfo_MultipleDates()
        {
            throw new NotImplementedException();
        }


        #endregion // Public Methods

        #region Helper Methods

        #endregion // Helper Methods
    }
}
