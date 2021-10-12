using OrderMgmtSystem.ViewModels.DialogViewModels;

namespace OrderMgmtSystem.Services.Dialogs.TestDialogs
{
    /// <summary>
    /// Provides a mock ViewModel for dialogs useful for unit testing.
    /// </summary>
    public class MockDialogViewModel : QuantityViewModel
    {
        public MockDialogViewModel(string title, string message) : base(title, message)
        {
        }
        public override int DefaultDialogResult => 3;
    }
}
