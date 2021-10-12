using DataModels;
using DataProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderMgmtSystem.Services.Dialogs.TestDialogs;
using OrderMgmtSystem.ViewModels;
using System.Collections.Generic;

namespace OrderMgmtSystem.Tests
{
    [TestClass]
    public class AddOrderViewModelTests
    {
        private static readonly IOrdersDataProvider ordersData = new TestDataProvider();
        private readonly AddOrderViewModel viewModel = new AddOrderViewModel(new MockDialogService());
        OrderItem orderItem1 = new OrderItem()
        {
            StockItemId = 2,
            Description = "Chair",
            Quantity = 1,
            Price = 25,
            OnBackOrder = 0
        };
        OrderItem orderItem2 = new OrderItem()
        {
            StockItemId = 2,
            Description = "Chair",
            Quantity = 1,
            Price = 25,
            OnBackOrder = 0
        };
        OrderItem orderItem3 = new OrderItem()
        {
            StockItemId = 1,
            Description = "Table",
            Quantity = 1,
            Price = 100,
            OnBackOrder = 0
        };

        /// <summary>
        /// Checks that a new Order is loaded in the ViewModel using the Data provider method
        /// and that its State is set to "New".
        /// </summary>
        [TestMethod]
        public void OnLoadNewOrder_ANewOrderIsLoaded()
        {
            // Arrange
            Order newOrder = ordersData.GetOrder();
            Assert.IsNull(viewModel.Order);
            // Act
            viewModel.LoadNewOrder(newOrder);
            // Assert
            Assert.IsNotNull(viewModel.Order);
            Assert.IsTrue(viewModel.Order.OrderStateId == 1);
        }

        [TestMethod]
        public void OnSubmitOrder_OrderIsSubmittedAndVMOrderRefreshed()
        {
            // Arrange
            viewModel.LoadNewOrder(ordersData.GetOrder());
            Order orderToBeSubmitted = viewModel.Order;
            // Act
            viewModel.SubmitOrder();
            // Assert
            Assert.IsTrue(viewModel.OrderItems.Count == 0 &&
                orderToBeSubmitted.OrderStateId == 2 &&
                viewModel.Order == null);
        }

        [TestMethod]
        public void OnAddNewOrderItem_OrderItemIsAddedToVMOrderItemsAndOrder()
        {
            // Arrange
            viewModel.LoadNewOrder(ordersData.GetOrder());
            OrderItem orderItem = new OrderItem();
            // Act
            viewModel.AddNewOrderItem(orderItem);
            // Assert
            Assert.IsTrue(viewModel.OrderItems.Contains(orderItem) &&
                viewModel.Order.OrderItems.Contains(orderItem));
        }

        [TestMethod]
        public void OnUpdateExistingOrderItem_ItemQuantityIncreases()
        {
            // Arrange
            int beforeQuantity = orderItem1.Quantity;

            viewModel.LoadNewOrder(ordersData.GetOrder());
            viewModel.AddNewOrderItem(orderItem1);

            // Act
            viewModel.UpdateExistingOrderItem(orderItem2, orderItem1);

            // Assert
            Assert.IsTrue(viewModel.Order.OrderItems.
                Find(i => i.StockItemId == 2)
                .Quantity == orderItem2.Quantity + beforeQuantity);
        }

        [TestMethod]
        public void OnRemoveItem_ItemIsRemoved()
        {
            // Arrange
            viewModel.LoadNewOrder(ordersData.GetOrder());
            viewModel.AddNewOrderItem(orderItem1);
            // Act
            viewModel.RemoveItem(orderItem1);
            // Assert
            Assert.IsTrue(viewModel.OrderItems.Count == 0 &&
                viewModel.Order.OrderItems.Count == 0);
        }

        [TestMethod]
        public void OnReturnItemsToStock_EventsAreRaisedAndOrderItemsSend()
        {
            // Arrange
            List<OrderItem> receivedItems = new List<OrderItem>();
            viewModel.OrderItemRemoved += delegate (object sender, OrderItem oi)
            {
                receivedItems.Add(oi);
            };
            viewModel.LoadNewOrder(ordersData.GetOrder());
            viewModel.AddNewOrderItem(orderItem2);
            viewModel.AddNewOrderItem(orderItem3);
            // Act
            viewModel.ReturnItemsToStock();
            // Assert 
            Assert.IsTrue(receivedItems.Count == 2 &&
                receivedItems.Contains(orderItem2) &&
                receivedItems.Contains(orderItem3));
        }
    }
}
