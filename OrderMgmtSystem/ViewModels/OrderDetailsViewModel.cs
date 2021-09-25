using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.Services.Windows;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System;

namespace OrderMgmtSystem.ViewModels
{
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
            _dialogservice = new DialogService();
        }
        #endregion

        #region Fields
        private readonly IDialogService _dialogservice;
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
        /// <subscribers>MainWindowViewMoedl</subscribers>
        public event EventHandler DeleteOrderRequested;
        #endregion

        #region Methods
        private void ProcessOrder()
        {
            if (Order.HasItemsOnBackOrder)
            {
                Order.OrderStateId = 3;
                // TODO: Inform that the order has been rejected
                CloseWindow();
            }
            else
            {
                Order.OrderStateId = 4;
                // TODO: Inform order has been completed
                CloseWindow();
            }
        }

        private void EditOrder()
        {
            OnEditOrderRequested(EventArgs.Empty);
        }

        private void OnEditOrderRequested(EventArgs e)
        {
            EditOrderRequested?.Invoke(this, e);
        }

        private void DeleteOrder()
        {
            // TODO: Add Dialog to confirm deletion
            OnDeleteOrderRequested(EventArgs.Empty);
            CloseWindow();
        }

        private void OnDeleteOrderRequested(EventArgs e)
        {
            DeleteOrderRequested?.Invoke(this, e);
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
