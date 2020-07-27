using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace CoroStats_BetaTest.ViewModels
{
    class ViewModel_MainWindow : ViewModelBase
    {
        #region Fields

        ReadOnlyCollection<CommandViewModel> _leftMenuCommands;
        ContentControl _currentContent;
        string _currentContentHeader;

        #endregion // Fields

        #region Constructor
        
        public ViewModel_MainWindow()
        {
            base.DisplayName = "MainWindow - Home";         
        }

        #endregion // Constructor

        public ReadOnlyCollection<CommandViewModel> LeftMenuCommands
        {
            get
            {
                if (_leftMenuCommands == null)
                {
                    List<CommandViewModel> cmds = this.CreateCommands();
                    _leftMenuCommands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _leftMenuCommands;
            }
        }


        #region CurrentContent

        public ContentControl CurrentContent
        {
            get
            {
                if (_currentContent == null)
                {
                    _currentContent = new ContentControl();
                    _currentContent.Content = new ViewModel_Home();
                }
                return _currentContent;
            }
        }

        public string CurrentContentHeader
        {
            get
            {
                if (_currentContentHeader == null)
                {
                    _currentContentHeader = "Home";
                }
                return _currentContentHeader;
            }
            set
            {
                _currentContentHeader = value; 
                this.OnPropertyChanged("CurrentContentHeader");
            }
        }

        #endregion // CurrentContent


        List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel(
                    "Home",
                    new RelayCommand(param => this.ShowHomeView())),
                new CommandViewModel(
                    "Add Data",
                    new RelayCommand(param => this.ShowAddDataView())),
            };
        }

        void ShowHomeView()
        {
            ViewModel_Home viewModel = new ViewModel_Home();
            this.CurrentContent.Content = viewModel;
            this.CurrentContentHeader = "Home";
        }

        void ShowAddDataView()
        {
            ViewModel_AddData viewModel = new ViewModel_AddData();
            this.CurrentContent.Content = viewModel;
            this.CurrentContentHeader = viewModel.DisplayName;
        }


    }
}
