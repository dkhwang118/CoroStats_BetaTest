///
///     DatabaseQueryService.cs
///     Author: David K. Hwang
/// 
///     Houses all methods that query the database (no modifications of database => DatabseModificationService.cs)
///

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.IO;

namespace CoroStats_BetaTest.Services
{
    public class DatabaseQueryService
    {
        #region Fields

        private SqlConnectionService _connService;

        #endregion // Fields

        #region Constructor

        public DatabaseQueryService(SqlConnectionService connService)
        {
            _connService = connService;
        }

        #endregion // Constructor

        #region Public Methods

        public bool StoredProceduresLoaded()
        {
            // help on checking if the stored procedure exists: https://stackoverflow.com/a/13797842
            _connService.OpenConnection();


            string query = "select * from sysobjects where type='P'";
            int storedProcedureCount = Directory.GetFiles(_connService.StoredProcedureDirectory).Length;
            int numOfFiles = 0;

            using (SqlCommand command = new SqlCommand(query, _connService.Conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        numOfFiles++;
                    }
                }
            }
            _connService.CloseConnection();

            if (storedProcedureCount == numOfFiles) return true;
            else return false;
        }

        public string DatabaseTablesLoaded()
        {
            int numTables = GetNumberOfTablesInDatabase();
            // Check if stored procedures have created the tables
            if (numTables == 8)
            {
                // if here. DB is initialized and tables are present
                return "Full";
            }
            else if (numTables > 0)
            {
                // Creation of tables went wrong => delete all tables and create new ones
                return "Partial";
            }
            else
            {
                return "None";
            }
        }

        /// <summary>
        /// Retrieves the total number of coronavirus Cases, Deaths, and Recoveries
        /// </summary>
        /// <returns>Dictionary accessed by string value of value name</returns>
        public Dictionary<string, int> GetTotalCasesDeathsRecoveries()
        {
            Dictionary<string, int> values = new Dictionary<string, int>();

            _connService.OpenConnection();

            try
            {
                SqlCommand command = new SqlCommand("Procedure_GetTotalCasesDeathsRecoveries", _connService.Conn);
                command.CommandType = CommandType.StoredProcedure;

                _connService.Conn.InfoMessage += new SqlInfoMessageEventHandler(_connService.ConnectionInfoMessage);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<object> vals = readSingleRow((IDataRecord)reader);
                        values[(String)vals[1]] = (int)vals[2];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Corona Statistics Database Helper");
            }

            // Close Connection
            _connService.CloseConnection();

            return values;
        }

        #endregion // Public Methods

        #region Helper Methods

        private int GetNumberOfTablesInDatabase()
        {
            _connService.OpenConnection();

            DataTable tables = _connService.Conn.GetSchema("Tables");

            _connService.CloseConnection();

            return tables.Rows.Count;
        }

        private List<Object> readSingleRow(IDataRecord data)
        {
            List<Object> values = new List<Object>();
            for (int i = 0; i < data.FieldCount; i++)
            {
                values.Add(data[i]);
            }

            return values;
        }

        #endregion // Helper Methods
    }
}
