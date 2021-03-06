﻿using System;
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
using CoroStats_BetaTest.ViewModels;

namespace CoroStats_BetaTest
{
    /// <summary>
    /// Interaction logic for CoronaStatsHome.xaml
    /// </summary>
    public partial class CoronaStatsHome : Page
    {
        public CoronaStatsHome()
        {
            // Initialize ViewModel for Homepage
            

            

            // try to open SQL connection to retrieve base values to display on home screen
            SqlConnectionService sqlCon = new SqlConnectionService();
            sqlCon.OpenConnection();

            //// check if database has already been initialized
            //if (sqlCon.InitializeDB())
            //{
            //    // do nothing; true => DB has already been initialized
            //}
            //else
            //{
            //    // splash screen; tell user to input initial data
            //    MessageBox.Show("Detected First Time Use: Please Input Data Through \n \t Settings --> Initialize Database");
            //}
            
            InitializeComponent();
        }

        /// <summary>
        /// Add Data Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_AddData(object sender, RoutedEventArgs e)
        {
            // GoTo Add Data Page
            View_AddData view_AddData = new View_AddData();
            this.NavigationService.Navigate(view_AddData);

        }

        private void Button_Click_Settings(object sender, RoutedEventArgs e)
        {
            View_Settings view_Settings = new View_Settings();
            this.NavigationService.Navigate(view_Settings);
        }
    }
}