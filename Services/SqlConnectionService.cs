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

        private SqlConnection _conn;                        // SqlConnection object
        private string _connectionString;                   // Connection string to Sql DB
        private string _connectionMessage;                  //
        private string _databaseFilePath;
        private string _workingDirectory;
        private string _projectDirectory;
        private string _baseDirectory;
        private string _storedProcedureDirectory;
        private bool _connIsOpen;
        private bool _createDatabaseConnectionLoaded;

        #endregion // Fields

        #region Properties

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

        #region Destructor 
        ~SqlConnectionService()
        {
            if (_conn != null) _conn.Dispose();
        }

        #endregion // Destructor

        #region Public Methods

        /// <summary>
        /// Initializes the SQL DB connection with the default connection string
        /// </summary>
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
                // If there is a connection object present, dispose
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

        #endregion // Public Methods

        #region Helper Methods

        public void ConnectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            _connectionMessage = e.Message;
        }

        #endregion
    }
}
