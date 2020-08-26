using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using CoroStats_BetaTest.Services;

namespace CoroStats_BetaTest.ViewModels
{
    public class ViewModel_MainWindow : ViewModelBase
    {
        #region Fields

        private ReadOnlyCollection<CommandViewModel> _leftMenuCommands;
        private ContentControl _currentContent;
        private Dictionary<string, ViewModelBase> _viewModelStore;
        private SqlConnectionService _connService;
        private DatabaseIntegrityService _integrityService;
        public DatabaseQueryService qService;
        public DatabaseModificationService modService;

        #endregion // Fields

        #region Constructor

        public ViewModel_MainWindow(SqlConnectionService connService)
        {
            base.DisplayName = "MainWindow - Home";
            _viewModelStore = new Dictionary<string, ViewModelBase>();
            _connService = connService;
            qService = new DatabaseQueryService(_connService);
            modService = new DatabaseModificationService(_connService);
            _integrityService = new DatabaseIntegrityService(_connService, qService, modService);

            // Databse Integrity Check => If database isn't present, creates new database file
            _integrityService.DatabaseCheckOnStartup();
        }

        #endregion // Constructor

        #region Presentation Properties

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

        public ContentControl CurrentContent
        {
            get
            {
                if (_currentContent == null)
                {
                    _currentContent = new ContentControl();
                    _viewModelStore.Add("Home", new ViewModel_Home(qService));
                    _currentContent.Content = _viewModelStore["Home"];
                }
                return _currentContent;
            }
        }

        #endregion // Presentation Properties

        #region Left Menu Commands


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
            ViewModelBase viewModel;
            if (!_viewModelStore.TryGetValue("Home", out viewModel))
            {
                _viewModelStore.Add("Home", new ViewModel_Home(qService));
            }         
            this.CurrentContent.Content = _viewModelStore["Home"];
        }

        void ShowAddDataView()
        {
            ViewModelBase viewModel;
            if (!_viewModelStore.TryGetValue("AddData", out viewModel))
            {
                
                _viewModelStore.Add("AddData", new ViewModel_AddData());
            }
            this.CurrentContent.Content = _viewModelStore["AddData"];
        }

        #endregion // Left Menu Commands

        #region Helper Methods

        private void OnApplicationClose()
        {
            _connService.Conn.Dispose();
        }

        #endregion // Helper Methods


    }
}
