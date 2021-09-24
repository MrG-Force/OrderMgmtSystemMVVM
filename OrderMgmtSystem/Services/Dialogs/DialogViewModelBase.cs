using OrderMgmtSystem.ViewModels;
using OrderMgmtSystem.ViewModels.BaseViewModels;

namespace OrderMgmtSystem.Services
{
    /// <summary>
    /// Base class to implement different types of dialogs with different <T> results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DialogViewModelBase<T> : ViewModelBase
    {
        private string _message;

        public string Title { get; set; }
        public string Message { get => _message; set => SetProperty(ref _message, value); }
        public int AvailableStock { get; set; }
        public T DialogResult { get; set; }

        // This is cool:
        // If the constructor has no parameters:
        //                              Use 'this' third constructor and pass empty strings
        public DialogViewModelBase() : this(string.Empty, string.Empty) { }
        // similarly...
        public DialogViewModelBase(string title) : this(title, string.Empty) { }
        public DialogViewModelBase(string title, string message)
        {
            Title = title;
            _message = message;
        }

        public void CloseDialogWithResult(IDialogWindow dialog, T result)
        {
            this.DialogResult = result;
            if (dialog != null)
            {
                dialog.DialogResult = true;
            }
        }
    }
}
