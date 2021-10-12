using OrderMgmtSystem.ViewModels.BaseViewModels;
using System;

namespace OrderMgmtSystem.Services.Windows
{
    /// <summary>
    /// A class to open and close windows.
    /// </summary>
    public class ChildWindowService : IChildWindowService
    {
        #region Constructor
        public ChildWindowService(ChildWindowViewModel childWindowVM)
        {
            _childWindow = new ChildWindow();
            _childWindowVM = childWindowVM;
        }
        #endregion

        #region Fields
        private ChildWindow _childWindow;
        private ChildWindowViewModel _childWindowVM;
        #endregion

        #region Events
        public event EventHandler<int> ChildWindowClosed;
        #endregion

        #region Methods
        /// <summary>
        /// Starts and shows a new Window with the passed viewModel and
        /// wires up the window.Closing event.
        /// </summary>
        /// <param name="viewModel"></param>
        public void OpenWindow()
        {
            _childWindow.DataContext = _childWindowVM;
            _childWindow.Show();
            _childWindow.Closed += OnWindow_Closed; // Consider Closing event for preventing window to close
        }

        /// <summary>
        /// Raises the ChildWindowClosed event passing the orderId
        /// to the subscribed listeners.
        /// </summary>
        /// <param name="orderId"></param>
        private void OnChildWindowClosed(int orderId)
        {
            ChildWindowClosed?.Invoke(this, orderId);
        }

        /// <summary>
        /// Handles the window Closing event by calling OnChildWindowClosed
        /// passing the corresponding order Id from the sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindow_Closed(object sender, EventArgs e)
        {
            ChildWindow obj = (ChildWindow)sender;
            IHandleOneOrder vm = (IHandleOneOrder)obj.DataContext;
            OnChildWindowClosed(vm.Order.Id);
        }
        #endregion
    }
}
