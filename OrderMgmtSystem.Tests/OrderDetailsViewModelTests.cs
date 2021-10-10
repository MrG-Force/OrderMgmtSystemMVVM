using DataModels;
using DataProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderMgmtSystem.Services.Dialogs.TestDialogs;
using OrderMgmtSystem.ViewModels;

namespace OrderMgmtSystem.Tests
{
    [TestClass]
    public class OrderDetailsViewModelTests
    {
        private static readonly IOrdersDataProvider ordersData = new TestDataProvider();
        private readonly OrderDetailsViewModel viewModel = new OrderDetailsViewModel(ordersData.GetOrder(), new MockDialogService());

        [TestMethod]
        public void OnProcessOrder_WithNoItemsOnBackOrder_OrderIsCompleted()
        {
            // Arrange
            // Act
            viewModel.ProcessOrder();
            // Assert
            Assert.IsTrue(viewModel.Order.OrderStatus == "Complete");
        }
        [TestMethod]
        public void OnProcessOrder_WithItemsOnBackOrder_OrderIsRejected()
        {
            // Arrange
            Order order = new Order()
            {
                HasItemsOnBackOrder = true
            };
            order.AddItem(new OrderItem(order.Id, 2)
            {
                Description = "Chair",
                Price = 15,
                Quantity = 10,
                OnBackOrder = 2
            });
            viewModel.Order = order;
            // Act
            viewModel.ProcessOrder();
            // Assert
            Assert.IsTrue(viewModel.Order.OrderStatus == "Rejected");
        }
    }
}
