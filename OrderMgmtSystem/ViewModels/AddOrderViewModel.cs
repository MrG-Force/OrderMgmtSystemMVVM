using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.ViewModels.DialogViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace OrderMgmtSystem.ViewModels
{
    /// <summary>
    /// Provides logic and functionality for the AddOrderView.
    /// </summary>
    /// <remarks>
    /// Note that the navigation command for adding a new item resides in 
    /// the MainWindowModel and its accessed through the view using Binding.
    /// </remarks>
    public class AddOrderViewModel : ViewModelBase
    {
        #region Fields
        private Order _order;
        private OrderItem _selectedItem = null;
        private readonly IDialogService _dialogService;
        #endregion

        #region Constructor
        public AddOrderViewModel()
        {
            OrderItems = new ObservableCollection<OrderItem>();
            RemoveItemCommand = new DelegateCommand<OrderItem>(RemoveItem, (SelectedItem) => _selectedItem != null);
            SubmitOrderCommand = new DelegateCommand(SubmitOrder, () => CanSubmit);
            CancelCurrentOrderCommand = new DelegateCommand(CancelCurrentOrder);
            _dialogService = new DialogService();
            _order = null;
        }
        #endregion

        #region Properties
        public Order Order { get => _order; set => SetProperty(ref _order, value); }
        public ObservableCollection<OrderItem> OrderItems { get; set; }
        public OrderItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                RemoveItemCommand.RaiseCanExecuteChanged();
            }
        }
        public DelegateCommand<OrderItem> RemoveItemCommand { get; private set; }
        public DelegateCommand SubmitOrderCommand { get; private set; }
        public DelegateCommand CancelCurrentOrderCommand { get; private set; }
        public bool CanSubmit => OrderItems.Count > 0;

        /// <summary>
        /// Happens when an item is removed from the order
        /// </summary>
        public event Action<int, int, int> OrderItemRemoved = delegate { };
        /// <summary>
        /// Happens when the order is submitted.
        /// </summary>
        public event Action<Order> OrderSubmitted = delegate { };
        /// <summary>
        /// Happens when the current order is cancelled.
        /// </summary>
        public event Action OrderCancelled = delegate { };
        #endregion

        #region Methods
        /// <summary>
        /// Starts a new empty order when the user navigates to AddOrderView.
        /// </summary>
        /// <remarks>It is called from the MainWindowModel</remarks>
        /// <param name="newOrder"></param>
        public void LoadNewOrder(Order newOrder)
        {
            _order = newOrder;
        }

        /// <summary>
        /// Adds the passed Item to the order.
        /// </summary>
        /// <remarks>It is used in conjunction with the NavigateCommand in the MainWindowModel.</remarks>
        /// <param name="newItem"></param>
        internal void AddOrderItem(OrderItem newItem)
        {
            // TODO: check if an item with the same id is already in the order
            OrderItem repItem = OrderItems
                .FirstOrDefault(item => item.StockItemId == newItem.StockItemId);
            if (repItem == null)
            {
                newItem.OrderHeaderId = Order.Id;
                OrderItems.Add(newItem);
                Order.AddItem(newItem);
                RaisePropertyChanged(nameof(Order));
                SubmitOrderCommand.RaiseCanExecuteChanged();
            }
            else
            {
                // Notify bindings
                repItem.Quantity += newItem.Quantity;
                // Sincronize items on back order
                repItem.OnBackOrder += newItem.OnBackOrder;
                RaisePropertyChanged(nameof(Order));
                //Order.RaisePropertyChanged(nameof(Order.Total));
            }
        }

        /// <summary>
        /// Removes the passed item from the Order and calls an event handler
        /// to update the quantity of the items in the StockItems list.
        /// </summary>
        /// <param name="item"></param>
        private void RemoveItem(OrderItem item)
        {
            OrderItems.Remove(item);
            Order.RemoveItem(item.StockItemId);
            RaisePropertyChanged(nameof(Order));
            OnOrderItemRemoved(item.StockItemId, item.Quantity, item.OnBackOrder);
            SubmitOrderCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Submits the current order and cleans the form.
        /// </summary>
        private void SubmitOrder()
        {
            // Change order state calling Order.Submit()
            Order.Submit();
            // MainWindow grabs this.Order and adds it to the DB and updates OrdersList
            OnOrderSubmitted(Order);
            // Clear OrderItems prop
            OrderItems.Clear();
            // Set this.Order to null
            Order = null;
        }

        /// <summary>
        /// Cancels the current order. Opens a dialog to confirm cancelation.
        /// </summary>
        /// <remarks>
        /// Reclaims the generated OrderId, clears the OrderItem Lis and updates the stocks
        /// </remarks>
        internal void CancelCurrentOrder()
        {
            bool result = true;
            // if order has items
            if (CanSubmit)
            {
                string message = "This order and all its data will be permanently deleted!";
                string title = $"Cancel order: {Order.Id}";
                var dialogViewModel = new CancelOrderDialogViewModel(title, message);
                result = _dialogService.OpenDialog(dialogViewModel);
            }
            if (result)
            {
                Order.CancelLastOrder();
                // Return Items to stock
                foreach (OrderItem item in OrderItems)
                {
                    // Use the eventhandler to updates the stock quantities
                    OnOrderItemRemoved(item.StockItemId, item.Quantity, item.OnBackOrder);
                }
                OrderItems.Clear();
                SubmitOrderCommand.RaiseCanExecuteChanged();
                Order = null;
                OnOrderCancelled();
            }
        }

        /// <summary>
        /// Raises the OrderItemRemoved event and sends the information to the subscribers.
        /// </summary>
        /// <remarks>The MainWIndowModel handles this event and notifies the StockItem List.</remarks>
        /// <param name="stockItemId">The id of the item that was removed</param>
        /// <param name="quantity">How Many items were removed</param>
        /// <param name="onBackOrder">Items on back order</param>
        private void OnOrderItemRemoved(int stockItemId, int quantity, int onBackOrder)
        {
            OrderItemRemoved(stockItemId, quantity, onBackOrder);
        }

        private void OnOrderSubmitted(Order order)
        {
            OrderSubmitted(order);
        }

        private void OnOrderCancelled()
        {
            OrderCancelled();
        }
        #endregion
    }
}
