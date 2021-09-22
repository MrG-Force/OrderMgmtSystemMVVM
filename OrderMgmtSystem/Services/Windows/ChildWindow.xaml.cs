using System.Windows;

namespace OrderMgmtSystem.Services.Windows
{
    /// <summary>
    /// Interaction logic for ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window
    {
        public ChildWindow()
        {
            InitializeComponent();
            Loaded += ChildWindow_Loaded;
        }

        /// <summary>
        /// Assigns this Window.Close() to the viewMode.Close delegate so
        /// the Window can be closed from the viewModel with a bound command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChildWindowViewModel viewModel)
            {
                if (viewModel.CurrentViewModel is ICloseWindows winCloserVM)
                {
                    winCloserVM.Close += () =>
                    {
                        Close();
                    };
                }
            };
        }
    }
}
