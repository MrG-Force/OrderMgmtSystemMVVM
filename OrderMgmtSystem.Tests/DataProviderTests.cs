using DataModels;
using DataProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLDataProvider;
using System.Collections.Generic;

namespace OrderMgmtSystem.Tests
{
    [TestClass]
    public class DataProviderTests
    {
        private readonly IOrdersDataProvider dataProvider = new SqlDataProvider();
        private readonly OrderItem orderItem1 = new OrderItem()
        {
            StockItemId = 2,
            Description = "Chair",
            Price = 25,
            Quantity = 5,
            OnBackOrder = 0
        };
        private readonly OrderItem orderItem2 = new OrderItem()
        {
            StockItemId = 1,
            Description = "Table",
            Price = 100,
            Quantity = 5,
            OnBackOrder = 0
        };
        private readonly OrderItem orderItem3 = new OrderItem()
        {
            StockItemId = 1,
            Description = "Table",
            Price = 100,
            Quantity = 5,
            OnBackOrder = 0
        };
        /// <summary>
        /// Confirms that the GetStockItems method returns a List of StockItems from the Database.
        /// </summary>
        [TestMethod]
        public void GetStockItems_ReturnsListOfStockItems()
        {
            // Arrange
            List<StockItem> propertyStockItems = dataProvider.StockItems;
            // Act
            List<StockItem> testStockItems = dataProvider.GetStockItems();
            // Assert
            Assert.IsNotNull(testStockItems);
            Assert.IsTrue(propertyStockItems.Count == testStockItems.Count);
        }

        /// <summary>
        /// Confirms that the GetOrders method returns a List of Orders.
        /// </summary>
        [TestMethod]
        public void GetOrders_ReturnsListOfAllOrdersInDB()
        {
            // Arrange
            List<Order> propertyOrders = dataProvider.Orders;
            // Act
            List<Order> testOrders = dataProvider.GetOrders();
            // Assert
            Assert.IsTrue(testOrders.Count == propertyOrders.Count);
        }

        /// <summary>
        /// Confirms that the GetOrder method creates a "New" Order and returns it.
        /// </summary>
        [TestMethod]
        public void GetOrder_CreatesAndReturnsNewOrder()
        {
            // Arrange
            int beforeCount = dataProvider.CountAllOrderHeaders();
            // Act
            Order order = dataProvider.GetOrder();
            int afterCount = dataProvider.CountAllOrderHeaders();
            order = dataProvider.GetOrderById(order.Id);
            // Assert
            Assert.IsTrue(order.OrderStatus == "New" &&
                afterCount == beforeCount + 1);
            // Clean
            dataProvider.DeleteOrder(order.Id);
        }

        /// <summary>
        /// Confirms that the DeleteOrder method deletes an Order with a given Id.
        /// </summary>
        [TestMethod]
        public void DeleteOrder_DeletesTheOrder()
        {
            // Arrange
            Order order = dataProvider.GetOrder();
            int beforeCount = dataProvider.CountAllOrderHeaders();
            // Act
            dataProvider.DeleteOrder(order.Id);
            int afterCount = dataProvider.CountAllOrderHeaders();
            // Assert
            Assert.IsTrue(afterCount == beforeCount - 1);
        }

        /// <summary>
        /// Confirms that the UpdateOrInsertOrderItem method adds the given OrderItem to the Order if the
        /// given item does not already exists in the Order and updates the stock.
        /// </summary>
        [TestMethod]
        public void UpdateOrInsertOrderItem_InsertsNewItemAndUpdatesStock()
        {
            // Arrange
            Order order = dataProvider.GetOrder();
            int itemsInOrderBefore = order.ItemsCount;
            int inStockBefore = dataProvider.GetStockItembyId(2).InStock;
            orderItem1.OrderHeaderId = order.Id;
            // Act
            dataProvider.UpdateOrInsertOrderItem(orderItem1);
            order = dataProvider.GetOrderById(order.Id);
            int inStockAfter = dataProvider.GetStockItembyId(orderItem1.StockItemId).InStock;
            // Assert
            Assert.IsTrue(order.ItemsCount == itemsInOrderBefore + 1);
            Assert.IsTrue(inStockAfter == inStockBefore - orderItem1.Quantity);
            // Clean
            dataProvider.ReturnStockItems(order.OrderItems);
            dataProvider.DeleteOrder(order.Id);

        }
        /// <summary>
        /// Confirms that the UpdateOrInsertOrderItem method updates the given 
        /// OrderItem quantity and the stock.
        /// </summary>
        [TestMethod]
        public void UpdateOrInsertOrderItem_UpdatesItemAndStock()
        {
            // Arrange
            Order order = dataProvider.GetOrder();
            int inStockBefore = dataProvider.GetStockItembyId(1).InStock;

            orderItem2.OrderHeaderId = order.Id;
            orderItem3.OrderHeaderId = order.Id;

            dataProvider.UpdateOrInsertOrderItem(orderItem2);
            int itemsInOrderBefore = dataProvider.GetOrderById(order.Id).ItemsCount;

            // Act
            dataProvider.UpdateOrInsertOrderItem(orderItem3);
            order = dataProvider.GetOrderById(order.Id);
            int inStockAfter = dataProvider.GetStockItembyId(1).InStock;

            // Assert
            Assert.AreEqual(itemsInOrderBefore, order.ItemsCount);
            Assert.IsTrue(order.OrderItems[0].Quantity == orderItem2.Quantity + orderItem3.Quantity);
            Assert.IsTrue(inStockAfter == inStockBefore - (orderItem2.Quantity + orderItem3.Quantity));

            // Clean 
            dataProvider.ReturnStockItems(order.OrderItems);
            dataProvider.DeleteOrder(order.Id);
        }

        /// <summary>
        /// Confirms that when the RemoveOrderItem method is called the given OrderItem
        /// is removed from the Order and the stock is updated.
        /// </summary>
        [TestMethod]
        public void RemoveOrderItem_RemovesOrderItemAndUpdatesStock()
        {
            // Arrange
            int inStockBefore = dataProvider.GetStockItembyId(2).InStock;

            Order order = dataProvider.GetOrder();
            orderItem1.OrderHeaderId = order.Id;
            orderItem2.OrderHeaderId = order.Id;
            dataProvider.UpdateOrInsertOrderItem(orderItem1);
            dataProvider.UpdateOrInsertOrderItem(orderItem2);
            int itemsInOrderBefore = dataProvider.GetOrderById(order.Id).ItemsCount;

            // Act
            dataProvider.RemoveOrderItem(orderItem1);

            // Assert
            Assert.IsTrue(dataProvider.GetOrderById(order.Id).ItemsCount == itemsInOrderBefore - 1);
            Assert.IsTrue(dataProvider.GetStockItembyId(2).InStock == inStockBefore);

            // Clean 
            dataProvider.ReturnStockItems(dataProvider.GetOrderById(order.Id).OrderItems);
            dataProvider.DeleteOrder(order.Id);
        }

        /// <summary>
        /// Confirms that the UpdateOrderState changes the state of an order 
        /// in the database.
        /// </summary>
        [TestMethod]
        public void UpdateOrderState_ChangesOrderStatus()
        {
            // Arrange
            Order order = dataProvider.GetOrder();
            orderItem1.OrderHeaderId = order.Id;
            dataProvider.UpdateOrInsertOrderItem(orderItem1);
            string startingStatus = order.OrderStatus;
            // Act
            dataProvider.UpdateOrderState(order.Id, 2);
            // Assert
            Assert.IsTrue(startingStatus == "New");
            Assert.IsTrue(dataProvider.GetOrderById(order.Id).OrderStatus == "Pending");

            // Act
            dataProvider.UpdateOrderState(order.Id, 3);
            // Assert
            Assert.IsTrue(dataProvider.GetOrderById(order.Id).OrderStatus == "Rejected");

            // Act
            dataProvider.UpdateOrderState(order.Id, 4);
            // Assert
            Assert.IsTrue(dataProvider.GetOrderById(order.Id).OrderStatus == "Complete");

            // Clean 
            dataProvider.ReturnStockItems(dataProvider.GetOrderById(order.Id).OrderItems);
            dataProvider.DeleteOrder(order.Id);
        }
    }
}
