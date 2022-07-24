using System.Windows.Input;

namespace ChatCommon
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action commandAction;
        private bool canExecute;

        public Command(Action commandAction, bool canExecute = true)
        {
            this.commandAction = commandAction;
            this.canExecute = canExecute;
        }

        public bool CanExecute
        {
            get { return canExecute; }
            set
            {
                if (canExecute != value)
                {
                    canExecute = value;

                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return canExecute;
        }

        void ICommand.Execute(object parameter)
        {
            if (commandAction != null)
            {
                commandAction();
            }
        }
    }
}
