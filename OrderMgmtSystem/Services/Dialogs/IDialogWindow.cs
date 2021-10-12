namespace OrderMgmtSystem.Services
{
    /// <summary>
    /// Defines the functionality for a dialog window.
    /// </summary>
    public interface IDialogWindow
    {
        bool? DialogResult { get; set; }
        object DataContext { get; set; }

        bool? ShowDialog();
    }
}
