using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.CommonEventArgs;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System;
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
            _tempOrder = new Order(order);
            TempOrderItems = new ObservableCollection<OrderItem>(_tempOrder.OrderItems);
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
        public ObservableCollection<OrderItem> TempOrderItems { get; set; }
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
            Order.OrderItems = TempOrder.OrderItems;
            Order.DateTime = TempOrder.DateTime;
            OnOrderUpdated(EventArgs.Empty);
        }


        /// <summary>
        /// Adds the the passed OrderItem to the temporary order.
        /// </summary>
        /// <param name="newItem"></param>
        internal override void AddOrderItem(OrderItem newItem)
        {
            OrderItem repItem = TempOrderItems
                .FirstOrDefault(item => item.StockItemId == newItem.StockItemId);
            if (repItem == null)
            {
                newItem.OrderHeaderId = TempOrder.Id;
                TempOrderItems.Add(newItem);
                TempOrder.AddItem(newItem);
                RaisePropertyChanged(nameof(TempOrder));
                SubmitOrderCommand.RaiseCanExecuteChanged();
            }
            else
            {
                // Notify bindings
                repItem.Quantity += newItem.Quantity;
                // Sincronize items on back order
                repItem.OnBackOrder += newItem.OnBackOrder;
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
            TempOrderItems.Remove(item);
            TempOrder.RemoveItem(item.StockItemId);
            RaisePropertyChanged(nameof(TempOrder));
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
        /// TODO
        /// </summary>
        protected override void CancelOperation()
        {
            throw new NotImplementedException();
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