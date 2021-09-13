using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;

namespace OrderMgmtSystem.ViewModels.DialogViewModels
{
    /// <summary>
    /// Provides the logic for the Quantity selector dialog.
    /// </summary>
    public class QuantityViewModel : DialogViewModelBase<int>
    {
        private string _warningMessage = "This order might be rejected if there is not enough stock when the " +
            "order is processed.";
        private int _numValue;

        public QuantityViewModel(string title, string message) : base(title, message)
        {
            _numValue = 1;
           
            DecreaseQuantityCommand = new DelegateCommand(DecreaseQuantity, () => CanDecrease);
            IncreaseQuantityCommand = new DelegateCommand(IncreaseQuantity, () => CanIncrease);
            AddToOrderCommand = new RelayCommandT<IDialogWindow>(SelectQuantity, () => IsValidQuantity);
        }
        
        public DelegateCommand IncreaseQuantityCommand { get; }
        public DelegateCommand DecreaseQuantityCommand { get; }
        public RelayCommandT<IDialogWindow> AddToOrderCommand { get; }

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
        public bool CanDecrease => NumValue >= 2;
        public bool CanIncrease => NumValue < 99;
        public bool IsValidQuantity => NumValue < 100 && NumValue > 0;
        public bool NotEnoughStock => NumValue > AvailableStock;
        public string WarningMessage { get => _warningMessage; }

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
        /// <param name="window"></param>
        public void SelectQuantity(IDialogWindow window)
        {
            CloseDialogWithResult(window, NumValue);
        }
    }
}
