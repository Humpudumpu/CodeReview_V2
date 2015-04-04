using System;
using System.Diagnostics;
using System.Windows.Input;


namespace CodeReview_V2
{
    public class Command : ICommand
    {
        #region Fields
        private Action<object> action;
        #endregion

        public Command(Action<object> action)
        {
            this.action = action;
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
