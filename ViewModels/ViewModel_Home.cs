///
///     ViewModel_Home.cs
///     author: David K. Hwang    
/// 
/// 
///     Contains Data/Variables to be shown on the CoronaStatsHome page
/// 
///



using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace CoroStats_BetaTest.ViewModels
{
    class ViewModel_Home : ViewModelBase
    {
        public DateTime DateToday { get { return DateTime.Today; } }
        public DateTime DateNow { get { return DateTime.Now; } }

        ReadOnlyCollection<CommandViewModel> _leftMenuCommands;

        public ViewModel_Home(ICommand addData, ICommand settings)
        {
            base.DisplayName = "Corona Stats - Home";

        }

        private long _totalCases;
        public long TotalCases
        {
            get => _totalCases;
            set => SetProperty(ref _totalCases, value);
        }

        private bool _dbIsInitialized;
        public bool DbIsInitialized
        {
            get => _dbIsInitialized;
            set => SetProperty(ref _dbIsInitialized, value);
        }

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

        List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel(
                    "Add Data",
                    new RelayCommand(param => this.ShowAllCustomers())),

                new CommandViewModel(
                    Strings.MainWindowViewModel_Command_CreateNewCustomer,
                    new RelayCommand(param => this.CreateNewCustomer()))
            };
        }

        


    }

    
}
