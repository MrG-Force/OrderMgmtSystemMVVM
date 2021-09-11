using DataModels;
using DataProvider;
using OrderMgmtSystem.Commands;
using System;

namespace OrderMgmtSystem.ViewModels
{
    /// <summary>
    /// A class that contains the logic for the application's MainWindow.
    /// </summary>
    /// <remarks>This class handles the main navigation and communication between views</remarks>
    internal class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private readonly IOrdersDataProvider _Data;

        private ViewModelBase _CurrentViewModel;

        private AddItemViewModel _addItemViewModel;
        private readonly AddOrderViewModel _addOrderViewModel = new AddOrderViewModel();
        private readonly OrdersViewModel _ordersViewModel;
        private OrderDetailsViewModel _orderDetailsViewModel;

        private bool _IsModalOpen;
        #endregion

        #region Constructor
        /// <summary>
        /// Initialize the MainWindowViewModel.
        /// </summary>
        /// <param name="ordersData"></param>
        public MainWindowViewModel(IOrdersDataProvider ordersData)
        {
            _Data = ordersData;
            _IsModalOpen = false;
            // ViewModels
            _CurrentViewModel = new OrdersViewModel(ordersData);
            _ordersViewModel = (OrdersViewModel)_CurrentViewModel;
            _orderDetailsViewModel = new OrderDetailsViewModel();
            _addItemViewModel = new AddItemViewModel(ordersData.StockItems);
            // Event handlers
            _addOrderViewModel.OrderSubmitted += SubmitOrderToDB;
            _addOrderViewModel.OrderCancelled += OnOrderCancelled;
            _addItemViewModel.OrderItemSelected += AddItemToNewOrder;
            _addOrderViewModel.OrderItemRemoved += UpdateItemStock;
            // Commands
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }
        #endregion

        #region Properties
        public DelegateCommand<string> NavigateCommand { get; private set; }

        public ViewModelBase CurrentViewModel
        {
            get => _CurrentViewModel;
            set => SetProperty(ref _CurrentViewModel, value);
        }

        public AddItemViewModel ModalViewModel
        {
            get => _addItemViewModel;
            set => SetProperty(ref _addItemViewModel, value);
        }

        public bool IsModalOpen
        {
            get => _IsModalOpen;
            set => SetProperty(ref _IsModalOpen, value);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Handles the OrderSubmitted event by adding new orders to the 
        /// orders list and navigating back to the ordersView.
        /// </summary>
        /// <param name="order">The new order to add</param>
        private void SubmitOrderToDB(Order order)
        {
            // Submit order to DB logic goes here
            _ordersViewModel.Orders.Add(order);
            Navigate("OrdersView");
        }

        /// <summary>
        /// Handles the OrderCancelled event by navigating back to the OrdersView.
        /// </summary>
        private void OnOrderCancelled()
        {
            Navigate("OrdersView");
        }

        /// <summary>
        /// Handles the OrderItemSelected event by calling the corresponding method 
        /// in the AddOrderViewModel to add the received item to the order. Closes the
        /// (modal) AddItemView.
        /// </summary>
        /// <param name="newItem">The item to add</param>
        private void AddItemToNewOrder(OrderItem newItem)
        {
            _addOrderViewModel.AddOrderItem(newItem);
            Navigate("CloseAddItem");
        }

        /// <summary>
        /// Handles the OrderItemRemoved event by re-stocking the item in the StockItems list.
        /// </summary>
        /// <param name="stockItemId"></param>
        /// <param name="quantity">The number of items(Quantity) removed from the order.</param>
        /// <param name="onBackOrder">The number of items that were unavailable when the order was placed.</param>
        private void UpdateItemStock(int stockItemId, int quantity, int onBackOrder)
        {
            _addItemViewModel.ReturnItemToStockList(stockItemId, quantity, onBackOrder);
        }

        /// <summary>
        /// Handles all the navigation between views.
        /// </summary>
        /// <remarks>When needed the function also helps to call functions to initialize views</remarks>
        /// <param name="destination"></param>
        private void Navigate(string destination)
        {
            switch (destination)
            {
                case "AddOrderView":
                    if (_addOrderViewModel.Order == null)
                    {
                        _addOrderViewModel.LoadNewOrder(GetNewOrder());
                    }
                    CurrentViewModel = _addOrderViewModel;
                    break;
                case "AddItemView":
                    IsModalOpen = true;
                    break;
                case "CloseAddItem":
                    IsModalOpen = false;
                    break;
                case "CancelAndBackToOrders":
                    CurrentViewModel = _ordersViewModel;
                    _addOrderViewModel.CancelCurrentOrder();
                    break;
                case "OrdersView":
                default:
                    CurrentViewModel = _ordersViewModel;
                    break;
            }
        }

        /// <summary>
        /// Gets an empty order with the corresponding Id from the data provider.
        /// </summary>
        /// <returns></returns>
        private Order GetNewOrder()
        {
            return _Data.GetOrder();
        }
        #endregion
    }
}
