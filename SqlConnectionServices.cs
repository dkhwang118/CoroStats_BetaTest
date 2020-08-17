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
        /// <summary>
        /// Sql Connection Object
        /// </summary>
        public SqlConnection Conn { get; set; }
        /// <summary>
        /// Sql connection string
        /// </summary>
        private string connectionString { get; set; }

        private string connectionMessage { get; set; }

        
        private void connectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            connectionMessage = e.Message;
        }

        /// <summary>
        /// Opens the SqlConnection to the Database
        /// </summary>
        public void OpenConnection()
        {
            // Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\David\source\repos\CoroStats_BetaTest\CoronaStatsDB.mdf;Integrated Secu
            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\David\\source\\repos\\CoroStats_BetaTest\\CoronaStatsDB.mdf;Integrated Security=True";
            Conn = new SqlConnection(connectionString);
            try
            {
                Conn.Open();
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
            Conn.Close();
        }

        #region Public Methods

        /// <summary>
        /// Executes the stored database procedure 'dbo.InitializeDB', attempting to initialize the database with the base tables
        /// </summary>
        /// <returns>bool if database has already been initialized</returns>
        public bool InitializeDB()
        {
            using (var command = new SqlCommand("InitializeDB", Conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                Conn.InfoMessage += new SqlInfoMessageEventHandler(connectionInfoMessage);
                command.ExecuteNonQuery();
                if (connectionMessage == "DB is not initialized")
                {
                    return false;
                }
                else { return true; }
            }
        }

        #endregion // Public Methods
    }
}
