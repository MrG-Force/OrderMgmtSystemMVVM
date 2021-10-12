using DataModels;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System.Collections.ObjectModel;
using System.Linq;

namespace OrderMgmtSystem.ViewModels
{
    /// <summary>
    /// Provides logic and functionality for the AddOrderView.
    /// </summary>
    /// <remarks>
    /// The navigation command for adding a new item resides in 
    /// the MainWindowModel and its accessed through the view using Binding.
    /// </remarks>
    public class AddOrderViewModel : SingleOrderViewModelBase
    {
        #region Constructor
        public AddOrderViewModel(IDialogService dialogService) : base(dialogService)
        {
            OrderItems = new ObservableCollection<OrderItem>();
        }
        #endregion

        #region Properties
        public override bool CanSubmit => OrderItems.Count > 0;
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
        /// Submits the current order and resets the form.
        /// </summary>
        internal override void SubmitOrder()
        {
            // Changes order state
            Order.Submit();
            // MainWindow grabs this.Order, adds it to the DB and updates OrdersList
            base.OnOrderSubmitted(Order);
            // Clears this.OrderItems
            OrderItems.Clear();
            // Set this.Order to null
            Order = null;
        }

        /// <summary>
        /// Checks whether the passed OrderItem exists or not in the Order and calls the corresponding method accordingly.
        /// </summary>
        /// <remarks>It is called in ChildWindowViewModel</remarks>
        /// <param name="newItem"></param>
        internal override void CheckNewOrExistingItem(OrderItem newItem)
        {
            OrderItem existingItem = OrderItems
                .FirstOrDefault(item => item.StockItemId == newItem.StockItemId);

            if (existingItem == null)
            {
                AddNewOrderItem(newItem);
            }
            else
            {
                UpdateExistingOrderItem(newItem, existingItem);
            }
        }

        /// <summary>
        /// Adds a new OrderItem to the Order.
        /// </summary>
        /// <param name="newItem"></param>
        internal override void AddNewOrderItem(OrderItem newItem)
        {
            OrderItems.Add(newItem);
            Order.AddItem(newItem);
            RaisePropertyChanged(nameof(Order));
            SubmitOrderCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Updates the Quantity of an existing OrderItem.
        /// </summary>
        /// <param name="newItem">The new item</param>
        /// <param name="existingItem">An item already in the order with the same id as the newItem</param>
        internal override void UpdateExistingOrderItem(OrderItem newItem, OrderItem existingItem)
        {
            existingItem.Quantity += newItem.Quantity;
            // Sincronize items on back order
            existingItem.OnBackOrder += newItem.OnBackOrder;
            RaisePropertyChanged(nameof(Order)); ;
        }

        /// <summary>
        /// Removes the passed OrderItem from the new Order.
        /// </summary>
        /// <remarks>The item is passed through binding from DataGrid as SelectedItem</remarks>
        /// <param name="item"></param>
        internal override void RemoveItem(OrderItem item)
        {
            OrderItems.Remove(item);
            Order.RemoveItem(item.StockItemId);
            RaisePropertyChanged(nameof(Order));
            SubmitOrderCommand.RaiseCanExecuteChanged();
            base.OnOrderItemRemoved(item);
        }

        /// <summary>
        /// Cancels the current order. Opens a dialog to confirm cancelation.
        /// </summary>
        /// <remarks>
        /// Reclaims the generated OrderId, clears the OrderItem Lis and updates the stocks
        /// </remarks>
        internal override void CancelOperation()
        {
            // if order has items
            if (CanSubmit)
            {
                string title = $"Cancel order: {Order.Id}";
                string message = "This order and all its data will be permanently deleted!";
                bool result = ConfirmCancel(title, message);
                if (result)
                {
                    // Return Items to stock
                    ReturnItemsToStock();
                    RefreshTempVars();
                }
                else
                    return;
            }
            base.OnOperationCancelled(Order.Id);
            Order = null;
        }

        /// <summary>
        /// Returns the OrderItems back to the stock.
        /// </summary>
        internal void ReturnItemsToStock()
        {
            foreach (OrderItem item in OrderItems)
            {
                base.OnOrderItemRemoved(item);
            }
        }

        /// <summary>
        /// Clears the collection of OrderItems for the next new order.
        /// </summary>
        private void RefreshTempVars()
        {
            OrderItems.Clear();
            SubmitOrderCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}
