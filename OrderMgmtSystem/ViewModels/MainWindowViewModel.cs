using DataModels;
using DataProvider;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Factories;
using OrderMgmtSystem.Services.Windows;
using System.Collections.Generic;

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
        private bool _isModalOpen;
        private List<int> openedOrdersIds = new List<int>();
        private ViewModelFactory _vMFactory;
        #endregion

        #region Properties
        public AddItemViewModel AddItemViewModel
        {
            get => _addItemViewModel;
            set => SetProperty(ref _addItemViewModel, value);
        }
        internal AddOrderViewModel AddOrderViewModel { get; set; }
        internal OrdersViewModel OrdersViewModel { get; set; }
        public DelegateCommand CreateNewOrderCommand { get; private set; }
        public RelayCommandT<string> NavigateCommand { get; private set; }
        public DelegateCommand<Order> SeeOrderDetailsCommand { get; private set; }
        public DelegateCommand CloseAppCommand { get; private set; }
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }
        public bool IsModalOpen
        {
            get => _isModalOpen;
            set => SetProperty(ref _isModalOpen, value);
        }
        public bool OrderDetailsWindowsOpen
        {
            get
            {
                return openedOrdersIds.Count == 0;
            }

        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the MainWindowModel object using dependency injection.
        /// </summary>
        /// <param name="ordersData"></param>
        /// <param name="currentViewModel"></param>
        /// <param name="addItemViewModel"></param>
        public MainWindowViewModel(IOrdersDataProvider ordersData, ViewModelBase currentViewModel, AddItemViewModel addItemViewModel, ViewModelFactory vMFactory)
        {
            _Data = ordersData;
            _isModalOpen = false;
            _currentViewModel = currentViewModel;
            _addItemViewModel = addItemViewModel;
            NavigateCommand = new RelayCommandT<string>(Navigate, () => OrderDetailsWindowsOpen);
            SeeOrderDetailsCommand = new DelegateCommand<Order>(ViewOrderDetails);
            CreateNewOrderCommand = new DelegateCommand(CreateNewOrder);
            _vMFactory = vMFactory;
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
            AddItemViewModel.NewOrderItemSelected += AddItemToNewOrder;
            AddOrderViewModel.OrderItemRemoved += UpdateItemStock;
        }

        private void CreateNewOrder()
        {
            if (AddOrderViewModel.Order == null)
            {
                AddOrderViewModel.LoadNewOrder(GetNewOrder());
            }
            Navigate("AddOrderView");


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
        /// Handles the NewOrderItemSelected event by calling the corresponding method 
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
                    CurrentViewModel = AddOrderViewModel;
                    break;
                case "AddItemView":
                    IsModalOpen = true;
                    break;
                case "CloseAddItem":
                    IsModalOpen = false;
                    break;
                case "OrdersView":
                default:
                    CurrentViewModel = OrdersViewModel;
                    break;
            }
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
            if (openedOrdersIds.Contains(order.Id))
            {
                return;
            }
            openedOrdersIds.Add(order.Id);
            NavigateCommand.RaiseCanExecuteChanged();
            OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel(order, AddItemViewModel);
            orderDetailsVM.DeleteOrderRequested += OnDeleteOrderRequested;
            EditOrderViewModel editOrderVM = new EditOrderViewModel(order, AddItemViewModel);
            ChildWindowService windowService = new ChildWindowService(orderDetailsVM, editOrderVM);
            windowService.OpenWindow();
            AddItemViewModel.NewOrderItemSelected -= AddItemToNewOrder;
            windowService.ChildWindowClosed += DetailsWindowClosing;
        }

        private void OnDeleteOrderRequested(int id)
        {
            OrdersViewModel.DeleteOrder(id);
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
            _ = openedOrdersIds.Remove(Id);
            NavigateCommand.RaiseCanExecuteChanged();
            AddItemViewModel.NewOrderItemSelected += AddItemToNewOrder;
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
