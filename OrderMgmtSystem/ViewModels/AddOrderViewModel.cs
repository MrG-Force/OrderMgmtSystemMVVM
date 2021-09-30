using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.CommonEventArgs;
using OrderMgmtSystem.Factories;
using OrderMgmtSystem.ViewModels.BaseViewModels;
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
    public class AddOrderViewModel : SingleOrderViewModelBase
    {
        #region Constructor
        public AddOrderViewModel() : base()
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
        /// Adds the passed OrderItem to the new Order.
        /// </summary>
        /// <param name="newItem"></param>
        internal override void CheckNewOrExistingItem(OrderItem newItem)
        {
            // prepare event data that includes the orderItem and bool orderItemExists
            newItem.OrderHeaderId = Order.Id;
            var eventData = new OrderItemAddedEventArgs() { Item = newItem };

            OrderItem existingItem = OrderItems
                .FirstOrDefault(item => item.StockItemId == newItem.StockItemId);

            if (existingItem == null)
            {
                AddNewOrderItem(newItem);
                eventData.OrderItemExists = false;
            }
            else
            {
                UpdateExistingOrderItem(newItem, existingItem);
                eventData.OrderItemExists = true;
            }
            base.OnOrderItemAdded(eventData);
        }

        internal override void AddNewOrderItem(OrderItem newItem)
        {
            OrderItems.Add(newItem);
            Order.AddItem(newItem);
            RaisePropertyChanged(nameof(Order));
            SubmitOrderCommand.RaiseCanExecuteChanged();
        }

        internal override void UpdateExistingOrderItem(OrderItem item, OrderItem existingItem)
        {
            existingItem.Quantity += item.Quantity;
            // Sincronize items on back order
            existingItem.OnBackOrder += item.OnBackOrder;
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
            var itemData = new OrderItemRemovedEventArgs()
            {
                OrderHeaderId = item.OrderHeaderId,
                StockItemId = item.StockItemId,
                Quantity = item.Quantity,
                OnBackOrder = item.OnBackOrder
            };
            SubmitOrderCommand.RaiseCanExecuteChanged();
            base.OnOrderItemRemoved(itemData);
        }

        /// <summary>
        /// Submits the current order and resets the form.
        /// </summary>
        protected override void SubmitOrder()
        {
            // Change order state calling Order.Submit()
            Order.Submit();
            // MainWindow grabs this.Order and adds it to the DB and updates OrdersList
            base.OnOrderSubmitted(Order);
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
        protected override void CancelOperation()
        {
            // if order has items
            if (CanSubmit)
            {
                bool result = ConfirmCancelOrder();
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

        private bool ConfirmCancelOrder()
        {
            bool result;
            string message = "This order and all its data will be permanently deleted!";
            string title = $"Cancel order: {Order.Id}";
            var dialogVM = (CancelOrderDialogViewModel)ViewModelFactory
                     .CreateDialogViewModel("CancelOrderDialog", title, message);

            result = _dialogService.OpenDialog(dialogVM);
            return result;
        }

        private void ReturnItemsToStock()
        {
            foreach (OrderItem item in OrderItems)
            {
                var itemData = new OrderItemRemovedEventArgs()
                {
                    OrderHeaderId = item.OrderHeaderId,
                    StockItemId = item.StockItemId,
                    Quantity = item.Quantity,
                    OnBackOrder = item.OnBackOrder
                };
                // Use the eventhandler to updates the stock quantities
                base.OnOrderItemRemoved(itemData);
            }
        }

        private void RefreshTempVars()
        {
            OrderItems.Clear();
            SubmitOrderCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}
