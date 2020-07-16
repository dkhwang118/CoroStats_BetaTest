///
///     ViewModel_SelectedWindow.cs
///     author: David K. Hwang
///     
///     Abstract base class representing the view that is shown to the right of the control panel 
///     Credit to: https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern
/// 
///



using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Text;

namespace CoroStats_BetaTest.ViewModels
{
    public abstract class ViewModel_SelectedWindow : ViewModelBase
    {
        #region Fields

        RelayCommand _navigateToCommand;

        #endregion // Fields

        #region Constructor

        protected ViewModel_SelectedWindow()
        {
        }

        #endregion // Constructor

        #region NavigateToCommand

        /// <summary>
        /// Returns the command that, when invoked, attempts to navigate to the next specified window view
        /// </summary>
        public ICommand NavigateToCommand
        {
            get
            {
                if (_navigateToCommand == null)
                    _navigateToCommand = new RelayCommand(param => this.OnRequestNavigateTo());
                return _navigateToCommand;
            }
        }

        #endregion // NavigateToCommand

        #region RequestNavigateTo [event]

        /// <summary>
        /// Raised when the current selected window should navigate to another window view
        /// </summary>
        public event EventHandler RequestNavigateTo;

        void OnRequestNavigateTo()
        {
            EventHandler handler = this.RequestNavigateTo;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestNavigateTo [event]

    }
}
