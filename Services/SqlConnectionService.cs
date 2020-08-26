///
///     SqlConnectionService.cs
///     Author: David K. Hwang
/// 
///     Class that houses all methods necessary to instantiate and handle the connection to the SQL Database
/// 
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

namespace CoroStats_BetaTest
{
    /// <summary>
    /// Referenced from https://stackoverflow.com/a/32994049
    /// </summary>

    public class SqlConnectionService
    {
        #region Fields

        private SqlConnection _conn;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Sql connection string
        /// </summary>
        private string _connectionString { get; set; }

        private string _connectionMessage { get; set; }

        private string _databaseFilePath { get; set; }

        private string _workingDirectory { get; set; }
        private string _projectDirectory { get; set; }
        private string _baseDirectory { get; set; }

        private string _storedProcedureDirectory { get; set; }

        private bool _connIsOpen { get; set; }

        private bool _connectionLoaded { get; set; }

        private bool _createDatabaseConnectionLoaded { get; set; }

        public SqlConnection Conn { get => _conn; }

        public string DatabaseFilePath { get => _databaseFilePath; }

        public string StoredProcedureDirectory { get => _storedProcedureDirectory; }

        public string ProjectDirectory { get => _projectDirectory; }

        #endregion // Properties

        #region Constructor

        public SqlConnectionService()
        {
            _connIsOpen = false;
            _createDatabaseConnectionLoaded = false;
            _workingDirectory = Environment.CurrentDirectory;
            _projectDirectory = Directory.GetParent(_workingDirectory).Parent.FullName;
            _databaseFilePath = _projectDirectory + "\\CoronaStatsDB.mdf";
            _baseDirectory = Directory.GetParent(_projectDirectory).FullName;
            _storedProcedureDirectory = _baseDirectory + "\\DatabaseScripts";
            _connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + _projectDirectory + "\\CoronaStatsDB.mdf;Integrated Security=True";
            
        }

        #endregion // Constructor

        #region Public Methods
        
        public void InitializeDatabaseConnection()
        {
            _conn = new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Opens the SqlConnection to the Database
        /// </summary>
        public void OpenConnection()
        {
            if (_createDatabaseConnectionLoaded)
            {
                _conn.Dispose();
                _conn = new SqlConnection(_connectionString);
            }
            
            try
            {
                _conn.Open();
                _connIsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Corona Statistics Database Helper");
            }
        }

        /// <summary>
        /// Opens the SqlConnection to the local MS SQL database host for creation of a new database
        /// </summary>
        public void OpenCreateDatabaseConnection()
        {        
            if (!_createDatabaseConnectionLoaded)
            {
                // If there is a connection present, dispose
                if (_conn != null) _conn.Dispose();

                // Create new SqlConnection with specific connection string
                string _createDatabaseConnString = String.Format("Integrated Security=True; Data Source=(LocalDB)\\MSSQLLocalDB; database=master");
                _conn = new SqlConnection(_createDatabaseConnString);
            }

            try
            {
                _conn.Open();
                _connIsOpen = true;
                _createDatabaseConnectionLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Corona Statistics Database Helper");
            }
        }

        /// <summary>
        /// Closes the database connection
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                _conn.Close();
                _connIsOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public string[] GetDates_SingleCountry()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string[]> GetDates_AllCountries()
        {
            throw new NotImplementedException();
        }

        #endregion // Public Methods

        #region Helper Methods

        public void ConnectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            _connectionMessage = e.Message;
        }

        #endregion
    }
}
