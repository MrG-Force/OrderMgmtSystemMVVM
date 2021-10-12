namespace OrderMgmtSystem.Services.Dialogs.TestDialogs
{
    /// <summary>
    /// This class provides a mock dialog service that always returns a default value for unit testing.
    /// </summary>
    public class MockDialogService : IDialogService
    {
        /// <summary>
        /// Gets a default value T from the dialog.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public T OpenDialog<T>(DialogViewModelBase<T> viewModel)
        {
            return viewModel.DefaultDialogResult;
        }
    }
}
