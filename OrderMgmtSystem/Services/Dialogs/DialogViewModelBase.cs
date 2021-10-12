using OrderMgmtSystem.ViewModels.BaseViewModels;

namespace OrderMgmtSystem.Services
{
    /// <summary>
    /// Base class to implement different types of dialogs with different T results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DialogViewModelBase<T> : ViewModelBase
    {
        #region Constructors
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public DialogViewModelBase() : this(string.Empty, string.Empty) { }
        /// <summary>
        /// A constructor that takes a string and uses as Title property.
        /// </summary>
        /// <param name="title"></param>
        public DialogViewModelBase(string title) : this(title, string.Empty) { }
        /// <summary>
        /// A constructor that takes two strings as arguments and use them as Title and Message properties.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public DialogViewModelBase(string title, string message)
        {
            Title = title;
            _message = message;
        }
        #endregion

        #region Fields
        private string _message;
        #endregion

        #region Properties
        /// <summary>
        /// Defines the Title that is exposed to a UserControl
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Defines a message to display in a UserControl
        /// </summary>
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        /// <summary>
        /// Defines a result value for this Dialog.
        /// </summary>
        public T DialogResult { get; set; }

        /// <summary>
        /// Defines a default result value for this Dialog.
        /// </summary>
        public virtual T DefaultDialogResult { get; set; }
        #endregion

        #region Methods
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
        #endregion
    }
}