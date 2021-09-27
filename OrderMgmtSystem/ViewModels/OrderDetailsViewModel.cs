using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Factories;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.Services.Windows;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using OrderMgmtSystem.ViewModels.DialogViewModels;
using System;

namespace OrderMgmtSystem.ViewModels
{
    /// <summary>
    /// A class that provides the logic and functionality to the OrderDetailsView that allows the 
    /// user to see the details of an order, delete it, process it or modify it.
    /// </summary>
    public class OrderDetailsViewModel : ViewModelBase, ICloseWindows, IHandleOneOrder
    {
        #region Constructor
        public OrderDetailsViewModel(Order order)
        {
            Order = order;
            Title = $"Order number: {order.Id}";
            CloseWindowCommand = new DelegateCommand(CloseWindow);
            ProcessOrderCommand = new DelegateCommand(ProcessOrder, () => CanProcessOrEdit);
            EditOrderCommand = new DelegateCommand(EditOrder, () => CanProcessOrEdit);
            DeleteOrderCommand = new DelegateCommand(DeleteOrder, () => CanDelete);
            _dialogService = new DialogService();
        }
        #endregion

        #region Fields
        private readonly IDialogService _dialogService;
        #endregion

        #region Properties
        public Order Order { get; set; }
        public string Title { get; }
        public DelegateCommand CloseWindowCommand { get; private set; }
        public DelegateCommand ProcessOrderCommand { get; private set; }
        public DelegateCommand EditOrderCommand { get; private set; }
        public DelegateCommand DeleteOrderCommand { get; private set; }
        public bool CanProcessOrEdit { get => Order.OrderStateId == 2; }
        public bool CanDelete { get => Order.OrderStateId == 2 || Order.OrderStateId == 3; }
        public Action Close { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when an order is set to be edited.
        /// </summary>
        /// <subscribers>ChildWindowViewModel</subscribers>
        public event EventHandler EditOrderRequested;

        /// <summary>
        /// Occurs when an order is deleted.
        /// </summary>
        /// <subscribers>MainWindowViewModel, ChildWindowViewModel</subscribers>
        public event EventHandler DeleteOrderRequested;

        /// <summary>
        /// Occurs when an order is rejected due to srock limitations.
        /// </summary>
        /// <subscribers>ChildWindowViewModel</subscribers>
        public event EventHandler OrderRejected;
        #endregion

        #region Methods
        /// <summary>
        /// Process the current order by changing its state to Complete or Rejected depending
        /// on wether the order has items on back order, will be mark as rejected if true or complete otherwise.
        /// </summary>
        /// <remarks>If the order is rejects all its stock items will be returned to the inventory</remarks>
        private void ProcessOrder()
        {
            if (Order.HasItemsOnBackOrder)
            {
                string title = $"Reject order: {Order.Id}";
                string warning = "Not enough items in stock:";
                string message = "This order will be rejected!";
                var dialogVM = (RejectOrderDialogViewModel)ViewModelFactory
                    .CreateDialogViewModel("RejectOrderDialog",
                    title, message, warning);
                bool result = _dialogService.OpenDialog(dialogVM);

                if (result)
                {
                    Order.OrderStateId = 3;
                    OnOrderRejected(EventArgs.Empty);
                    CloseWindow();
                }
            }
            else
            {
                Order.OrderStateId = 4;
                // TODO: Inform order has been completed
                CloseWindow();
            }
        }

        /// <summary>
        /// Takes the user to the EditOrder view where the current order can be modified.
        /// </summary>
        private void EditOrder()
        {
            OnEditOrderRequested(EventArgs.Empty);
        }

        /// <summary>
        /// Deletes the current order.
        /// </summary>
        /// <remarks>Prompts user to confirm the action</remarks>
        private void DeleteOrder()
        {
            // TODO: Add Dialog to confirm deletion
            string message = "This order and all its data will be permanently deleted. Are you sure?";
            string title = $"Delete order: {Order.Id}?";
            var dialogVM = (CancelOrderDialogViewModel)ViewModelFactory
                    .CreateDialogViewModel("CancelOrderDialog", title, message);
            
            if (_dialogService.OpenDialog(dialogVM))
            {
                OnDeleteOrderRequested(EventArgs.Empty);
                CloseWindow();
            }
        }

        /// <summary>
        /// Raises the EditOrderRequested event.
        /// </summary>
        /// <param name="e"></param>
        private void OnEditOrderRequested(EventArgs e)
        {
            EditOrderRequested?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the DeleteOrderRequested event.
        /// </summary>
        /// <param name="e"></param>
        private void OnDeleteOrderRequested(EventArgs e)
        {
            DeleteOrderRequested?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the OrderRejected event.
        /// </summary>
        /// <param name="e"></param>
        private void OnOrderRejected(EventArgs e)
        {
            OrderRejected?.Invoke(this, e);
        }

        /// <summary>
        /// Invokes the Close delegate. 
        /// </summary>
        /// <remarks>It is wired up in the ChildWindow code behind.</remarks>
        private void CloseWindow()
        {
            Close?.Invoke();
        }
        #endregion



    }
}
