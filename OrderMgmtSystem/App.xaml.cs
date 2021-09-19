using DataProvider;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Services.Dialogs;
using OrderMgmtSystem.Services.Windows;
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
        public static DelegateCommand CloseAppCommand { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            ComposeObjects();
            CloseAppCommand = new DelegateCommand(CloseApp);
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
            var windowService = new ChildWindowService();
            var dialogService = new DialogService();
            var dialogViewModel = new QuantityViewModel("Quantity", "Please enter a quantity:");
            var addItemViewModel = new AddItemViewModel(ordersData.StockItems, dialogService, dialogViewModel);
            var viewModel = new MainWindowViewModel(ordersData, currentViewModel, addItemViewModel, windowService)
            {
                AddOrderViewModel = addOrderViewModel,
                OrdersViewModel = currentViewModel,
            };
            viewModel.SubscribeHandlersToEvents();
            Application.Current.MainWindow = new MainWindow
            {
                DataContext = viewModel
            };
        }
        private static void CloseApp()
        {
            CurrentMainWindow.Close();
        }
    }
}
