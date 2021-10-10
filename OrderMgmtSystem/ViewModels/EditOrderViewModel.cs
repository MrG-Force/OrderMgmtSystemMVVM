using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.CommonEventArgs;
using OrderMgmtSystem.Factories;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using OrderMgmtSystem.ViewModels.DialogViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OrderMgmtSystem.ViewModels
{
    public class EditOrderViewModel : SingleOrderViewModelBase
    {
        #region Constructor
        public EditOrderViewModel(Order order, DialogService dialogService) : base(dialogService)
        {
            Title = $"Editing order number: {order.Id}";
            Order = order;
            OrderItems = new ObservableCollection<OrderItem>(Order.OrderItems);
            _tempOrder = new Order(order);
            AddedOrderItems = new List<OrderItem>();
            RemovedOrderItems = new List<OrderItem>();
            InitialTotal = Order.Total;
        }
        #endregion

        #region Fields
        private Order _tempOrder;
        #endregion

        #region Properties
        /// <summary>
        /// The total of the order before editing.
        /// </summary>
        /// <remarks>This property is used to detect changes in the order</remarks>
        public decimal InitialTotal { get; set; }

        /// <summary>
        /// The title of the window that has this viewModel as DataContext.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// A temporary order that is commited when the order is updated or discarded if the
        /// operation is cancelled.
        /// </summary>
        public Order TempOrder
        {
            get => _tempOrder;
            set
            {
                _tempOrder = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Keeps track of OrderItems that have been added while editing the order. 
        /// </summary>
        public List<OrderItem> AddedOrderItems { get; set; }

        /// <summary>
        /// Keeps track of OrderItems that have been removed while editing the order.
        /// </summary>
        public List<OrderItem> RemovedOrderItems { get; set; }

        /// <summary>
        /// Enables or disables the base.SubmitOrderCommand
        /// </summary>
        public override bool CanSubmit => InitialTotal != TempOrder.Total;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the user requests the order to be updated.
        /// </summary>
        /// <subscribers>ChildWIndowViewModel</subscribers>
        public event EventHandler OrderUpdated;

        /// <summary>
        /// Occurs when the user cancel the edit order process.
        /// </summary>
        public event EventHandler<EventArgs> OrderItemsUpdateReverted;
        #endregion

        #region Methods
        /// <summary>
        /// Publishes the OrderUpdated event.
        /// </summary>
        /// <param name="e"></param>
        private void OnOrderUpdated(EventArgs e)
        {
            OrderUpdated?.Invoke(this, e);
        }

        /// <summary>
        /// Commits the changes to the Order and calls the OrderUpdated event publisher.
        /// </summary>
        internal override void SubmitOrder()
        {
            Order.OrderItems = this.OrderItems.ToList();
            Order.DateTime = TempOrder.DateTime;
            Order.HasItemsOnBackOrder = TempOrder.HasItemsOnBackOrder;
            OnOrderUpdated(EventArgs.Empty);
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
            RaisePropertyChanged(nameof(TempOrder));
            SubmitOrderCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Adds a new OrderItem to the Order.
        /// </summary>
        /// <param name="newItem"></param>
        internal override void AddNewOrderItem(OrderItem newItem)
        {
            AddedOrderItems.Add(new OrderItem(newItem, Order.Id));

            OrderItems.Add(newItem);
            TempOrder.AddItem(newItem);

        }

        /// <summary>
        /// Updates the Quantity of an existing OrderItem.
        /// </summary>
        /// <param name="newItem">The new item</param>
        /// <param name="existingItem">An item already in the order with the same id as the newItem</param>
        internal override void UpdateExistingOrderItem(OrderItem newItem, OrderItem existingItem)
        {
            AddedOrderItems.Add(new OrderItem(newItem, Order.Id));

            newItem.Quantity += existingItem.Quantity;
            newItem.OnBackOrder += existingItem.OnBackOrder;
            int i = OrderItems.IndexOf(existingItem);
            _ = OrderItems.Remove(existingItem);
            OrderItems.Insert(i, newItem);
            TempOrder.OrderItems = new List<OrderItem>(OrderItems);
            TempOrder.HasItemsOnBackOrder = newItem.HasItemsOnBackOrder;
        }

        /// <summary>
        /// Removes the passed OrderItem from the temporary Order.
        /// </summary>
        /// <param name="item"></param>
        internal override void RemoveItem(OrderItem item)
        {
            RemovedOrderItems.Add(item); // Keep track of removed item

            OrderItems.Remove(item); // Remove item from view
            TempOrder.RemoveItem(item.StockItemId);// Remove item for the editing temp order

            RaisePropertyChanged(nameof(TempOrder));
            SubmitOrderCommand.RaiseCanExecuteChanged();
            base.OnOrderItemRemoved(item); // this one returns the item to the stock list
        }

        /// <summary>
        /// Cancel the changes made in the EditOrder view and return the corresponding stockItems
        /// to the inventory.
        /// </summary>
        internal override void CancelOperation()
        {
            // if order has changed
            if (CanSubmit)
            {
                string title = $"Cancel changes in order: {Order.Id}";
                string message = "All the changes will be reverted!";
                bool result = ConfirmCancel(title, message);
                if (result)
                {
                    ReturnItemsToStock();
                    OnOrderItemsUpdateReverted(EventArgs.Empty);
                }
                else
                    return;
            }
            // navigate back to OrderDetails
            base.OnOperationCancelled(Order.Id);
        }

        /// <summary>
        /// Returns the Added items in the order back to the stock.
        /// </summary>
        private void ReturnItemsToStock()
        {
            foreach (OrderItem item in AddedOrderItems)
            {
                base.OnOrderItemRemoved(item);
            }
        }

        /// <summary>
        /// Raises the OrderItemsUpdateReverted event.
        /// </summary>
        /// <param name="e"></param>
        private void OnOrderItemsUpdateReverted(EventArgs e)
        {
            OrderItemsUpdateReverted?.Invoke(this, e);
        }

        /// <summary>
        /// This metod is used to refresh the command so the command is no
        /// exectuted when the same order is opened more than once for editing.
        /// </summary>
        public void RefreshCanSubmit()
        {
            SubmitOrderCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}