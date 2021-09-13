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
        private static void ComposeObjects()
        {
            var ordersData = new RandomDataProvider();
            var viewModel = new MainWindowViewModel(ordersData);
            Application.Current.MainWindow = new MainWindow
            {
                DataContext = viewModel
            };
        }
    }
}
