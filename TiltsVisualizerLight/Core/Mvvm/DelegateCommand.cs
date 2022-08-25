using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mvvm.Core {
    public class DelegateCommand<T> : ICommand {
        Func<T, bool> canExecuteMethod = null;
        Action<T> executeMethod = null;

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }
        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(T parameter) {
            if(canExecuteMethod == null)
                return true;
            return canExecuteMethod(parameter);
        }
        void ICommand.Execute(object parameter) {
            Execute((T)parameter);
        }
        public void Execute(T parameter) {
            if(!CanExecute(parameter))
                return;
            if(executeMethod == null)
                return;
            executeMethod(parameter);
        }
        bool ICommand.CanExecute(object parameter) {
            return CanExecute((T)parameter);
        }
    }

    public class DelegateCommand : DelegateCommand<object> {
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(
                executeMethod != null ? (Action<object>)(o => executeMethod()) : null,
                canExecuteMethod != null ? (Func<object, bool>)(o => canExecuteMethod()) : null) {
        }
    }
}
