using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using System.Windows.Input;

namespace OrderMgmtSystem.ViewModels.DialogViewModels
{
    internal class RejectOrderDialogViewModel : DialogViewModelBase<bool>
    {
        public RejectOrderDialogViewModel(string title, string message, string warning) : base(title, message)
        {
            ProceedCommand = new DelegateCommand<IDialogWindow>(Proceed);
            CancelCommand = new DelegateCommand<IDialogWindow>(Cancel);
            Warning = warning;
        }

        public string Warning { get; set; }

        public ICommand ProceedCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

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
