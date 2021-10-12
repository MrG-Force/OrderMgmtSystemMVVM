using DataProvider;
using OrderMgmtSystem.Commands;
using OrderMgmtSystem.Factories;
using OrderMgmtSystem.ViewModels;
using SQLDataProvider;
using System.Windows;

namespace OrderMgmtSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IOrdersDataProvider _dataProvider;

        /// <summary>
        /// This property is used to assign main window as owner of other 
        /// windows and dialogs through binding in Xaml.
        /// </summary>
        public static Window CurrentMainWindow => Current.MainWindow;
        public static DelegateCommand CloseAppCommand { get; private set; }

        /// <summary>
        /// Runs when the application starts.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Uncomment the DataProvider you want to use

            //_dataProvider = new RandomDataProvider();
            //_dataProvider = new TestDataProvider();
            _dataProvider = new SqlDataProvider();

            var vMFactory = new ViewModelFactory(_dataProvider);
            ComposeObjects(vMFactory);
            CloseAppCommand = new DelegateCommand(CloseApp);
            MainWindow.Show();

            base.OnStartup(e);
        }

        /// <summary>
        ///  A bootstrapper method responsible of composing the required view models. 
        /// </summary>
        /// <param name="vMFactory">An object that knows how to create view models</param>
        private static void ComposeObjects(ViewModelFactory vMFactory)
        {
            // Get MainWindowViewModel from Factory
            var mainWindowViewModel = (MainWindowViewModel)vMFactory.CreateViewModel("MainWindow");
            mainWindowViewModel.SubscribeHandlersToEvents();
            Application.Current.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
        }
        private static void CloseApp()
        {
            CurrentMainWindow.Close();
        }
    }
}
