using System;
using System.Windows.Input;

namespace OrderMgmtSystem.Commands
{
    public class RelayCommandT<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public RelayCommandT(Action<T> executeMethod)
        {
            _execute = executeMethod;
        }

        public RelayCommandT(Action<T> executeMethod, Func<bool> canExecute)
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
            _execute?.Invoke((T)parameter);
        }

        internal void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
