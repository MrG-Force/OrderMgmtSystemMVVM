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
        #region Constructor
        public OrdersViewModel(IOrdersDataProvider ordersData)
        {
            _ordersData = ordersData;
            Orders = new ObservableCollection<Order>(_ordersData.Orders);
        }
        #endregion

        #region Fields
        private readonly IOrdersDataProvider _ordersData;
        private Order _selectedOrder;
        #endregion

        #region Properties
        public ObservableCollection<Order> Orders { get; }
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set => _selectedOrder = value;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Deletes the selected order.
        /// </summary>
        public void DeleteOrder()
        {
            Orders.Remove(SelectedOrder);
        }
        #endregion
    }
}
