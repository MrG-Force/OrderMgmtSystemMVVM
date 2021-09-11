using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.ViewModels.DialogViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderMgmtSystem.ViewModels
{
    public class AddItemViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        public AddItemViewModel(List<StockItem> stockItems)
        {
            _dialogService = new DialogService();
            StockItems = new List<StockItem>(stockItems);
            AddItemCommand = new DelegateCommand<StockItem>(AddItem);
        }

        /// <summary>
        /// Happens when an item is selected and added to the order to trigger Closing the (modal)View.
        /// </summary>
        // delegate trick: assign an empty anonymous method as a subscriber, no need to worry about PropertyChanged being null.
        public event Action<OrderItem> OrderItemSelected = delegate { };

        // --- props
        public DelegateCommand<StockItem> AddItemCommand { get; private set; }
        public List<StockItem> StockItems { get; set; }

        /// <summary>
        /// This method provides the functionality to the AddItemCommand which 
        /// is bound to the "Add to order" button in the view. 
        /// </summary>
        /// <remarks>Calls the GetQuantity method that opens a dialog</remarks>
        /// <param name="selectedItem">(Binding)The stock item corresponding to the row where the button is located</param>
        public void AddItem(StockItem selectedItem)
        {
            int availableStock = selectedItem.InStock;
            // Fetch a quantity from the user
            int qty = GetQuantity(availableStock);
            if (qty == 0)
            {
                return;
            }
            // Update the StockItems collection
            StockItem changedItem = StockItems
                .FirstOrDefault(item => item.Id == selectedItem.Id);

            // The StockItem class implements INotifyPropertyChanged and raises PropertyChanged on StockItems to notify bindings
            // The StockItem class handles the negative stock if not enoug items available
            if (changedItem != null) changedItem.InStock -= qty;

            // Create the new OrderItem
            OrderItem orderItem = new OrderItem(selectedItem)
            {
                Quantity = qty
            };
            if (qty > availableStock)
            {
                orderItem.OnBackOrder = qty - availableStock;
            }

            // Pass the new item Raise OrderItemSelected to notify MainWindow and close this Modal View
            OnOrderItemSelected(orderItem);
        }

        /// <summary>
        /// Shows a dialog to allow the user to select the quantity of the selected item.
        /// </summary>
        /// <returns></returns>
        private int GetQuantity(int stock)
        {
            // Call a Dialog service
            var qtyDialogModel = new QuantityViewModel("Quantity", "Please enter a quantity:", stock);
            // Get Valid quantity from the user
            int result = _dialogService.OpenDialog(qtyDialogModel);
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
        /// Raises OrderItemSelected event and passes the OrderItem to the listeners.
        /// </summary>
        /// <param name="orderItem"></param>
        private void OnOrderItemSelected(OrderItem orderItem)
        {
            OrderItemSelected(orderItem);
        }
    }
}