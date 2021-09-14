using DataProvider;
using OrderMgmtSystem.ViewModels;
using System.Windows;

namespace OrderMgmtSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private IOrdersDataProvider _Data;
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
            var addItemViewModel = new AddItemViewModel(ordersData.StockItems);
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
