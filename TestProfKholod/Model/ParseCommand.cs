using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestProfKholod.Model
{
    public class ParseCommand : ICommand
    {
        
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        

        public ParseCommand(Action<object> execute, Predicate<object> canExecute)
        {
            
            _execute = execute;
            _canExecute = canExecute;
        }

        public ParseCommand(Action<object> execute) : this(execute, null)
        {
            
        }
       

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

