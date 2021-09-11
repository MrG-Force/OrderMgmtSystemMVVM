using DataModels;
using DataProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMgmtSystem.ViewModels
{
    /// <summary>
    /// This class is responsible of the presentation logic of the UI(OrdersView). Provides methods and properties
    /// that can be bound to the UI.
    /// </summary>
    public class OrdersViewModel : ViewModelBase
    {
        private readonly IOrdersDataProvider _OrdersData;
        private Order _SelectedOrder;

        public OrdersViewModel(IOrdersDataProvider ordersData)
        {
            _OrdersData = ordersData;
            Orders = new ObservableCollection<Order>(_OrdersData.Orders);
        }
        public ObservableCollection<Order> Orders { get; }
        public Order SelectedOrder
        {
            get => _SelectedOrder;
            set => _SelectedOrder = value;
        }
    }
}
