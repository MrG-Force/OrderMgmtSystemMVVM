using DataModels;
using DataProvider;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Factories;
using OrderMgmtSystem.Services.Windows;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System;
using System.Collections.Generic;

namespace OrderMgmtSystem.ViewModels
{
    /// <summary>
    /// A class that contains the logic for the application's MainWindow.
    /// </summary>
    /// <remarks>This class handles the main navigation and communication between views</remarks>
    internal class MainWindowViewModel : ViewModelBase
    {
        #region Constructor
        /// <summary>
        /// Sets up the MainWindowModel.
        /// </summary>
        /// <remarks>Uses dependency injection through constructor</remarks>
        /// <param name="ordersData">An object to comunicate with the data source</param>
        /// <param name="ordersVM"></param>
        /// <param name="addOrderVM"></param>
        /// <param name="addItemVM"></param>
        /// <param name="vMFactory"></param>
        public MainWindowViewModel(IOrdersDataProvider ordersData, OrdersViewModel ordersVM, AddOrderViewModel addOrderVM, AddItemViewModel addItemVM, ViewModelFactory vMFactory)
        {
            _data = ordersData;
            _ordersViewModel = ordersVM;
            _addOrderViewModel = addOrderVM;
            _addItemViewModel = addItemVM;
            _currentViewModel = _ordersViewModel;
            _vMFactory = vMFactory;

            _isModalOpen = false;
            NavigateCommand = new RelayCommandT<string>(Navigate);
            ViewOrderDetailsCommand = new DelegateCommand<Order>(ViewOrderDetails);
            CreateNewOrderCommand = new DelegateCommand(CreateNewOrder, () => OrderDetailsWindowsOpen);
        }
        #endregion

        #region Fields

        private ViewModelBase _currentViewModel;
        private readonly AddItemViewModel _addItemViewModel;
        private readonly AddOrderViewModel _addOrderViewModel;
        private readonly OrdersViewModel _ordersViewModel;
        private readonly ViewModelFactory _vMFactory;

        private readonly IOrdersDataProvider _data;

        private readonly List<int> _openedOrdersIds = new List<int>();
        private bool _isModalOpen;
        #endregion

        #region Properties
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }
        public AddItemViewModel AddItemViewModel => _addItemViewModel;
        public DelegateCommand CreateNewOrderCommand { get; private set; }
        public RelayCommandT<string> NavigateCommand { get; private set; }
        public DelegateCommand<Order> ViewOrderDetailsCommand { get; private set; }
        public DelegateCommand CloseAppCommand { get; private set; }
        public bool IsModalOpen
        {
            get => _isModalOpen;
            set => SetProperty(ref _isModalOpen, value);
        }
        public bool OrderDetailsWindowsOpen
        {
            get
            {
                return _openedOrdersIds.Count == 0;
            }

        }
        #endregion

        #region Methods
        #region Event handling
        /// <summary>
        /// Subscribes event handlers to events raised by the view models.
        /// </summary>
        /// <remarks>This function is called OnStartup in App.xaml.cs</remarks>
        public void SubscribeHandlersToEvents()
        {
            _addOrderViewModel.OrderSubmitted += AddOrderVM_OrderSubmitted;
            _addOrderViewModel.OperationCancelled += AddOrderVM_OperationCancelled;
            _addOrderViewModel.OrderItemRemoved += AddOrderVM_OrderItemRemoved;
            _addItemViewModel.NewOrderItemSelected += AddItemVM_NewOrderItemSelected;
        }

        /// <summary>
        /// Handles the OrderSubmitted event by adding new orders to the 
        /// orders list and navigating back to the ordersView.
        /// </summary>
        /// <param name="order">The new order to add</param>
        private void AddOrderVM_OrderSubmitted(object sender, Order order)
        {
            _data.UpdateOrderState(order.Id, 2);
            _ordersViewModel.Orders.Add(order);
            Navigate("OrdersView");
        }

        /// <summary>
        /// Handles the OperationCancelled event by navigating back to the OrdersView.
        /// </summary>
        private void AddOrderVM_OperationCancelled(object sender, int orderId)
        {
            _data.DeleteOrder(orderId);
            Navigate("OrdersView");
        }

        /// <summary>
        /// Handles the OrderItemRemoved event by re-stocking the item in the StockItems list.
        /// </summary>
        /// <param name="stockItemId"></param>
        /// <param name="quantity">The number of items(Quantity) removed from the order.</param>
        /// <param name="onBackOrder">The number of items that were unavailable when the order was placed.</param>
        private void AddOrderVM_OrderItemRemoved(object sender, OrderItem orderItem)
        {
            _data.RemoveOrderItem(orderItem);
            _addItemViewModel.ReturnItemToStockList(orderItem);
        }

        /// <summary>
        /// Handles the NewOrderItemSelected event by calling the corresponding methods 
        /// in the ViewModel and in the DataProvider. Closes the (modal)AddItemView.
        /// </summary>
        /// <param name="newItem">The item to add</param>
        private void AddItemVM_NewOrderItemSelected(object sender, OrderItem newItem)
        {
            newItem.OrderHeaderId = _addOrderViewModel.Order.Id;
            _data.UpdateOrInsertOrderItem(newItem);
            _addOrderViewModel.CheckNewOrExistingItem(newItem);
            Navigate("CloseAddItemView");
        }

        /// <summary>
        /// This event handler deletes the SelectedOrder in the OrdersView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderDetailsVM_DeleteOrderRequested(object sender, EventArgs e)
        {
            _data.DeleteOrder(_ordersViewModel.SelectedOrder.Id);
            _ordersViewModel.DeleteOrder();
        }

        /// <summary>
        /// Handles the OrderDetailsWindow Closing event.
        /// </summary>
        /// <remarks>
        /// Removes the current's order Id from the OrderWindowsOpened list 
        /// so the order can be opened again in a new window.
        /// </remarks>
        /// <param name="Id"></param>
        private void WindowService_ChildWindowClosed(object sender, int Id)
        {
            _ = _openedOrdersIds.Remove(Id);
            CreateNewOrderCommand.RaiseCanExecuteChanged();
        }
        #endregion

        /// <summary>
        /// Calls a the LoadNewOrder method and navigates to the AddOrderView.
        /// </summary>
        private void CreateNewOrder()
        {
            if (_addOrderViewModel.Order == null)
            {
                _addOrderViewModel.LoadNewOrder(GetNewOrder());
            }
            Navigate("AddOrderView");
        }

        /// <summary>
        /// Opens a new window displaying the details of the selected Order.
        /// </summary>
        /// <remarks>
        /// Registers the order Id opened in the OrderWindowsOpened list to prevent the
        /// same order to be opened in more than one window.
        /// </remarks>
        /// <param name="order">Sent by View as CommandParameter through Binding</param>
        private void ViewOrderDetails(Order order)
        {
            // Don't open 2 windows with the same order
            if (_openedOrdersIds.Contains(order.Id))
            {
                return;
            }
            _openedOrdersIds.Add(order.Id);
            CreateNewOrderCommand.RaiseCanExecuteChanged();

            var childWindowVM = (ChildWindowViewModel)_vMFactory.CreateViewModel("ChildWindow", order, AddItemViewModel);
            childWindowVM.OrderDetailsVM.DeleteOrderRequested += OrderDetailsVM_DeleteOrderRequested;
            IChildWindowService windowService = new ChildWindowService(childWindowVM);

            windowService.OpenWindow();
            windowService.ChildWindowClosed += WindowService_ChildWindowClosed;
        }

        /// <summary>
        /// Gets an empty order with the corresponding Id from the data provider.
        /// </summary>
        /// <returns></returns>
        private Order GetNewOrder()
        {
            return _data.GetOrder();
        }

        /// <summary>
        /// Handles the navigation between views.
        /// </summary>
        /// <param name="destination"></param>
        private void Navigate(string destination)
        {
            switch (destination)
            {
                case "AddOrderView":
                    CurrentViewModel = _addOrderViewModel;
                    break;
                case "AddItemView":
                    IsModalOpen = true;
                    break;
                case "CloseAddItemView":
                    IsModalOpen = false;
                    break;
                case "OrdersView":
                default:
                    CurrentViewModel = _ordersViewModel;
                    break;
            }
        }
        #endregion
    }
}
