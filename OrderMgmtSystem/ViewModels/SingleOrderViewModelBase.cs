using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.CommonEventArgs;
using OrderMgmtSystem.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace OrderMgmtSystem.ViewModels
{
    public abstract class SingleOrderViewModelBase : ViewModelBase, IHandleOneOrder
    {
        public SingleOrderViewModelBase()
        {
            RemoveItemCommand = new DelegateCommand<OrderItem>(RemoveItem, (SelectedItem) => _selectedItem != null);
            SubmitOrderCommand = new DelegateCommand(SubmitOrder, () => CanSubmit);
            CancelOperationCommand = new DelegateCommand(CancelOperation);
        }

        private readonly IDialogService _dialogService;

        private Order _order;
        private OrderItem _selectedItem;

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
        public DelegateCommand CancelOperationCommand { get; private set; }
        public bool CanSubmit => OrderItems.Count > 0;

        public event EventHandler<OrderItemRemovedEventArgs> OrderItemRemoved;
        public event EventHandler<Order> OrderSubmitted;
        public event EventHandler OrderCancelled;

        internal void AddOrderItem(OrderItem newItem)
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
        protected abstract void RemoveItem(OrderItem item);
        protected abstract void SubmitOrder();
        protected abstract void CancelOperation();
        protected virtual void OnOrderItemRemoved(OrderItemRemovedEventArgs e)
        {
            OrderItemRemoved?.Invoke(this, e);
        }
        protected virtual void OnOrderSubmitted(Order order)
        {
            OrderSubmitted?.Invoke(this, order);
        }
        protected virtual void OnOrderCancelled(EventArgs e)
        {
            OrderCancelled?.Invoke(this, e);
        }
    }
}