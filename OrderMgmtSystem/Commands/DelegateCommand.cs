using System;
using System.Windows.Input;

namespace OrderMgmtSystem.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action executeMethod)
        {
            _execute = executeMethod;
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecute)
        {
            _execute = executeMethod;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute();
            }
            return _execute != null;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke();
        }

        internal void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> executeMethod)
        {
            _execute = executeMethod;
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecute)
        {
            _execute = executeMethod;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                T tparam = (T)parameter;
                return _canExecute(tparam);
            }
            return _execute != null;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke((T)parameter);
        }
        internal void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
