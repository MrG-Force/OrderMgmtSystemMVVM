using DataModels;
using DataProvider;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.ViewModels;
using OrderMgmtSystem.ViewModels.DialogViewModels;
using System;

namespace OrderMgmtSystem.Factories
{
    public class ViewModelFactory
    {
        private IOrdersDataProvider _dataProvider;

        public ViewModelFactory(IOrdersDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public ViewModelBase CreateViewModel(string viewType, Order order = null)
        {
            switch (viewType)
            {
                case "MainWindow": 
                    return new MainWindowViewModel(_dataProvider, CreateViewModel("Orders"), (AddItemViewModel)CreateViewModel("AddItem"), this);
                case "AddItem": 
                    return new AddItemViewModel(_dataProvider.StockItems, new DialogService(), (DialogViewModelBase<int>)CreateDialogViewModel("Quantity", "Quantity", "Please enter a quantity:"));
                case "AddOrder":
                    return new AddOrderViewModel();
                case "EditOrder":
                    return new EditOrderViewModel(order, (AddItemViewModel)CreateViewModel("AddItem"));
                case "OrderDetails":
                    return new OrderDetailsViewModel(order, (AddItemViewModel)CreateViewModel("AddItem"));
                case "Orders":
                    return new OrdersViewModel(_dataProvider);
                default:
                    throw new ArgumentException($"Invalid reader type: {viewType}");
            }
        }

        public static ViewModelBase CreateDialogViewModel(string viewType, string title, string message)
        {
            switch (viewType)
            {
                case "CancelOrderDialog":
                    return new CancelOrderDialogViewModel(title, message);
                case "Quantity":
                    return new QuantityViewModel(title, message);
                default:
                    throw new ArgumentException($"Invalid reader type: {viewType}");
            }
        }
    }
}
