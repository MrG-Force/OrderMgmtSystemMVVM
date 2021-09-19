using OrderMgmtSystem.ViewModels;
using System;

namespace OrderMgmtSystem.Services.Windows
{
    public class ChildWindowService
    {
        /// <summary>
        /// Starts and shows a new Window with the passed viewModel and
        /// wires up the window.Closing event.
        /// </summary>
        /// <param name="viewModel"></param>
        public void OpenWindow(ViewModelBase viewModel)
        {
            ChildWindow window = new ChildWindow();
            window.DataContext = viewModel;
            window.Show();
            window.Closing += OnWindow_Closing;
        }

        public event Action<int> ChildWindowClosed = delegate { };

        /// <summary>
        /// Raises the ChildWindowClosed event passing the orderId
        /// to the subscribed listeners.
        /// </summary>
        /// <param name="orderId"></param>
        private void OnChildWindowClosed(int orderId)
        {
            ChildWindowClosed(orderId);
        }

        /// <summary>
        /// Handles the window Closing event by calling OnChildWindowClosed
        /// passing the corresponding order Id from the sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChildWindow obj = (ChildWindow)sender;
            OrderDetailsViewModel vm = (OrderDetailsViewModel)obj.DataContext;
            OnChildWindowClosed(vm.Order.Id);
        }
    }
}
