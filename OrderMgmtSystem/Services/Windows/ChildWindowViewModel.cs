using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.CommonEventArgs;
using OrderMgmtSystem.ViewModels;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System;
using System.Collections.ObjectModel;

namespace OrderMgmtSystem.Services.Windows
{
    public class ChildWindowViewModel : ViewModelBase, IHandleOneOrder
    {
        #region Constructor
        public ChildWindowViewModel(OrderDetailsViewModel orderDetailsVM, EditOrderViewModel editOrderVM, AddItemViewModel addItemVM)
        {
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
            _editOrderVM.OrderUpdated += EditOrderVM_OrderUpdated;
            _editOrderVM.OrderItemRemoved += EditOrderVM_OrderItemRemoved;
            _addItemVM.EditingOrderItemSelected += AddItemVM_EditingOrderItemSelected;
        }
        /// <summary>
        /// Unsubscribes the class from the events so it can be properly disposed.
        /// </summary>
        private void UnsubscribeToEvents()
        {
            _orderDetailsVM.EditOrderRequested -= OrderDetailsVM_EditOrderRequested;
            _editOrderVM.OrderUpdated -= EditOrderVM_OrderUpdated;
            _editOrderVM.OrderItemRemoved -= EditOrderVM_OrderItemRemoved;
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
            EditOrderVM.TempOrderItems = new ObservableCollection<OrderItem>(OrderDetailsVM.Order.OrderItems);
            EditOrderVM.InitialTotal = OrderDetailsVM.Order.Total;
            EditOrderVM.RefreshCanSubmit();
        }

        /// <summary>
        /// This event handler returns items removed from an order back to the Stock items list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Contain relevant information to return the item</param>
        private void EditOrderVM_OrderItemRemoved(object sender, OrderItemRemovedEventArgs e)
        {
            _addItemVM.ReturnItemToStockList(e.StockItemId, e.Quantity, e.OnBackOrder);
        }

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
        /// This event handler takes the application to the EditOrderView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderDetailsVM_EditOrderRequested(object sender, EventArgs e)
        {
            Navigate("EditOrderView");
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
