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
        public T DialogResult { get; set; }
        public virtual T DefaultDialogResult { get; set; }

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

        /// <summary>
        /// Sets the DialogResult of this ViewModel and closes the dialog 
        /// by setting its DialogResult to true.
        /// </summary>
        /// <param name="dialog">Object passed through binding</param>
        /// <param name="result">This ViewModel result</param>
        public void CloseDialogWithResult(IDialogWindow dialog, T result)
        {
            this.DialogResult = result;
            if (dialog != null)
            {
                dialog.DialogResult = true;
            }
        }

        /// <summary>
        /// Sets the DialogResult of this ViewModel and closes the dialog
        /// by setting its DialogResult to false.
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="result"></param>
        public void CancelAndClose(IDialogWindow dialog, T result)
        {
            this.DialogResult = result;
            if (dialog != null)
            {
                dialog.DialogResult = false;
            }
        }
    }
}
