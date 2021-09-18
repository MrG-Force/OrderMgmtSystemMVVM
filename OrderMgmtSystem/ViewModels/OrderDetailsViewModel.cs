using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using System.Collections.Generic;

namespace OrderMgmtSystem.ViewModels
{
    public class OrderDetailsViewModel : ViewModelBase
    {
        private Order _order;
        private readonly IDialogService _dialogservice;

        public Order Order { get => _order; set => _order = value; }
        public List<OrderItem> OrderItems { get; private set; }
        public string Title { get; }
        public OrderDetailsViewModel(Order order)
        {
            Order = order;
            Title = $"Order number: {order.Id}";
            _dialogservice = new DialogService();
        }
    }
}
