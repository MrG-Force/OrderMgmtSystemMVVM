using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using System.Windows.Input;

namespace OrderMgmtSystem.ViewModels.DialogViewModels
{
    internal class CancelOrderDialogViewModel : DialogViewModelBase<bool>
    {
        public ICommand ProceedCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public CancelOrderDialogViewModel(string title, string message) : base(title, message)
        {
            ProceedCommand = new DelegateCommand<IDialogWindow>(Proceed);
            CancelCommand = new DelegateCommand<IDialogWindow>(Cancel);
        }

        public override bool DefaultDialogResult { get => true; }

        private void Proceed(IDialogWindow window)
        {
            CloseDialogWithResult(window, true);
        }
        private void Cancel(IDialogWindow window)
        {
            CloseDialogWithResult(window, false);
        }
    }
}
