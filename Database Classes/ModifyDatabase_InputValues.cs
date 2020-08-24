///
///     ModifyDatabasE_InputValues.cs
///     Author: David K. Hwang
/// 
///     Class that uses a SqlConnectionServices object to input values into 
///     the connected database.
/// 
///     Contains the logic for checking what values must be input into the database.
/// 
/// 
///


using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;
using CoroStats_BetaTest.Services;

namespace CoroStats_BetaTest.Database_Classes
{
    class ModifyDatabase_InputValues
    {
        #region Fields

        private SqlConnectionServices _sqlService;
        private ExcelFileParsingService _parser;

        #endregion // Fields

        #region Constructor

        public ModifyDatabase_InputValues(SqlConnectionServices sqlService, ExcelFileParsingService parser)
        {
            _sqlService = sqlService;
            _parser = parser;
        }

        #endregion // Constructor

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public void ParseAndInputValues()
        {
            throw new NotImplementedException();
        }

        #endregion // Public Methods


        #region Private Methods (Logic)

        private string[] searchForDatesInSpreadsheet()
        {
            throw new NotImplementedException();
        }

        private string[] searchForDatesInDatabase()
        {
            throw new NotImplementedException();
        }

        private void addAllValuesFromSpreadsheet()
        {
            throw new NotImplementedException();
        }

        private void addNewValuesFromSpreadsheet()
        {
            throw new NotImplementedException();
        }

        #endregion // Private Methods (Logic)
    }
}
