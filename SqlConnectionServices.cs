using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Windows;

namespace CoroStats_BetaTest
{
    

    public class SqlConnectionServices
    { 
        /// <summary>
        /// Sql Connection Object
        /// </summary>
        public SqlConnection Con { get; set; }
        /// <summary>
        /// Sql connection string
        /// </summary>
        private string connectionString { get; set; }

        public void OpenConnection()
        {
            // Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\David\source\repos\CoroStats_BetaTest\CoronaStatsDB.mdf;Integrated Secu
            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\David\\source\\repos\\CoroStats_BetaTest\\CoronaStatsDB.mdf;Integrated Security=True";
            Con = new SqlConnection(connectionString);
            try
            {
                Con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void CloseConnection()
        {
            Con.Close();
        }
    }
}
