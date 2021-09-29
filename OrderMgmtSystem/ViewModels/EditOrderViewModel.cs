using DataModels;
using OrderMgmtSystem.Commands;
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
            TempOrderItems = new List<OrderItem>();
            SubmitOrderCommand = new DelegateCommand(SubmitOrder, () => CanSubmit);
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
        public List<OrderItem> TempOrderItems { get; set; }
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
        /// Adds the the passed OrderItem to the temporary order.
        /// </summary>
        /// <remarks>It is called in ChildWindowViewModel</remarks>
        /// <param name="newItem"></param>
        internal override void AddOrderItem(OrderItem newItem)
        {
            OrderItem repItem = OrderItems
                .FirstOrDefault(item => item.StockItemId == newItem.StockItemId);
            if (repItem == null)
            {
                newItem.OrderHeaderId = TempOrder.Id;
                OrderItems.Add(newItem);
                TempOrderItems.Add(newItem);
                TempOrder.AddItem(newItem);
                RaisePropertyChanged(nameof(TempOrder));
                SubmitOrderCommand.RaiseCanExecuteChanged();
            }
            else
            {
                TempOrderItems.Add(new OrderItem(newItem));
                newItem.Quantity += repItem.Quantity;
                newItem.OnBackOrder += repItem.OnBackOrder;
                int indx = OrderItems.IndexOf(repItem);
                _= OrderItems.Remove(repItem);
                OrderItems.Insert(indx, newItem);
                TempOrder.OrderItems = new List<OrderItem>(OrderItems);
                if (newItem.HasItemsOnBackOrder)
                {
                    TempOrder.HasItemsOnBackOrder = true;
                }
                RaisePropertyChanged(nameof(TempOrder));
                SubmitOrderCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Removes the passed OrderItem from the temporary Order.
        /// </summary>
        /// <param name="item"></param>
        internal override void RemoveItem(OrderItem item)
        {
            OrderItems.Remove(item);
            TempOrder.RemoveItem(item.StockItemId);
            RaisePropertyChanged(nameof(TempOrder));
            SubmitOrderCommand.RaiseCanExecuteChanged();
            base.OnOrderItemRemoved(item);
        }

        /// <summary>
        /// Cancel the changes made in the EditOrder view and return the corresponding stockItems
        /// to the inventory.
        /// </summary>
        protected override void CancelOperation()
        {
            bool result = false;
            // if order has changed
            if (CanSubmit)
            {
                string message = "All the changes will be reverted!";
                string title = $"Cancel changes in order: {Order.Id}";
                var dialogVM = (CancelOrderDialogViewModel)ViewModelFactory
                    .CreateDialogViewModel("CancelOrderDialog", title, message);
                result = _dialogService.OpenDialog(dialogVM);
            }
            if (result)
            {
                // Return Items to stock
                foreach (OrderItem item in TempOrderItems)
                {
                    base.OnOrderItemRemoved(item);
                }
                OrderItems = new ObservableCollection<OrderItem>(Order.OrderItems);
                TempOrder = new Order(Order);
                TempOrderItems.Clear();
                SubmitOrderCommand.RaiseCanExecuteChanged();
            }
            base.OnOperationCancelled(Order.Id);
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