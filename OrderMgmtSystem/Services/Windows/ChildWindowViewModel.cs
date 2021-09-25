using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.CommonEventArgs;
using OrderMgmtSystem.ViewModels;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using System.Collections.ObjectModel;

namespace OrderMgmtSystem.Services.Windows
{
    public class ChildWindowViewModel : ViewModelBase, IHandleOneOrder
    {
        private ViewModelBase _currentViewModel;
        private OrderDetailsViewModel _orderDetailsVM;
        private EditOrderViewModel _editOrderVM;
        private AddItemViewModel _addItemVM;
        private bool _isModalOpen;

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

        private void SubscribeToEvents()
        {
            _orderDetailsVM.EditOrderRequested += OrderDetailsVM_EditOrderRequested;
            _editOrderVM.OrderUpdated += EditOrderVM_OrderUpdated;
            _editOrderVM.OrderItemRemoved += EditOrderVM_OrderItemRemoved;
            _addItemVM.EditingOrderItemSelected += AddItemVM_EditingOrderItemSelected;
        }

        private void UnsubscribeToEvents()
        {
            _orderDetailsVM.EditOrderRequested -= OrderDetailsVM_EditOrderRequested;
            _editOrderVM.OrderUpdated -= EditOrderVM_OrderUpdated;
            _editOrderVM.OrderItemRemoved -= EditOrderVM_OrderItemRemoved;
            _addItemVM.EditingOrderItemSelected -= AddItemVM_EditingOrderItemSelected;
        }

        private void EditOrderVM_OrderItemRemoved(object sender, OrderItemRemovedEventArgs e)
        {
            _addItemVM.ReturnItemToStockList(e.StockItemId, e.Quantity, e.OnBackOrder);
        }

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
        private void EditOrderVM_OrderUpdated()
        {
            OrderDetailsVM.Order = EditOrderVM.Order;
            EditOrderVM.TempOrder = new Order(OrderDetailsVM.Order);
            EditOrderVM.TempOrderItems = new ObservableCollection<OrderItem>(OrderDetailsVM.Order.OrderItems);
            EditOrderVM.InitialTotal = OrderDetailsVM.Order.Total;
            EditOrderVM.RefreshCanSubmit();
            
            Navigate("OrderDetailsView");
        }

        private void OrderDetailsVM_EditOrderRequested()
        {
            Navigate("EditOrderView");
        }

        private void AddItemVM_EditingOrderItemSelected(OrderItem item)
        {
            EditOrderVM.AddOrderItem(item);
            Navigate("CloseAddItemView");
        }

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

        public override void Dispose()
        {
            UnsubscribeToEvents();
            base.Dispose();
        }
    }
}
