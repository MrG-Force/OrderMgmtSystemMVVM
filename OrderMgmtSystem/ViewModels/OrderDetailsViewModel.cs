using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.Services.Windows;
using System;
using System.Collections.Generic;

namespace OrderMgmtSystem.ViewModels
{
    public class OrderDetailsViewModel : ViewModelBase, ICloseWindows, IHandleOneOrder
    {
        private readonly IDialogService _dialogservice;

        public AddItemViewModel AddItemViewModel { get; private set; }
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; private set; }
        public string Title { get; }
        public DelegateCommand CloseWindowCommand { get; private set; }
        public DelegateCommand ProcessOrderCommand { get; private set; }
        public DelegateCommand EditOrderCommand { get; private set; }
        public DelegateCommand DeleteOrderCommand { get; private set; }
        public bool CanProcessOrEdit { get => Order.OrderStateId == 2; }
        public bool CanDelete { get => Order.OrderStateId == 2 || Order.OrderStateId == 3; }
        public bool IsModalOpen { get => false; }
        public Action Close { get; set; }

        /// <summary>
        /// ChildWindowService subscribes to this event to switch data context in ChildWindow.
        /// </summary>
        public event Action<Order> EditOrderRequested = delegate { };
        public event Action<int> DeleteOrderRequested = delegate { };
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
            OnEditOrderRequested(Order);
        }

        private void OnEditOrderRequested(Order order)
        {
            EditOrderRequested(order);
        }

        private void DeleteOrder()
        {
            // TODO: Add DIalog to confirm deletion
            OnDeleteOrderRequested(Order.Id);
            CloseWindow();
        }

        private void OnDeleteOrderRequested(int id)
        {
            DeleteOrderRequested(id);
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
