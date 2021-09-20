using DataModels;
using OrderMgmtSystem.Commands;
using System;
using System.Collections.ObjectModel;

namespace OrderMgmtSystem.ViewModels
{
    public class EditOrderViewModel : AddOrderViewModel, IHandleOneOrder
    {
        private bool _isModalOpen;

        public string Title { get; }
        public bool IsModalOpen
        {
            get => _isModalOpen;
            set => SetProperty(ref _isModalOpen, value);
        }
        public AddItemViewModel AddItemViewModel { get; private set; }

        public event Action OrderUpdated = delegate { };
        public EditOrderViewModel(Order order, AddItemViewModel addItemVM) : base(order)
        {
            Order = order;
            Title = $"Editing order number: {order.Id}";
            OrderItems = new ObservableCollection<OrderItem>(order.OrderItems);
            AddItemViewModel = addItemVM;
            AddItemViewModel.EditingOrderItemSelected += AddOrderItem;
            _isModalOpen = false;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }
        internal override void AddOrderItem(OrderItem newItem)
        {
            base.AddOrderItem(newItem);
            Navigate("CloseAddItem");
        }
        internal override void SubmitOrder()
        {
            OnOrderUpdated();

        }

        private void OnOrderUpdated()
        {
            OrderUpdated();
        }

        private void Navigate(string destination = null)
        {
            switch (destination)
            {
                case "CloseAddItem":
                    IsModalOpen = false;
                    break;
                default:
                    IsModalOpen = true;
                    break;
            }
        }
    }
}
