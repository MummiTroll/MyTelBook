using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TelBook
{
    public class Command : ICommand
    {
        public Action _action;
        public Func<bool> _canExecute;
        public Command(Action action, Func<bool> canExecute)
        {
            _action = action; _canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke();
        }
        public void Execute(object parameter)
        {
            _action();
        }
    }
}
