///
///     ViewModel_AddData.cs
///     author: David K. Hwang    
/// 
/// 
///     Contains Data/Variables to be shown on the Add Data page
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
    class ViewModel_AddData : ViewModel_SelectedWindow
    {
        public ViewModel_AddData()
        {
            this.DisplayName = "Corona Stats - Add Data";
        }
    }
}
