using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CoroStats_BetaTest.ViewModels
{
    public class CommandViewModel : ViewModelBase
    {
        public CommandViewModel(string displayName, ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            base.DisplayName = displayName;
            this.Command = command;
        }
        public ICommand Command { get; private set; }
    }
}
