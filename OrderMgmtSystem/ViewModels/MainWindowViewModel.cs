using DataModels;
using DataProvider;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.CommonEventArgs;
using OrderMgmtSystem.Factories;
using OrderMgmtSystem.Services.Windows;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
//TODO: Return Items to stock when order is deleted on edit order mode
//      Reconsider the inheritance of the viewModel for edit order
namespace OrderMgmtSystem.ViewModels
{
    /// <summary>
    /// A class that contains the logic for the application's MainWindow.
    /// </summary>
    /// <remarks>This class handles the main navigation and communication between views</remarks>
    internal class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        //-- ViewModels
        private ViewModelBase _currentViewModel;
        private readonly AddItemViewModel _addItemViewModel;
        private readonly AddOrderViewModel _addOrderViewModel;
        private readonly OrdersViewModel _ordersViewModel;
        private readonly ViewModelFactory _vMFactory;
        //-- DataProvider
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

        #region Constructor
        /// <summary>
        /// Sets up the MainWindowModel.
        /// </summary>
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

        #region Methods
        /// <summary>
        /// Subscribes event handlers to events raised by the view models.
        /// </summary>
        /// <remarks>This function is called OnStartup in App.xaml.cs</remarks>
        public void SubscribeHandlersToEvents()
        {
            _addOrderViewModel.OrderSubmitted += SubmitOrderToDB;
            _addOrderViewModel.OrderCancelled += OnOrderCancelled;
            _addOrderViewModel.OrderItemRemoved += UpdateItemStock;
            _addItemViewModel.NewOrderItemSelected += AddItemVM_NewOrderItemSelected;
        }

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
        /// Handles the OrderSubmitted event by adding new orders to the 
        /// orders list and navigating back to the ordersView.
        /// </summary>
        /// <param name="order">The new order to add</param>
        private void SubmitOrderToDB(object sender, Order order)
        {
            // Submit order to DB logic goes here
            _ordersViewModel.Orders.Add(order);
            Navigate("OrdersView");
        }

        /// <summary>
        /// Handles the OrderCancelled event by navigating back to the OrdersView.
        /// </summary>
        private void OnOrderCancelled(object sender, EventArgs e)
        {
            Debug.WriteLine(sender.ToString());
            Navigate("OrdersView");
        }

        /// <summary>
        /// Handles the NewOrderItemSelected event by calling the corresponding method 
        /// in the AddOrderViewModel to add the received item to the order. Closes the
        /// (modal) AddItemView.
        /// </summary>
        /// <param name="newItem">The item to add</param>
        private void AddItemVM_NewOrderItemSelected(object sender, OrderItem newItem)
        {
            _addOrderViewModel.AddOrderItem(newItem);
            Navigate("CloseAddItemView");
        }

        /// <summary>
        /// Handles the OrderItemRemoved event by re-stocking the item in the StockItems list.
        /// </summary>
        /// <param name="stockItemId"></param>
        /// <param name="quantity">The number of items(Quantity) removed from the order.</param>
        /// <param name="onBackOrder">The number of items that were unavailable when the order was placed.</param>
        private void UpdateItemStock(object sender, OrderItemRemovedEventArgs e)
        {
            _addItemViewModel.ReturnItemToStockList(e.StockItemId, e.Quantity, e.OnBackOrder);
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
            childWindowVM.OrderDetailsVM.DeleteOrderRequested += OnDeleteOrderRequested;
            IChildWindowService windowService = new ChildWindowService(childWindowVM);

            windowService.OpenWindow();
            windowService.ChildWindowClosed += DetailsWindowClosing;
        }

        /// <summary>
        /// This event handler deletes the SelectedOrder in the OrdersView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteOrderRequested(object sender, EventArgs e)
        {
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
        private void DetailsWindowClosing(int Id)
        {
            _ = _openedOrdersIds.Remove(Id);
            CreateNewOrderCommand.RaiseCanExecuteChanged();
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
        /// Handles all the navigation between views.
        /// </summary>
        /// <remarks>When needed the function also helps to call functions to initialize views</remarks>
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
