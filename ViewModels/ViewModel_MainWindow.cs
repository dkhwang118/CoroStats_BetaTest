﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace CoroStats_BetaTest.ViewModels
{
    class ViewModel_MainWindow : ViewModelBase
    {
        #region Fields

        ReadOnlyCollection<CommandViewModel> _leftMenuCommands;
        ViewModel_SelectedWindow _currentWindow;

        #endregion // Fields

        #region Constructor
        
        public ViewModel_MainWindow()
        {
            base.DisplayName = "MainWindow - Home";
            _currentWindow = new ViewModel_Home();
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


        #region CurrentWindow

        public ViewModel_SelectedWindow CurrentWindow
        {
            get
            {
                if (_currentWindow == null)
                {
                    _currentWindow = new ViewModel_Home();
                }
                return _currentWindow;
            }
        }

        #endregion // CurrentWindow


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
            this._currentWindow = viewModel;
        }

        void ShowAddDataView()
        {
            ViewModel_AddData viewModel = new ViewModel_AddData();
            this.SetProperty<ViewModel_SelectedWindow>(ref _currentWindow, viewModel);
        }


    }
}