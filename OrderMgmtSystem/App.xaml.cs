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
        private IOrdersDataProvider _Data;
        public static Window CurrentMainWindow => Current.MainWindow; // to easily assign the owner of child windows and popups
        protected override void OnStartup(StartupEventArgs e)
        {
            _Data = new RandomDataProvider();
            MainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel(_Data)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
