using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Windows;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderMgmtSystem.ViewModels
{
    /// <summary>
    /// This class provides the logic and functionality to the AddItemView.
    /// </summary>
    public class AddItemViewModel : ViewModelBase
    {
        #region Constructor
        public AddItemViewModel(List<StockItem> stockItems, IDialogService dialogService, DialogViewModelBase<int> dialogViewModelBase)
        {
            _dialogService = dialogService;
            _dialogViewModel = dialogViewModelBase;
            StockItems = stockItems;
            RequestAddItemCommand = new DelegateCommand<ViewModelBase>(RequestAddItem);
        }
        #endregion

        #region Fields
        private readonly IDialogService _dialogService;
        private readonly DialogViewModelBase<int> _dialogViewModel;
        private StockItem _selectedStockItem;
        #endregion

        #region Properties
        public StockItem SelectedStockItem
        {
            get => _selectedStockItem;
            set
            {
                _selectedStockItem = value;
                RaisePropertyChanged();
            }
        }
        public List<StockItem> StockItems { get; set; }
        public DelegateCommand<ViewModelBase> RequestAddItemCommand { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when an item is selected and added to a new order.
        /// </summary>
        /// <subscribers>MainWindowViewModel</subscribers>
        public event EventHandler<OrderItem> NewOrderItemSelected;

        /// <summary>
        /// Occurs when an OrderItem is ready to be added to the Order in the EditOrder View.
        /// </summary>
        /// <subscribers>ChildWindowViewModel</subscribers>
        public event EventHandler<OrderItem> EditingOrderItemSelected;
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new OrderItem and updates the corresponding InStock property 
        /// in the StockItems list. 
        /// </summary>
        /// <remarks>Calls the GetQuantity method that opens a dialog</remarks>
        /// <param name="vM">The DataContext of the calling Window</param>
        public void RequestAddItem(ViewModelBase vM)
        {
            int availableStock = SelectedStockItem.InStock;
            _dialogViewModel.AvailableStock = availableStock;
            // Fetch a quantity from the user
            int qty = GetQuantity(_dialogViewModel);
            if (qty == 0)
            {
                return;
            }
            // Update the StockItems collection
            StockItem changedItem = StockItems
                .FirstOrDefault(item => item.Id == SelectedStockItem.Id);

            // The StockItem class implements INotifyPropertyChanged and raises PropertyChanged on StockItems to notify bindings
            // The StockItem class handles the negative stock if not enough items available
            if (changedItem != null) changedItem.InStock -= qty;

            // Create the new OrderItem
            OrderItem orderItem = new OrderItem(SelectedStockItem)
            {
                Quantity = qty
            };
            if (qty > availableStock)
            {
                orderItem.OnBackOrder = qty - availableStock;
            }

            // Raise the events depending on the ViewModel
            if (vM is MainWindowViewModel)
            {
                OnNewOrderItemSelected(orderItem);
            }
            else if (vM is ChildWindowViewModel)
            {
                OnEditingOrderItemSelected(orderItem);
            }
        }

        /// <summary>
        /// Shows a dialog to allow the user to select the quantity of the selected item.
        /// </summary>
        /// <returns></returns>
        private int GetQuantity(DialogViewModelBase<int> quantityViewModel)
        {
            int result = _dialogService.OpenDialog(quantityViewModel);
            return result;
        }

        /// <summary>
        /// Updates the InStock property of the StockItem with the given Id.
        /// </summary>
        /// <param name="stockItemId"></param>
        /// <param name="quantity"></param>
        internal void ReturnItemToStockList(int stockItemId, int quantity, int onBackOrder)
        {
            StockItem returnedItem = StockItems
                .FirstOrDefault(item => item.Id == stockItemId);
            if (returnedItem != null)
            {
                returnedItem.InStock += quantity - onBackOrder;
            }
        }

        /// <summary>
        /// Raises NewOrderItemSelected event and passes the OrderItem to the listeners.
        /// </summary>
        /// <param name="orderItem"></param>
        private void OnNewOrderItemSelected(OrderItem orderItem)
        {
            NewOrderItemSelected?.Invoke(this, orderItem);
        }

        /// <summary>
        /// Raises EditingOrderItemSelected event and passes the OrderItem to the listeners.
        /// </summary>
        /// <param name="orderItem"></param>
        private void OnEditingOrderItemSelected(OrderItem orderItem)
        {
            EditingOrderItemSelected?.Invoke(this, orderItem);
        }
        #endregion
    }
}