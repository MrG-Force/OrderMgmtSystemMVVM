using DataModels;
using DataProvider;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.ViewModels;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System;

namespace OrderMgmtSystem.Services.Windows
{
    public class ChildWindowViewModel : ViewModelBase, IHandleOneOrder
    {
        #region Constructor
        public ChildWindowViewModel(IOrdersDataProvider data, OrderDetailsViewModel orderDetailsVM, EditOrderViewModel editOrderVM, AddItemViewModel addItemVM)
        {
            _data = data;
            _orderDetailsVM = orderDetailsVM;
            _editOrderVM = editOrderVM;;
            _addItemVM = addItemVM;
            _currentViewModel = orderDetailsVM;
            _isModalOpen = false;

            NavigateCommand = new DelegateCommand<string>(Navigate);

            SubscribeToEvents();
        }
        #endregion

        #region Fields
        IOrdersDataProvider _data;
        private ViewModelBase _currentViewModel;
        private readonly OrderDetailsViewModel _orderDetailsVM;
        private readonly EditOrderViewModel _editOrderVM;
        private readonly AddItemViewModel _addItemVM;
        private bool _isModalOpen;
        #endregion

        #region Properties
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }
        public OrderDetailsViewModel OrderDetailsVM => _orderDetailsVM;
        private EditOrderViewModel EditOrderVM => _editOrderVM;
        public AddItemViewModel AddItemVM => _addItemVM;
        public bool IsModalOpen
        {
            get => _isModalOpen;
            set => SetProperty(ref _isModalOpen, value);
        }
        public Order Order { get => OrderDetailsVM.Order; set => Order = value; }

        public DelegateCommand<string> NavigateCommand { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Subscribe the class to the relevant events in the child View Models.
        /// </summary>
        private void SubscribeToEvents()
        {
            _orderDetailsVM.EditOrderRequested += OrderDetailsVM_EditOrderRequested;
            _orderDetailsVM.DeleteOrderRequested += OrderDetailsVM_OrderDeletedOrRejected;
            _orderDetailsVM.OrderRejected += OrderDetailsVM_OrderDeletedOrRejected;
            _editOrderVM.OrderUpdated += EditOrderVM_OrderUpdated;
            _editOrderVM.OrderItemRemoved += EditOrderVM_OrderItemRemoved;
            _editOrderVM.OperationCancelled += EditOrderVM_OperationCancelled;
            _addItemVM.EditingOrderItemSelected += AddItemVM_EditingOrderItemSelected;
        }

        /// <summary>
        /// Unsubscribes the class from the events so it can be properly disposed.
        /// </summary>
        private void UnsubscribeToEvents()
        {
            _orderDetailsVM.EditOrderRequested -= OrderDetailsVM_EditOrderRequested;
            _orderDetailsVM.DeleteOrderRequested -= OrderDetailsVM_OrderDeletedOrRejected;
            _orderDetailsVM.OrderRejected -= OrderDetailsVM_OrderDeletedOrRejected;
            _editOrderVM.OrderUpdated -= EditOrderVM_OrderUpdated;
            _editOrderVM.OrderItemRemoved -= EditOrderVM_OrderItemRemoved;
            _editOrderVM.OperationCancelled -= EditOrderVM_OperationCancelled;
            _addItemVM.EditingOrderItemSelected -= AddItemVM_EditingOrderItemSelected;
        }

        /// <summary>
        /// Synchronize the Order property in the EditOrderVM and OrderDetailsVM and refreshes the properties
        /// in the EditOrderVM to continue editing if needed.
        /// </summary>
        private void UpdateViewModels()
        {
            OrderDetailsVM.Order = EditOrderVM.Order;
            EditOrderVM.TempOrder = new Order(OrderDetailsVM.Order);
            EditOrderVM.TempOrderItems.Clear();
            EditOrderVM.InitialTotal = OrderDetailsVM.Order.Total;
            EditOrderVM.RefreshCanSubmit();
        }

        #region EventHandling
        /// <summary>
        /// This event handler takes the application back to the OrderDetailsView and calls the UpdateViewModels method. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditOrderVM_OrderUpdated(object sender, EventArgs e)
        {
            UpdateViewModels();
            Navigate("OrderDetailsView");
        }

        /// <summary>
        /// This event handler returns items removed from an order back to the Stock items list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Contain relevant information to return the item</param>
        private void EditOrderVM_OrderItemRemoved(object sender, OrderItem item)
        {
            //_data.RemoveOrderItem();
            _addItemVM.ReturnItemToStockList(item.StockItemId, item.Quantity, item.OnBackOrder);
        }

        /// <summary>
        /// This event handler takes the user to the OrderDetailsView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditOrderVM_OperationCancelled(object sender, int orderId)
        {
            _data.DeleteOrder(orderId);
            Navigate("OrderDetailsView");
        }

        /// <summary>
        /// This event handler takes the application to the EditOrderView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderDetailsVM_EditOrderRequested(object sender, EventArgs e)
        {
            Navigate("EditOrderView");
        }

        /// <summary>
        /// This event hanlder returns the Stock items to the inventory when the order is deleted or rejected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderDetailsVM_OrderDeletedOrRejected(object sender, EventArgs e)
        {
            foreach (var item in _orderDetailsVM.Order.OrderItems)
            {
                _addItemVM.ReturnItemToStockList(item.StockItemId, item.Quantity, item.OnBackOrder);
            };
        }

        /// <summary>
        /// This event handler adds a selected OrderItem to the Order and closes the modal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="item"></param>
        private void AddItemVM_EditingOrderItemSelected(object sender, OrderItem item)
        {
            EditOrderVM.AddOrderItem(item);
            Navigate("CloseAddItemView");
        }
        #endregion

        /// <summary>
        /// Handles the navigation in the ChildWindow.
        /// </summary>
        /// <param name="destination"></param>
        private void Navigate(string destination)
        {
            switch (destination)
            {
                case "EditOrderView":
                    CurrentViewModel = EditOrderVM;
                    break;
                case "AddItemView":
                    IsModalOpen = true;
                    break;
                case "CloseAddItemView":
                    IsModalOpen = false;
                    break;
                case "OrderDetailsView":
                default:
                    CurrentViewModel = OrderDetailsVM;
                    break;
            }
        }

        /// <summary>
        /// Allows this transient ViewModel to be disposed correctly by unsubscribing from all the registered events.
        /// </summary>
        public override void Dispose()
        {
            UnsubscribeToEvents();
            base.Dispose();
        }
        #endregion
    }
}
