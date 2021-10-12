using DataModels;
using DataProvider;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.Services.Windows;
using OrderMgmtSystem.ViewModels;
using OrderMgmtSystem.ViewModels.BaseViewModels;
using OrderMgmtSystem.ViewModels.DialogViewModels;
using System;

namespace OrderMgmtSystem.Factories
{
    /// <summary>
    /// This class provides methods to create ViewModels using a IOrdersDataProvider to create the required data objects.
    /// </summary>
    public class ViewModelFactory
    {
        private readonly IOrdersDataProvider _dataProvider;

        public ViewModelFactory(IOrdersDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Creates a ViewModel for the specified View type.
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="order">For views that implement IHandleOneOrder</param>
        /// <param name="addItemViewModel">For classes that require the AddItemView modal</param>
        /// <returns></returns>
        public ViewModelBase CreateViewModel(string viewType, Order order = null, AddItemViewModel addItemViewModel = null)
        {
            switch (viewType)
            {
                case "MainWindow":
                    return new MainWindowViewModel
                        (_dataProvider,
                        (OrdersViewModel)CreateViewModel("Orders"),
                        (AddOrderViewModel)CreateViewModel("AddOrder"),
                        (AddItemViewModel)CreateViewModel("AddItem"), this);
                case "AddItem":
                    return new AddItemViewModel
                        (_dataProvider.StockItems,
                        new DialogService(),
                        (DialogViewModelBase<int>)CreateDialogViewModel("Quantity", "Quantity", "Please enter a quantity:"));
                case "AddOrder":
                    return new AddOrderViewModel(new DialogService());
                case "EditOrder":
                    return new EditOrderViewModel(order, new DialogService());
                case "OrderDetails":
                    return new OrderDetailsViewModel(order, new DialogService());
                case "Orders":
                    return new OrdersViewModel(_dataProvider);
                case "ChildWindow":
                    return new ChildWindowViewModel(_dataProvider,
                        (OrderDetailsViewModel)CreateViewModel("OrderDetails", order),
                        (EditOrderViewModel)CreateViewModel("EditOrder", order),
                        addItemViewModel);
                default:
                    throw new ArgumentException($"Invalid ViewModel type: {viewType}");
            }
        }

        /// <summary>
        /// A static method that creates a DialogViewModel for the given viewType.
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="title">The title for the dialog window.</param>
        /// <param name="message">The message that the dialog will display.</param>
        /// <param name="warning">For dialogs that require to display a warning.</param>
        /// <returns></returns>
        public static ViewModelBase CreateDialogViewModel(string viewType, string title, string message, string warning = null)
        {
            switch (viewType)
            {
                case "CancelOrderDialog":
                    return new CancelOrderDialogViewModel(title, message);
                case "RejectOrderDialog":
                    return new RejectOrderDialogViewModel(title, message, warning);
                case "SuccessDialog":
                    return new SuccessDialogViewModel(title, message);
                case "Quantity":
                    return new QuantityViewModel(title, message);
                default:
                    throw new ArgumentException($"Invalid ViewModel type: {viewType}");
            }
        }
    }
}
