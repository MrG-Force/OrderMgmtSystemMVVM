using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
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
        /// Happens when an item is selected and added to the order to trigger Closing the (modal)View.
        /// </summary>
        // delegate trick: assign an empty anonymous method as a subscriber, no need to worry about PropertyChanged being null.
        public event Action<OrderItem> NewOrderItemSelected = delegate { };
        public event Action<OrderItem> EditingOrderItemSelected = delegate { };
        #endregion

        #region Constructor
        public AddItemViewModel(List<StockItem> stockItems, IDialogService dialogService, DialogViewModelBase<int> dialogViewModelBase)
        {
            _dialogService = dialogService;
            _dialogViewModel = dialogViewModelBase;
            StockItems = stockItems;
            RequestAddItemCommand = new DelegateCommand<ViewModelBase>(RequestAddItem);
        }
        #endregion

        #region Methods
        private void RequestAddItem(ViewModelBase vM)
        {
            AddItem(vM.GetType().Name);
        }

        /// <summary>
        /// Creates a new OrderItem and updates the corresponding InStock property 
        /// in the StockItems list. Provides the functionality to the AddItemCommand that 
        /// is bound to the "Add to order" button in the view. 
        /// </summary>
        /// <remarks>Calls the GetQuantity method that opens a dialog</remarks>
        /// <param name="vMName">The name of view model set as datacontext of the container window</param>
        public void AddItem(string vMName)
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
            if (vMName.Equals("MainWindowViewModel"))
            {
                OnNewOrderItemSelected(orderItem);
            }
            else if (vMName.Equals("ChildWindowViewModel"))
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
            NewOrderItemSelected(orderItem);
        }

        /// <summary>
        /// Raises EditingOrderItemSelected event and passes the OrderItem to the listeners.
        /// </summary>
        /// <param name="orderItem"></param>
        private void OnEditingOrderItemSelected(OrderItem orderItem)
        {
            EditingOrderItemSelected(orderItem);
        }
        #endregion
    }
}