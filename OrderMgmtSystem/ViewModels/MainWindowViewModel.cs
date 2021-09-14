﻿using DataModels;
using DataProvider;
using OrderMgmtSystem.Commands;

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
        private ViewModelBase _currentViewModel;
        private AddItemViewModel _addItemViewModel;
        private bool _IsModalOpen;
        #endregion

        #region Properties
        public AddItemViewModel AddItemViewModel
        {
            get => _addItemViewModel;
            set => SetProperty(ref _addItemViewModel, value);
        }
        internal AddOrderViewModel AddOrderViewModel { get; set; }
        internal OrdersViewModel OrdersViewModel { get; set; }
        internal OrderDetailsViewModel OrderDetailsViewModel { get; set; }
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }
        public bool IsModalOpen
        {
            get => _IsModalOpen;
            set => SetProperty(ref _IsModalOpen, value);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the MainWindowModel object using dependency injection.
        /// </summary>
        /// <param name="ordersData"></param>
        /// <param name="currentViewModel"></param>
        /// <param name="addItemViewModel"></param>
        public MainWindowViewModel(IOrdersDataProvider ordersData, ViewModelBase currentViewModel, AddItemViewModel addItemViewModel)
        {
            _Data = ordersData;
            _IsModalOpen = false;
            _currentViewModel = currentViewModel;
            _addItemViewModel = addItemViewModel;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Subscribes event handlers to events raised by the view models.
        /// </summary>
        /// <remarks>This function is called OnStartup in App.xaml.cs</remarks>
        public void SubscribeHandlersToEvents()
        {
            AddOrderViewModel.OrderSubmitted += SubmitOrderToDB;
            AddOrderViewModel.OrderCancelled += OnOrderCancelled;
            AddItemViewModel.OrderItemSelected += AddItemToNewOrder;
            AddOrderViewModel.OrderItemRemoved += UpdateItemStock;
        }

        /// <summary>
        /// Handles the OrderSubmitted event by adding new orders to the 
        /// orders list and navigating back to the ordersView.
        /// </summary>
        /// <param name="order">The new order to add</param>
        private void SubmitOrderToDB(Order order)
        {
            // Submit order to DB logic goes here
            OrdersViewModel.Orders.Add(order);
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
            AddOrderViewModel.AddOrderItem(newItem);
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
            AddItemViewModel.ReturnItemToStockList(stockItemId, quantity, onBackOrder);
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
                    if (AddOrderViewModel.Order == null)
                    {
                        AddOrderViewModel.LoadNewOrder(GetNewOrder());
                    }
                    CurrentViewModel = AddOrderViewModel;
                    break;
                case "AddItemView":
                    IsModalOpen = true;
                    break;
                case "CloseAddItem":
                    IsModalOpen = false;
                    break;
                case "CancelAndBackToOrders":
                    CurrentViewModel = OrdersViewModel;
                    AddOrderViewModel.CancelCurrentOrder();
                    break;
                case "OrdersView":
                default:
                    CurrentViewModel = OrdersViewModel;
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
