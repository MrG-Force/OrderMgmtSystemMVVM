using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.Services.Windows;
using System;
using System.Collections.Generic;

namespace OrderMgmtSystem.ViewModels
{
    public class OrderDetailsViewModel : ViewModelBase, ICloseWindows
    {
        private readonly IDialogService _dialogservice;

        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; private set; }
        public string Title { get; }
        public Action Close { get; set; }
        public DelegateCommand CloseWindowCommand { get; private set; }

        public OrderDetailsViewModel(Order order)
        {
            Order = order;
            Title = $"Order number: {order.Id}";
            CloseWindowCommand = new DelegateCommand(CloseWindow);
            _dialogservice = new DialogService();
        }

        /// <summary>
        /// Invokes the Close delegate.
        /// </summary>
        private void CloseWindow()
        {
            Close?.Invoke();
        }
    }
}
