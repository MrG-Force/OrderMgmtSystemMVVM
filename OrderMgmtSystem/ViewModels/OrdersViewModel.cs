using DataModels;
using DataProvider;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System.Collections.ObjectModel;

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

        public void DeleteOrder()
        {
            Orders.Remove(SelectedOrder);
        }
    }
}
