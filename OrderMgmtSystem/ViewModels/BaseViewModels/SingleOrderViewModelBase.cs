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
        /// Occurs when an OrderItem is added to the current order.
        /// </summary>
        /// <subscribers>MainWindowViewModel, ChildWindowViewModel</subscribers>
        public event EventHandler<OrderItemAddedEventArgs> OrderItemAdded;

        /// <summary>
        /// Occurs when an OrderItem is removed from the order
        /// </summary>
        /// /// <subscribers>MainWindowViewModel, ChildWindowViewModel</subscribers>
        public event EventHandler<OrderItem> OrderItemRemoved;

        /// <summary>
        /// Occurs when the Order is submitted.
        /// </summary>
        /// /// <subscribers>MainWindowViewModel</subscribers>
        public event EventHandler<Order> OrderSubmitted;

        /// <summary>
        /// Occurs when the current Order is cancelled.
        /// </summary>
        /// /// <subscribers>MainWindowViewModel, ChildWindowViewModel</subscribers>
        public event EventHandler<int> OperationCancelled;
        #endregion

        #region Methods
        /// <summary>
        /// Adds an OrderItem to the Order.
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

        /// <summary>
        /// Raises the OrderItemAdded event.
        /// </summary>
        /// <param name="e">An eventArgs obj that contains the OrderItem and a bool</param>
        protected virtual void OnOrderItemAdded(OrderItemAddedEventArgs e)
        {
            OrderItemAdded?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the OrderItemRemoved event.
        /// </summary>
        /// <param name="e">An eventArgs obj that contains data to update the StockItems</param>
        protected virtual void OnOrderItemRemoved(OrderItem orderItem)
        {
            OrderItemRemoved?.Invoke(this, orderItem);
        }
        /// <summary>
        /// Raises the OrderSubmitted event.
        /// </summary>
        /// <param name="order">The submitted Order</param>
        protected virtual void OnOrderSubmitted(Order order)
        {
            OrderSubmitted?.Invoke(this, order);
        }
        /// <summary>
        /// Raises the OperationCancelled event passing the Order id so the unfinished order
        /// can be deleted.
        /// </summary>
        /// <param name="orderId"></param>
        protected virtual void OnOperationCancelled(int orderId)
        {
            OperationCancelled?.Invoke(this, orderId);
        }
        #endregion
    }
}