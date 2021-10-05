using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;

namespace OrderMgmtSystem.ViewModels.DialogViewModels
{
    /// <summary>
    /// Provides the logic for the Quantity selector dialog.
    /// </summary>
    public class QuantityViewModel : DialogViewModelBase<int>
    {
        private int _numValue;

        public QuantityViewModel(string title, string message) : base(title, message)
        {
            _numValue = 1;

            DecreaseQuantityCommand = new DelegateCommand(DecreaseQuantity, () => CanDecrease);
            IncreaseQuantityCommand = new DelegateCommand(IncreaseQuantity, () => CanIncrease);
            AddToOrderCommand = new RelayCommandT<IDialogWindow>(SelectQuantity, () => IsValidQuantity);
            CancelSelectQuantityCommand = new RelayCommandT<IDialogWindow>(CancelSelectQuantity);
        }

        public DelegateCommand IncreaseQuantityCommand { get; }
        public DelegateCommand DecreaseQuantityCommand { get; }
        public RelayCommandT<IDialogWindow> AddToOrderCommand { get; }
        public RelayCommandT<IDialogWindow> CancelSelectQuantityCommand { get; }

        public int NumValue
        {
            get { return _numValue; }
            set
            {
                SetProperty(ref _numValue, value);
                RaisePropertyChanged(nameof(CanDecrease));
                RaisePropertyChanged(nameof(NotEnoughStock));
                DecreaseQuantityCommand.RaiseCanExecuteChanged();
                AddToOrderCommand.RaiseCanExecuteChanged();
            }
        }
        public int AvailableStock { get; set; }
        public bool CanDecrease => NumValue >= 2;
        public bool CanIncrease => NumValue < 99;
        public bool IsValidQuantity => NumValue < 100 && NumValue > 0;
        public bool NotEnoughStock => NumValue > AvailableStock;
        public string WarningMessage { get; } = "This order might be rejected if there is not enough stock when the " +
            "order is processed.";
        public override int DefaultDialogResult => 0;

        /// <summary>
        /// A simple method to bind to the increase quantity button.
        /// </summary>
        private void IncreaseQuantity()
        {
            NumValue++;
        }
        /// <summary>
        ///  A simple method to bind to the decrease quantity button.
        /// </summary>
        private void DecreaseQuantity()
        {
            NumValue--;
        }

        /// <summary>
        /// Calls the CloseDialogResult method from the base class to get the result and close
        /// the dialog.
        /// </summary>
        /// <param name="window">An IDialog object Passed through binding from the view</param>
        public void SelectQuantity(IDialogWindow window)
        {
            CloseDialogWithResult(window, NumValue);
            NumValue = 1;
        }

        /// <summary>
        /// Binds to the cancel button and calls the CancelAndClose method from the base class.
        /// </summary>
        /// <param name="window">An IDialog object Passed through binding from the view</param>
        public void CancelSelectQuantity(IDialogWindow window)
        {
            CancelAndClose(window, DefaultDialogResult);
            NumValue = 1;
        }
    }
}
