using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Windows;

namespace CoroStats_BetaTest
{
    /// <summary>
    /// Referenced from https://stackoverflow.com/a/32994049
    /// </summary>

    public class SqlConnectionServices
    {
        #region fields

        private SqlConnection _conn { get; set; }

        /// <summary>
        /// Sql connection string
        /// </summary>
        private string _connectionString { get; set; }

        private string _connectionMessage { get; set; }

        #endregion // Fields


        #region Helper Functions

        private void connectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            _connectionMessage = e.Message;
        }

        #endregion // Helper Functions

        #region Public Methods

        /// <summary>
        /// Opens the SqlConnection to the Database
        /// </summary>
        public void OpenConnection()
        {
            // Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\David\source\repos\CoroStats_BetaTest\CoronaStatsDB.mdf;Integrated Secu
            _connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\David\\source\\repos\\CoroStats_BetaTest\\CoronaStatsDB.mdf;Integrated Security=True";
            _conn = new SqlConnection(_connectionString);
            try
            {
                _conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Closes the database connection
        /// </summary>
        public void CloseConnection()
        {
            _conn.Close();
        }

        /// <summary>
        /// Executes the stored database procedure 'dbo.InitializeDB', attempting to initialize the database with the base tables
        /// </summary>
        /// <returns>bool if database has already been initialized</returns>
        public bool InitializeDB()
        {
            using (var command = new SqlCommand("InitializeDB", _conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                _conn.InfoMessage += new SqlInfoMessageEventHandler(connectionInfoMessage);
                command.ExecuteNonQuery();
                if (_connectionMessage == "DB is not initialized")
                {
                    return false;
                }
                else { return true; }
            }
        }

        public bool CheckForInitializedDB()
        {
            throw new NotImplementedException();
        }

        public string[] GetDates_SingleCountry()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string[]> GetDates_AllCountries()
        {
            throw new NotImplementedException();
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
