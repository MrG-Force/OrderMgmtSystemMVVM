using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Factories;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.ViewModels.DialogViewModels;
using System;
using System.Collections.ObjectModel;

namespace OrderMgmtSystem.ViewModels.BaseViewModels
{
    public abstract class SingleOrderViewModelBase : ViewModelBase, IHandleOneOrder
    {
        #region Constructor
        public SingleOrderViewModelBase()
        {
            RemoveItemCommand = new DelegateCommand<OrderItem>(RemoveItem, (SelectedItem) => SelectedItem != null);
            CancelOperationCommand = new DelegateCommand(CancelOperation);
            SubmitOrderCommand = new DelegateCommand(SubmitOrder, () => CanSubmit);
            _dialogService = new DialogService();
        }
        #endregion

        #region Fields
        protected readonly IDialogService _dialogService;
        protected Order _order;
        private OrderItem _selectedItem;
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
        internal abstract void CheckNewOrExistingItem(OrderItem newItem);
        internal abstract void AddNewOrderItem(OrderItem newItem);
        internal abstract void UpdateExistingOrderItem(OrderItem item, OrderItem existingItem);
        internal abstract void RemoveItem(OrderItem item);
        protected abstract void SubmitOrder();
        protected abstract void CancelOperation();

        /// <summary>
        /// Gets confirmation to proceed with cancelation
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool ConfirmCancel(string title, string message)
        {
            bool result;
            var dialogVM = (CancelOrderDialogViewModel)ViewModelFactory
                     .CreateDialogViewModel("CancelOrderDialog", title, message);

            result = _dialogService.OpenDialog(dialogVM);
            return result;
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