using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.Services.Windows;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System;
using System.Collections.Generic;

namespace OrderMgmtSystem.ViewModels
{
    public class OrderDetailsViewModel : ViewModelBase, ICloseWindows, IHandleOneOrder
    {
        public OrderDetailsViewModel(Order order, AddItemViewModel addItemVM)
        {
            Order = order;
            Title = $"Order number: {order.Id}";
            CloseWindowCommand = new DelegateCommand(CloseWindow);
            ProcessOrderCommand = new DelegateCommand(ProcessOrder, () => CanProcessOrEdit);
            EditOrderCommand = new DelegateCommand(EditOrder, () => CanProcessOrEdit);
            DeleteOrderCommand = new DelegateCommand(DeleteOrder, () => CanDelete);
            _dialogservice = new DialogService();
            AddItemViewModel = addItemVM;
        }
        ~OrderDetailsViewModel()
        {

        }

        private readonly IDialogService _dialogservice;

        public AddItemViewModel AddItemViewModel { get; private set; }
        public Order Order { get; set; }
        public string Title { get; }
        public DelegateCommand CloseWindowCommand { get; private set; }
        public DelegateCommand ProcessOrderCommand { get; private set; }
        public DelegateCommand EditOrderCommand { get; private set; }
        public DelegateCommand DeleteOrderCommand { get; private set; }
        public bool CanProcessOrEdit { get => Order.OrderStateId == 2; }
        public bool CanDelete { get => Order.OrderStateId == 2 || Order.OrderStateId == 3; }
        public Action Close { get; set; }

        /// <summary>
        /// ChildWindowService subscribes to this event to switch data context in ChildWindow.
        /// </summary>
        public event Action EditOrderRequested = delegate { };
        public event Action DeleteOrderRequested = delegate { };

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
            OnEditOrderRequested();
        }

        private void OnEditOrderRequested()
        {
            EditOrderRequested();
        }

        private void DeleteOrder()
        {
            // TODO: Add Dialog to confirm deletion
            DeleteOrderRequested();
            CloseWindow();
        }


        /// <summary>
        /// Invokes the Close delegate. 
        /// </summary>
        /// <remarks>It is wired up in the ChildWindow code behind.</remarks>
        private void CloseWindow()
        {
            Close?.Invoke();
        }
    }
}
