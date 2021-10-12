namespace OrderMgmtSystem.Services
{
    /// <summary>
    /// Defines a service to display dialogs and get a result type T.
    /// </summary>
    public interface IDialogService
    {
        T OpenDialog<T>(DialogViewModelBase<T> viewModel);
    }
}
