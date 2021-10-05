using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using OrderMgmtSystem.ViewModels.DialogViewModels;
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
            _dialogViewModel = (QuantityViewModel)dialogViewModelBase;
            StockItems = stockItems;
            RequestAddItemCommand = new DelegateCommand<string>(RequestAddItem);
        }
        #endregion

        #region Fields
        private readonly IDialogService _dialogService;
        private readonly QuantityViewModel _dialogViewModel;
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
        public DelegateCommand<string> RequestAddItemCommand { get; private set; }
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
        /// <param name="viewModel">The name of the calling Window</param>
        public void RequestAddItem(string viewModel)
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
            if (viewModel == "MainWindow")
            {
                OnNewOrderItemSelected(orderItem);
            }
            else if (viewModel is "ChildWindow")
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
        public void ReturnItemToStockList(OrderItem orderItem)
        {
            StockItem returnedItem = StockItems
                .FirstOrDefault(item => item.Id == orderItem.StockItemId);
            returnedItem.InStock += orderItem.Quantity - orderItem.OnBackOrder;
        }
        /// <summary>
        /// This method deals with the operationCancelled event updating the StockItems amount by
        /// subtracting the corrsponding items from the StockItems list.
        /// </summary>
        /// <param name="items"></param>
        public void UpdateItemsReturnedToOrder(List<OrderItem> items)
        {
            foreach (var item in items)
            {
                StockItem stockItem = StockItems
                    .FirstOrDefault(i => i.Id == item.StockItemId);
                stockItem.InStock -= item.Quantity - item.OnBackOrder;
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