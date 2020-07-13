using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CoroStats_BetaTest.Pages;

namespace CoroStats_BetaTest
{
    /// <summary>
    /// Interaction logic for CoronaStatsHome.xaml
    /// </summary>
    public partial class CoronaStatsHome : Page
    {
        public CoronaStatsHome()
        {
            InitializeComponent();

            // try to open SQL connection
            SqlConnectionServices SqlCon = new SqlConnectionServices();
            SqlCon.OpenConnection();


        }

        /// <summary>
        /// Add Data Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // GoTo Add Data Page
            View_AddData view_AddData = new View_AddData();
            this.NavigationService.Navigate(view_AddData);

        }
    }
}