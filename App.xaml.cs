using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CoroStats_BetaTest.ViewModels;
using CoroStats_BetaTest.Services;

namespace CoroStats_BetaTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SqlConnectionService connService = new SqlConnectionService();

            MainWindow window = new MainWindow(connService);

            // Create the ViewModel to which 
            // the main window binds.
            var viewModel = new ViewModel_MainWindow(connService);

            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            window.DataContext = viewModel;

            window.Show();
        }
    }
}
