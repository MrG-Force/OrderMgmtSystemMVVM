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
            SubmitOrderCommand = new DelegateCommand(SubmitOrder, () => CanSubmit);
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
        internal override void AddOrderItem(OrderItem newItem)
        {
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
            }
        }

        /// <summary>
        /// Removes the passed OrderItem from the new Order.
        /// </summary>
        /// <param name="newItem"></param>
        internal override void RemoveItem(OrderItem item)
        {
            OrderItems.Remove(item);
            Order.RemoveItem(item.StockItemId);
            RaisePropertyChanged(nameof(Order));
            var itemData = new OrderItemRemovedEventArgs()
            {
                StockItemId = item.StockItemId,
                Quantity = item.Quantity,
                OnBackOrder = item.OnBackOrder
            };
            base.OnOrderItemRemoved(itemData);
            SubmitOrderCommand.RaiseCanExecuteChanged();
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
            bool result = true;
            // if order has items
            if (CanSubmit)
            {
                string message = "This order and all its data will be permanently deleted!";
                string title = $"Cancel order: {Order.Id}";
                var dialogVM = (CancelOrderDialogViewModel)ViewModelFactory
                    .CreateDialogViewModel("CancelOrderDialog", title, message);
                result = _dialogService.OpenDialog(dialogVM);
            }
            if (result)
            {
                //Order.CancelLastOrder(); //---Remove if not using random data
                // Return Items to stock
                foreach (OrderItem item in OrderItems)
                {
                    var itemData = new OrderItemRemovedEventArgs()
                    {
                        StockItemId = item.StockItemId,
                        Quantity = item.Quantity,
                        OnBackOrder = item.OnBackOrder
                    };
                    // Use the eventhandler to updates the stock quantities
                    base.OnOrderItemRemoved(itemData);
                }
                OrderItems.Clear();
                SubmitOrderCommand.RaiseCanExecuteChanged();
                base.OnOperationCancelled(Order.Id);
                Order = null;
            }
        }
        #endregion
    }
}
