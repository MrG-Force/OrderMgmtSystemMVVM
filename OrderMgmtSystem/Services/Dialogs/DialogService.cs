namespace OrderMgmtSystem.Services.Dialogs
{
    /// <summary>
    /// A class to provide a dialogs.
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Opens a DialogWindow that returns a generic type value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <returns>A result of type <T> from the dialog ViewModel</returns>
        public T OpenDialog<T>(DialogViewModelBase<T> viewModel)
        {
            IDialogWindow window = new DialogWindow
            {
                DataContext = viewModel
            };
            if ((bool)window.ShowDialog())
            {
                return viewModel.DialogResult;
            }
            return viewModel.DefaultDialogResult;
        }
    }
}