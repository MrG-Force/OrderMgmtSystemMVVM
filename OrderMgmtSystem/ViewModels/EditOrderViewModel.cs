using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.CommonEventArgs;
using OrderMgmtSystem.Factories;
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
        public EditOrderViewModel(Order order) : base()
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
        public decimal InitialTotal { get; set; }
        public string Title { get; }
        public Order TempOrder
        {
            get => _tempOrder;
            set
            {
                _tempOrder = value;
                RaisePropertyChanged();
            }
        }
        public List<OrderItem> AddedOrderItems { get; set; }
        public List<OrderItem> RemovedOrderItems { get; set; }

        public override bool CanSubmit => InitialTotal != TempOrder.Total;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the user requests the order to be updated.
        /// </summary>
        /// <subscribers>ChildWIndowViewModel</subscribers>
        public event EventHandler OrderUpdated;
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
        protected override void SubmitOrder()
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

        internal override void AddNewOrderItem(OrderItem newItem)
        {
            newItem.OrderHeaderId = TempOrder.Id;
            OrderItems.Add(newItem);
            AddedOrderItems.Add(newItem);
            TempOrder.AddItem(newItem);
        }

        internal override void UpdateExistingOrderItem(OrderItem item, OrderItem existingItem)
        {
            AddedOrderItems.Add(new OrderItem(item));

            item.Quantity += existingItem.Quantity;
            item.OnBackOrder += existingItem.OnBackOrder;
            int i = OrderItems.IndexOf(existingItem);
            _ = OrderItems.Remove(existingItem);
            OrderItems.Insert(i, item);
            TempOrder.OrderItems = new List<OrderItem>(OrderItems);
            TempOrder.HasItemsOnBackOrder = item.HasItemsOnBackOrder;
        }

        /// <summary>
        /// Removes the passed OrderItem from the temporary Order.
        /// </summary>
        /// <param name="item"></param>
        internal override void RemoveItem(OrderItem item)
        {
            OrderItems.Remove(item);
            TempOrder.RemoveItem(item.StockItemId);
            RemovedOrderItems.Add(item);
            RaisePropertyChanged(nameof(TempOrder));
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
        /// Cancel the changes made in the EditOrder view and return the corresponding stockItems
        /// to the inventory.
        /// </summary>
        protected override void CancelOperation()
        {
            // if order has changed
            if (CanSubmit)
            {
                bool result = ConfirmCancelOrder();
                if (result)
                {
                    ReturnItemsToStock(); //Has to work both ways it only returns the items to stock but doesnt work when the update was an item removed
                    ReturnItemsToOrder();
                    RefreshTempVars();
                }
                else
                    return;
            }
            base.OnOperationCancelled(Order.Id);
        }

        // TODO: Maybe we need to keep two lists, one for the added items and one for the removed items. If the order changes are
        // canceled then we return the items added back to the stock and take back the items removed. Thats the best  I have right now.


        /// <summary>
        /// Gets a confirmation to cancel the changes in the current order.
        /// </summary>
        /// <returns>User choice</returns>
        private bool ConfirmCancelOrder()
        {
            bool result;
            var dialogVM = (CancelOrderDialogViewModel)ViewModelFactory
                     .CreateDialogViewModel("CancelOrderDialog",
                     $"Cancel changes in order: {Order.Id}",
                     "All the changes will be reverted!");

            result = _dialogService.OpenDialog(dialogVM);
            return result;
        }

        /// <summary>
        /// Returns the Added items in the order back to the stock.
        /// </summary>
        private void ReturnItemsToStock()
        {
            foreach (OrderItem item in AddedOrderItems)
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

        private void ReturnItemsToOrder()
        {
            foreach (OrderItem item in RemovedOrderItems)
            {
                var itemData = new OrderItemRemovedEventArgs()
                {
                    OrderHeaderId = item.OrderHeaderId,
                    StockItemId = item.StockItemId,
                    Quantity = item.Quantity * -1,
                    OnBackOrder = item.OnBackOrder
                };
                // Use the eventhandler to updates the stock quantities
                base.OnOrderItemRemoved(itemData);
            }
        }

        /// <summary>
        /// Refreshes the EditOrderVM temporary variables in case the user returns to edit this order.
        /// </summary>
        private void RefreshTempVars()
        {
            OrderItems = new ObservableCollection<OrderItem>(Order.OrderItems);
            TempOrder = new Order(Order);
            AddedOrderItems.Clear();
            SubmitOrderCommand.RaiseCanExecuteChanged();
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