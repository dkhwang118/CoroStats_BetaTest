///
///     ViewModelBase_Window.cs
///     Author: David K. Hwang
/// 
///     Class implementing the ViewModelBase class with extra ICommand features to notify the application when the window closes
/// 
///

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CoroStats_BetaTest.ViewModels
{
    class ViewModelBase_Window : ViewModelBase
    {
        #region Fields

        RelayCommand _closeCommand;

        #endregion // Fields

        #region Constructor

        protected ViewModelBase_Window()
        {
        }

        #endregion // Constructor

    }
}
