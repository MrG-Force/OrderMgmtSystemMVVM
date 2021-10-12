using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using System.Windows.Input;

namespace OrderMgmtSystem.ViewModels.DialogViewModels
{
    /// <summary>
    /// A class to provide logic and functionality to a success dialog window.
    /// </summary>
    internal class SuccessDialogViewModel : DialogViewModelBase<bool>
    {
        public SuccessDialogViewModel(string title, string message) : base(title, message)
        {
            ProceedCommand = new DelegateCommand<IDialogWindow>(Proceed);
        }
        public override bool DefaultDialogResult => true;
        public ICommand ProceedCommand { get; private set; }
        private void Proceed(IDialogWindow window)
        {
            CloseDialogWithResult(window, true);
        }
    }
}
