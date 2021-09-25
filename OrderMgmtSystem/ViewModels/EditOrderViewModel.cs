using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.CommonEventArgs;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace OrderMgmtSystem.ViewModels
{
    public class EditOrderViewModel : SingleOrderViewModelBase
    {
        public EditOrderViewModel(Order order) : base()
        {
            Title = $"Editing order number: {order.Id}";
            Order = order;
            _tempOrder = new Order(order);
            TempOrderItems = new ObservableCollection<OrderItem>(_tempOrder.OrderItems);
            SubmitOrderCommand = new DelegateCommand(SubmitOrder, () => CanSubmit);
            InitialTotal = Order.Total;
        }
        private Order _tempOrder;

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

        public event Action OrderUpdated = delegate { };

        protected override void SubmitOrder()
        {
            Order.OrderItems = TempOrder.OrderItems;
            Order.DateTime = TempOrder.DateTime;
            OrderUpdated();
        }

        protected override void CancelOperation()
        {
            throw new NotImplementedException();
        }

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
                // BUG to chase, add check to enable the button when an existing item is added
                // doesn't update
            }
        }

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

        public void RefreshCanSubmit()
        {
            SubmitOrderCommand.RaiseCanExecuteChanged();
        }
    }
}
