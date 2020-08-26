///
///     DatabaseModificationService.cs
///     Author: David K. Hwang
/// 
///     Houses Methods to modify the database .mdf file
/// 
/// 
///


using System.Data.SqlClient;

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
    }
}
