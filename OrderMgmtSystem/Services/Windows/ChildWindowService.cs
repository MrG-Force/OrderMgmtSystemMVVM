using DataModels;
using OrderMgmtSystem.ViewModels;
using System;

namespace OrderMgmtSystem.Services.Windows
{
    public class ChildWindowService
    {
        private ChildWindow _childWindow;
        private ViewModelBase _orderDetailsviewModel;
        private AddItemViewModel _addItemVM;
        private EditOrderViewModel _editOrderVM;

        public event Action<int> ChildWindowClosed = delegate { };

        public ChildWindowService(ViewModelBase OrderDetailsVM, ViewModelBase editOrderVM)
        {
            _childWindow = new ChildWindow();
            _orderDetailsviewModel = OrderDetailsVM;
            if (_orderDetailsviewModel is OrderDetailsViewModel odvm
                && editOrderVM is EditOrderViewModel edovm)
            {
                odvm.EditOrderRequested += OnEditOrderRequested;
                _addItemVM = odvm.AddItemViewModel;
                _editOrderVM = edovm;
                _editOrderVM.OrderUpdated += OnOrderUpdated;
                
            }
        }

        private void OnOrderUpdated()
        {
            _childWindow.DataContext = _orderDetailsviewModel;
        }

        private void OnEditOrderRequested(Order order)
        {
            EditOrderViewModel editOrderVM = new EditOrderViewModel(order, _addItemVM);
            _childWindow.DataContext = editOrderVM;
        }

        /// <summary>
        /// Starts and shows a new Window with the passed viewModel and
        /// wires up the window.Closing event.
        /// </summary>
        /// <param name="viewModel"></param>
        public void OpenWindow()
        {
            _childWindow.DataContext = _orderDetailsviewModel;
            _childWindow.Show();
            _childWindow.Closing += OnWindow_Closing;
        }


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
            IHandleOneOrder vm = (IHandleOneOrder)obj.DataContext;
            OnChildWindowClosed(vm.Order.Id);
        }
    }
}
