using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CoroStats_BetaTest.Pages
{
    /// <summary>
    /// Interaction logic for View_AddDataFromSpreadsheet.xaml
    /// </summary>
    public partial class View_AddDataFromSpreadsheet : Window
    {
        private readonly Action _onWindowClose;

        public View_AddDataFromSpreadsheet()
        {

        }

        public View_AddDataFromSpreadsheet(Action onWindowClose)
        {
            InitializeComponent();
            _onWindowClose = onWindowClose;
        }

        void View_AddDataManually_Closed(object sender, EventArgs e)
        {
            _onWindowClose();
        }
    }
}
