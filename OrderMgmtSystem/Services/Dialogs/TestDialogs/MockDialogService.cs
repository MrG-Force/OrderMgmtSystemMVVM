namespace OrderMgmtSystem.Services.Dialogs.TestDialogs
{
    public class MockDialogService : IDialogService
    {
        public T OpenDialog<T>(DialogViewModelBase<T> viewModel)
        {
            return viewModel.DefaultDialogResult;
        }
    }
}
