using DataModels;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.ViewModels;

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
            _orderDetailsVM.EditOrderRequested += OnEditOrderRequested;
            _editOrderVM = editOrderVM;
            _editOrderVM.OrderUpdated += OnOrderUpdated;
            _addItemVM = addItemVM;
            _currentViewModel = orderDetailsVM;
            _isModalOpen = false;

            NavigateCommand = new DelegateCommand<string>(Navigate);
        }
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }
        public OrderDetailsViewModel OrderDetailsVM => _orderDetailsVM;
        public EditOrderViewModel EditOrderVM => _editOrderVM;
        public AddItemViewModel AddItemVM => _addItemVM;
        public bool IsModalOpen
        {
            get => _isModalOpen;
            set => SetProperty(ref _isModalOpen, value);
        }
        public Order Order { get => OrderDetailsVM.Order; set => Order = value; }

        public DelegateCommand<string> NavigateCommand { get; private set; }
        private void OnOrderUpdated()
        {
            Navigate("OrderDetailsView");
        }

        private void OnEditOrderRequested()
        {
            Navigate("EditOrderView");
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
    }
}
