using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderMgmtSystem.ViewModels
{
    public class AddItemViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly DialogViewModelBase<int> _dialogViewModel;
        private StockItem _selectedStockItem;


        public AddItemViewModel(List<StockItem> stockItems, IDialogService dialogService, DialogViewModelBase<int> dialogViewModelBase)
        {
            _dialogService = dialogService;
            _dialogViewModel = dialogViewModelBase;
            StockItems = stockItems;
            AddItemCommand = new DelegateCommand<ViewModelBase>(AddItem);

        }

        /// <summary>
        /// Happens when an item is selected and added to the order to trigger Closing the (modal)View.
        /// </summary>
        // delegate trick: assign an empty anonymous method as a subscriber, no need to worry about PropertyChanged being null.
        public event Action<OrderItem> NewOrderItemSelected = delegate { };
        public event Action<OrderItem> EditingOrderItemSelected = delegate { };

        // --- props
        public StockItem SelectedStockItem
        {
            get => _selectedStockItem;
            set => SetProperty(ref _selectedStockItem, value);
        }

        public DelegateCommand<ViewModelBase> AddItemCommand { get; private set; }
        public List<StockItem> StockItems { get; set; }

        /// <summary>
        /// This method creates a new OrderItem and updates the corresponding InStock property 
        /// in the StockItems list. Provides the functionality to the AddItemCommand that 
        /// is bound to the "Add to order" button in the view. 
        /// </summary>
        /// <remarks>Calls the GetQuantity method that opens a dialog</remarks>
        /// <param name="selectedItem">(Binding)The stock item corresponding to the row where the button is located</param>
        public void AddItem(ViewModelBase vm)
        {
            string vMName = vm.GetType().Name;
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
            // The StockItem class handles the negative stock if not enoug items available
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

            // TODO, This is a bitch when trying to use the modal in other windows
            // Pass the new item Raise NewOrderItemSelected to notify MainWindow and close this Modal View
            if (vm is MainWindowViewModel)
            {
                OnNewOrderItemSelected(orderItem);
            }
            else if (vm is EditOrderViewModel)
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
            // Call a Dialog service
            // Get Valid quantity from the user
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
            NewOrderItemSelected(orderItem);
        }
        private void OnEditingOrderItemSelected(OrderItem orderItem)
        {
            EditingOrderItemSelected(orderItem);
        }
    }
}