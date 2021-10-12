using DataProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderMgmtSystem.ViewModels;
using System.Linq;

namespace OrderMgmtSystem.Tests
{
    [TestClass]
    public class OrdersViewModelTests
    {
        private static readonly IOrdersDataProvider ordersData = new TestDataProvider();
        private readonly OrdersViewModel viewModel = new OrdersViewModel(ordersData);

        [TestMethod]
        public void OnDeleteOrder_SelectedOrderIsRemoved()
        {
            // Arrange
            viewModel.SelectedOrder = viewModel.Orders.FirstOrDefault();
            int startCount = viewModel.Orders.Count();
            // Act
            viewModel.DeleteOrder();
            // Assert
            Assert.IsTrue(viewModel.Orders.Count == startCount - 1);
            Assert.IsTrue(viewModel.Orders.IndexOf(viewModel.SelectedOrder) == -1);
        }
    }
}
