///
///     SqlConnectionService.cs
///     Author: David K. Hwang
/// 
///     Class that houses all methods necessary to instantiate the connection to the SQL Database
/// 
///     Houses method to instantiate the database with all tables necessary for application function
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

        private SqlConnection _conn { get; set; }

        /// <summary>
        /// Sql connection string
        /// </summary>
        private string _connectionString { get; set; }

        private string _connectionMessage { get; set; }

        private string _databaseFilePath { get; set; }

        private string _workingDirectory { get; set; }
        private string _projectDirectory { get; set; }
        private string _baseDirectory { get; set; }

        private string _connIsOpen { get; set; }

        #endregion // Fields

        #region Constructor

        public SqlConnectionService()
        {
            _workingDirectory = Environment.CurrentDirectory;
            _projectDirectory = Directory.GetParent(_workingDirectory).Parent.FullName;
            _databaseFilePath = _projectDirectory + "\\CoronaStatsDB.mdf";
            _baseDirectory = Directory.GetParent(_projectDirectory).FullName;
            _connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + _projectDirectory + "\\CoronaStatsDB.mdf;Integrated Security=True";
        }

        #endregion // Constructor

        #region Public Methods

        /// <summary>
        /// Method to create the application database
        /// </summary>
        public void CreateDatabase()
        {
            // Credit for database creation code: https://support.microsoft.com/en-us/help/307283/how-to-create-a-sql-server-database-programmatically-by-using-ado-net
            // Credit for finding the working and current directory: https://stackoverflow.com/a/11882118

            String str;
            //string _connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=master; AttachDbFilename=" + projectDirectory + "\\CoronaSstatsDB2.mdf; Integrated Security=True;";
            string _createDatabaseConnString = String.Format("Integrated Security=True; Data Source=(LocalDB)\\MSSQLLocalDB; database=master");
            _conn = new SqlConnection(_createDatabaseConnString);

            str = "CREATE DATABASE CoronaStatsDB ON PRIMARY " +
                "(NAME = CoronaStatsDB_Data, " +
                "FILENAME = '" + _projectDirectory + "\\CoronaStatsDB.mdf', " +
                "SIZE = 10MB, MAXSIZE = 512MB, FILEGROWTH = 10%) " +
                "LOG ON (NAME = CoronaStatsDB_log, " +
                "FILENAME = '" + _projectDirectory + "\\CoronaStatsDB_log.ldf', " +
                "SIZE = 1MB, " +
                "MAXSIZE = 5MB, " +
                "FILEGROWTH = 10%)";

            SqlCommand myCommand = new SqlCommand(str, _conn);
            try
            {
                _conn.Open();
                myCommand.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Corona Statistics Database Helper");
            }
            _conn.Close();
            _conn.Dispose();

            // Change _connection string to reflect new database connection
            _connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + _projectDirectory + "\\CoronaStatsDB.mdf;Integrated Security=True";
            _conn = new SqlConnection(_connectionString);
        }
        

        /// <summary>
        /// Opens the SqlConnection to the Database
        /// </summary>
        public void OpenConnection()
        {
            // Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\David\source\repos\CoroStats_BetaTest\CoronaStatsDB.mdf;Integrated Secu
            //_connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + _projectDirectory + "\\CoronaSstatsDB2.mdf;Integrated Security=True";
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

        public void InitializeDB()
        {
            // Check if database exists
            if (databaseIsPresent())
            {
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

        /// <summary>
        /// Executes the stored database procedure 'dbo.InitializeDB', attempting to initialize the database with the base tables
        /// </summary>
        /// <returns>bool if database has already been initialized</returns>
        public bool StoredProcedureInitializeDB()
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

        private void createDatabseTables()
        {
            using (var command = new SqlCommand("Procedure_InitializeDB", _conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                _conn.InfoMessage += new SqlInfoMessageEventHandler(connectionInfoMessage);
                _conn.Open();
                command.ExecuteNonQuery();
                _conn.Close();
            }
        }

        private void deleteDatabaseTables()
        {
            // remove all constraints
            string str = "EXEC sp_msforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"";

            SqlCommand myCommand = new SqlCommand(str, _conn);
            try
            {
                _conn.Open();
                myCommand.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Corona Statistics Database Helper");
            }
            _conn.Close();


            using (var command = new SqlCommand("Procedure_DeleteAllTables", _conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                _conn.InfoMessage += new SqlInfoMessageEventHandler(connectionInfoMessage);
                _conn.Open();
                command.ExecuteNonQuery();
                _conn.Close();
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

        #region Helper Methods

        private void connectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            _connectionMessage = e.Message;
        }

        private bool databaseIsPresent()
        {
            if (File.Exists(_databaseFilePath))
            {
                _conn = new SqlConnection(_connectionString);
                return true;
            }
            else return false;
        }

        private void loadStoredProceduresToDatabase()
        {
            // help on creating the stored procedure: https://stackoverflow.com/a/34973237
            // help creating the stored procedure to delete all tables: https://stackoverflow.com/a/43128914


            foreach (string fileName in Directory.GetFiles(_baseDirectory + "\\DatabaseScripts"))
            {
                // read text from stored procedure file
                StringBuilder sbStoredProcedure = new StringBuilder();
                StreamReader stream = File.OpenText(fileName);
                string s = "";

                while ((s = stream.ReadLine()) != null)
                {
                    sbStoredProcedure.Append(s);
                }

                try
                {
                    using (SqlCommand cmd = new SqlCommand(sbStoredProcedure.ToString(), _conn))
                    {
                        _conn.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString());
                }
                _conn.Close();

            }
        }

        public bool StoredProceduresLoaded()
        {
            // help on checking if the stored procedure exists: https://stackoverflow.com/a/13797842

            string query = "select * from sysobjects where type='P'";
            int storedProcedureCount = Directory.GetFiles(_baseDirectory + "\\DatabaseScripts").Length;
            int numOfFiles = 0;

            using (SqlCommand command = new SqlCommand(query, _conn))
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

        private int numberOfTablesInDatabase()
        {
            _conn.Open();
            DataTable tables = _conn.GetSchema("Tables");
            StringBuilder sbTables = new StringBuilder();

            foreach (DataRow row in tables.Rows)
            {
                sbTables.Append(String.Format("{0} - {1} - {2} \n", (string)row[0], (string)row[1], (string)row[2]));
            }

            int rowNum = tables.Rows.Count;

            _conn.Close();

            return rowNum;
        }

        #endregion
    }
}
