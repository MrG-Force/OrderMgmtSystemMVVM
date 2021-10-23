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
        /// Gets a sample list of 9 StockItems with a InStock property of 10.
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
        public Order GetOrder()
        {
            Order order = new Order();
            return order;
        }

        public void ReturnStockItems(List<OrderItem> orderItems)
        {
            return;
        }

        #region Not implemented Interface methods
        public int CountAllOrderHeaders()
        {
            return 0;
        }

        public void DeleteOrder(int orderId)
        {
            return;
        }

        public void RemoveOrderItem(OrderItem orderItem)
        {
            return;
        }

        public Order GetOrderById(int OrderId)
        {
            return null;
        }

        public StockItem GetStockItembyId(int itemId)
        {
            return null;
        }

        public void UpdateOrInsertOrderItem(OrderItem orderItem)
        {
            return;
        }

        public void UpdateOrderState(int orderId, int stateId)
        {
            return;
        }

        public void RevertChangesInOrderItems(List<OrderItem> originalList)
        {
            return;
        }
        #endregion
        #endregion
    }
}
