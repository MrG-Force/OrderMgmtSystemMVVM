using OrderMgmtSystem.ViewModels.DialogViewModels;

namespace OrderMgmtSystem.Services.Dialogs.TestDialogs
{
    public class MockDialogViewModel : QuantityViewModel
    {
        public MockDialogViewModel(string title, string message): base(title, message)
        {
            DialogResult = 3;
        }
    }
}
