using DataModels;
using System.Collections.Generic;

namespace DataProvider
{
    /// <summary>
    /// A class that provides simple predictable data for testing.
    /// </summary>
    public class TestDataProvider : IOrdersDataProvider
    {
        #region Constructor
        public TestDataProvider()
        {
            _stockItems = GetStockItems();
            _orders = GetOrders();
        }
        #endregion

        #region Fields
        private List<StockItem> _stockItems;
        private List<Order> _orders;

        private readonly string[] StockItemNames = new string[] { "Table","Chair", "Sofa", "Wardrobe", "Cupboard",
            "Single Bed", "Double Bed", "Queen Bed","King Bed" };
        private readonly decimal[] StockItemPrices = new decimal[] { 100, 25, 250, 180, 65, 120, 180, 220, 320 };
        private readonly int[] StockItemStocks = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10 };
        #endregion

        #region Properties
        public List<Order> Orders => _orders;
        public List<StockItem> StockItems => _stockItems;
        #endregion

        #region Methods
        /// <summary>
        /// Gets a sample list of StockItems for testing.
        /// </summary>
        /// <returns></returns>
        public List<StockItem> GetStockItems()
        {
            List<StockItem> stockItems = new List<StockItem>();
            for (int i = 0; i < 9; i++)
            {
                StockItem stockItem = new StockItem()
                {
                    Id = i + 1,
                    Name = StockItemNames[i],
                    Price = StockItemPrices[i],
                    InStock = StockItemStocks[i]
                };
                stockItems.Add(stockItem);
            }
            return stockItems;
        }

        /// <summary>
        /// Returns a sample list of 5 OrderItems for testing.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private List<OrderItem> GetOrderItems(int orderId)
        {
            List<OrderItem> orderItems = new List<OrderItem>(5);
            for (int i = 0; i < 5; i++)
            {
                OrderItem item = new OrderItem()
                {
                    OrderHeaderId = orderId,
                    StockItemId = i + 1,
                    Description = StockItemNames[i],
                    Price = StockItemPrices[i],
                    Quantity = 10,
                    OnBackOrder = 0
                };
                orderItems.Add(item);
            }
            return orderItems;
        }

        /// <summary>
        /// Returns a list with 10 sample pending orders.
        /// </summary>
        /// <returns></returns>
        public List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>(10);
            for (int i = 0; i < 10; i++)
            {
                Order order = new Order(i + 1001)
                {
                    OrderStateId = 2,
                    OrderItems = GetOrderItems(i + 1001)
                };
                orders.Add(order);
            }
            return orders;
        }

        public void ReturnStockItems(List<OrderItem> orderItems)
        {
            return;
        }

        #region Not implemented Interface methods
        public void AddNewOrder(Order newOrder)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteOrder(int orderId)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveOrderItem(OrderItem orderItem)
        {
            return;
        }

        public Order GetOrder()
        {
            Order order = new Order();
            return order;
        }

        public Order GetOrderById()
        {
            throw new System.NotImplementedException();
        }


        public string GetStatusString()
        {
            throw new System.NotImplementedException();
        }

        public StockItem GetStockItembyId()
        {
            throw new System.NotImplementedException();
        }


        public void InsertOrderItem()
        {
            throw new System.NotImplementedException();
        }

        public int StartNewOrder()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateOrderItems(List<OrderItem> updatedItems)
        {
            return;
        }

        public void UpdateOrInsertOrderItem(OrderItem orderItem)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateOrderState(int orderId, int stateId)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateStockItemAmount()
        {
            throw new System.NotImplementedException();
        }

        public void RevertChangesInOrderItems(List<OrderItem> originalList)
        {
            throw new System.NotImplementedException();
        }
        #endregion
        #endregion
    }
}
