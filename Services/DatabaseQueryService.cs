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

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<object> vals = ReadSingleRow((IDataRecord)reader);
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

        public int GetWHO_Region(string WHO_region)
        {
            Object value = -1;

            _connService.OpenConnection();

            try
            {
                SqlCommand command = new SqlCommand("Procedure_SearchFor_WHO_Region", _connService.Conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@WHO_Region", WHO_region);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        value = ReadSingleValue((IDataRecord)reader);  
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return (int)value;
        }

        /// <summary>
        /// Searches DB for country specified and returns country ID number if found
        /// </summary>
        /// <param name="countryName">Country Name</param>
        /// <returns>Country ID; = -1 if country isn't in DB</returns>
        public int FindCountryInDB_ReturnCountryId(string countryName)
        {
            // variable
            string qString = "SELECT CountryId FROM dbo.CountryInfo WHERE Name = @NAME";
            int countryId = -1;

            _connService.OpenConnection();

            try
            {
                SqlCommand cmd = new SqlCommand(qString, _connService.Conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@NAME", countryName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        countryId = (int)ReadSingleValue((IDataRecord)reader);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return countryId;
        }

        /// <summary>
        /// Checks if country is present in DB
        /// </summary>
        /// <param name="countryName">Country Name</param>
        /// <returns>false if not present; true if present</returns>
        public bool IsCountryPresentInDB(string countryName)
        {
            // variable
            string qString = "SELECT CountryId FROM dbo.CountryInfo WHERE Name = @NAME";
            bool isPresent = false;

            _connService.OpenConnection();

            try
            {
                SqlCommand cmd = new SqlCommand(qString, _connService.Conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@NAME", countryName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        isPresent = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return isPresent;
        }

        public List<string> GetAllCountriesInDb()
        {
            // variable
            List<string> countryList = new List<string>();
            string qString = "SELECT [Name] FROM dbo.CountryInfo";

            _connService.OpenConnection();

            try
            {
                SqlCommand cmd = new SqlCommand(qString, _connService.Conn);
                cmd.CommandType = CommandType.Text;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        reader.Read();
                        countryList.Add((string)ReadSingleValue((IDataRecord)reader));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return countryList;
        }


        public int FindDateInDB_ReturnDateId(string date)
        {
            // variable
            string qString = "SELECT DateId FROM dbo.CoronavirusDate WHERE Date = @DATE";
            int dateId = -1; ;

            _connService.OpenConnection();

            try
            {
                SqlCommand cmd = new SqlCommand(qString, _connService.Conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DATE", date);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        dateId = (int)ReadSingleValue((IDataRecord)reader);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return dateId;
        }

        public bool IsDateDataPresent(int countryId, int dateId)
        {
            // variable
            string qString = "SELECT * FROM dbo.NewCoronavirusCasesByDate WHERE CountryId = @COUNTRYID AND DateId = @DATEID";
            bool isPresent = false;

            _connService.OpenConnection();

            try
            {
                SqlCommand cmd = new SqlCommand(qString, _connService.Conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@COUNTRYID", countryId);
                cmd.Parameters.AddWithValue("@DATEID", dateId);


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        isPresent = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return isPresent;
        }

        public int GetNewCases_SingleCountrySingleDate(int countryId, int dateId)
        {
            // variable
            string qString = "SELECT NewCases FROM dbo.NewCoronavirusCasesByDate WHERE CountryId = @COUNTRYID AND DateId = @DATEID";
            int newCases = -1; ;

            _connService.OpenConnection();

            try
            {
                SqlCommand cmd = new SqlCommand(qString, _connService.Conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@COUNTRYID", countryId);
                cmd.Parameters.AddWithValue("@DATEID", dateId);


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        newCases = (int)ReadSingleValue((IDataRecord)reader);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return newCases;
        }

        public List<string> GetDatesOnFile_ByCountryName(string countryName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to get dates on file for each country on file
        /// </summary>
        /// <returns>A list of tuples with (string countryName, string date)</returns>
        public Dictionary<string, List<string>> GetDatesOnFile_AllCountries()
        {
            // variable
            //List<(string, string)> countryDates = new List<(string, string)>();
            Dictionary<string, List<string>> dict_countryDate = new Dictionary<string, List<string>>();
            string qString = "SELECT [Name], [Date] "
                            +  "FROM dbo.CountryInfo " 
                            + "JOIN dbo.NewCoronavirusCasesByDate ON CountryInfo.CountryId = NewCoronavirusCasesByDate.CountryId "
                            + "JOIN dbo.CoronavirusDate ON NewCoronavirusCasesByDate.DateId = CoronavirusDate.DateId";

            _connService.OpenConnection();

            try
            {
                SqlCommand cmd = new SqlCommand(qString, _connService.Conn);
                cmd.CommandType = CommandType.Text;
                List<Object> vals;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                         vals = ReadSingleRow((IDataRecord)reader);
                        //countryDates.Add(((string)vals[0], (string)vals[1]));
                        if (!dict_countryDate.ContainsKey((string)vals[0])) dict_countryDate[(string)vals[0]] = new List<string>();
                        if (!dict_countryDate[(string)vals[0]].Contains((string)vals[1])) dict_countryDate[(string)vals[0]].Add((string)vals[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            _connService.CloseConnection();

            return dict_countryDate;
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

        /// <summary>
        /// Helper Method to read a single row from database
        /// </summary>
        /// <param name="data">database record object representing a single row</param>
        /// <returns>List of objects that represent a single row of values returned from database query</returns>
        private List<Object> ReadSingleRow(IDataRecord data)
        {
            List<Object> values = new List<Object>();
            for (int i = 0; i < data.FieldCount; i++)
            {
                values.Add(data[i]);
            }

            return values;
        }

        /// <summary>
        /// Helper Method to read a single value returned from the database
        /// </summary>
        /// <param name="data">database record object representing a single row</param>
        /// <returns>the first value returned from the database query</returns>
        private Object ReadSingleValue(IDataRecord data)
        {
            return data[0];
        }

        #endregion // Helper Methods
    }
}
