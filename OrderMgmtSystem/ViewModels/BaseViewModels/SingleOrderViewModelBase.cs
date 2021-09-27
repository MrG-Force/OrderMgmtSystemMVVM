using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.CommonEventArgs;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using System;
using System.Collections.ObjectModel;

namespace OrderMgmtSystem.ViewModels.BaseViewModels
{
    public abstract class SingleOrderViewModelBase : ViewModelBase, IHandleOneOrder
    {
        #region Constructor
        public SingleOrderViewModelBase()
        {
            RemoveItemCommand = new DelegateCommand<OrderItem>(RemoveItem, (SelectedItem) => _selectedItem != null);
            CancelOperationCommand = new DelegateCommand(CancelOperation);
            _dialogService = new DialogService();
        }
        #endregion

        #region Fields
        protected readonly IDialogService _dialogService;

        protected Order _order;
        private OrderItem _selectedItem;
        // Get a VMFactory field to create dialog view models
        #endregion

        #region Properties
        public Order Order
        {
            get => _order;
            set
            {
                _order = value;
                RaisePropertyChanged();
            }
        }
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
        public DelegateCommand SubmitOrderCommand { get; protected set; }
        public DelegateCommand CancelOperationCommand { get; private set; }
        public abstract bool CanSubmit { get; }
        #endregion

        #region Events
        /// <summary>
        /// Happens when an item is removed from the order
        /// </summary>
        public event EventHandler<OrderItemRemovedEventArgs> OrderItemRemoved;
        /// <summary>
        /// Happens when the order is submitted.
        /// </summary>
        public event EventHandler<Order> OrderSubmitted;
        /// <summary>
        /// Happens when the current order is cancelled.
        /// </summary>
        public event EventHandler OperationCancelled;
        #endregion

        #region Methods
        /// <summary>
        /// Adds an OrderItem to the order.
        /// </summary>
        /// <remarks>It is used in conjunction with the NavigateCommand in the MainWindowModel.</remarks>
        /// <param name="newItem"></param>
        internal abstract void AddOrderItem(OrderItem newItem);

        /// <summary>
        /// Removes the passed item from the Order and calls an event handler
        /// to update the quantity of the items in the StockItems list.
        /// </summary>
        /// <param name="item"></param>
        internal abstract void RemoveItem(OrderItem item);
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
        protected virtual void OnOperationCancelled(EventArgs e)
        {
            OperationCancelled?.Invoke(this, e);
        }
        #endregion
    }
}