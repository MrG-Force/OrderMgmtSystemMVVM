using DataProvider;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.ViewModels;
using OrderMgmtSystem.ViewModels.DialogViewModels;
using System.Windows;

namespace OrderMgmtSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Window CurrentMainWindow => Current.MainWindow; // to easily assign the owner of child windows and popups
        protected override void OnStartup(StartupEventArgs e)
        {
            ComposeObjects();
            MainWindow.Show();

            base.OnStartup(e);
        }

        /// <summary>
        /// A bootstrapper method respponsible for the instantiation of the view models 
        /// for the application.
        /// </summary>
        private static void ComposeObjects()
        {
            var ordersData = new RandomDataProvider();
            var currentViewModel = new OrdersViewModel(ordersData);
            var addOrderViewModel = new AddOrderViewModel();
            var orderDetailsViewModel = new OrderDetailsViewModel();
            var dialogService = new DialogService();
            var dialogViewModel = new QuantityViewModel("Quantity", "Please enter a quantity:");
            var addItemViewModel = new AddItemViewModel(ordersData.StockItems, dialogService, dialogViewModel);
            var viewModel = new MainWindowViewModel(ordersData, currentViewModel, addItemViewModel)
            {
                AddOrderViewModel = addOrderViewModel,
                OrdersViewModel = currentViewModel,
                OrderDetailsViewModel = orderDetailsViewModel
            };
            viewModel.SubscribeHandlersToEvents();
            Application.Current.MainWindow = new MainWindow
            {
                DataContext = viewModel
            };
        }
    }
}
