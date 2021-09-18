using OrderMgmtSystem.ViewModels;
using System;

namespace OrderMgmtSystem.Services.Windows
{
    public class ChildWindowService
    {
        public void OpenWindow(ViewModelBase viewModel)
        {
            ChildWindow window = new ChildWindow();
            window.DataContext = viewModel;
            window.Show();
            window.Closing += OnWindow_Closing;
        }

        public event Action<int> ChildWindowClosed = delegate { };
        private void OnChildWindowClosed(int orderId)
        {
            ChildWindowClosed(orderId);
        }

        private void OnWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChildWindow obj = (ChildWindow)sender;
            OrderDetailsViewModel vm = (OrderDetailsViewModel)obj.DataContext;
            OnChildWindowClosed(vm.Order.Id);
        }
    }
}
