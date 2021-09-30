using DataModels;
using System;
using System.Collections.Generic;

namespace DataProvider
{
    /// <summary>
    /// A class to supply sample random data and test the application.
    /// </summary>
    public class RandomDataProvider : IOrdersDataProvider
    {
        #region Constructor
        public RandomDataProvider()
        {
            _stockItems = GetStockItems();
            _dBStockItems = GetDBTestStockItems();
            _dbOrderItems = GetDbOrderItems(5);
            if (_orders == null)
            {
                _orders = GetOrders();
            }
        }
        #endregion

        #region Fields
        readonly Random random = new Random();
        private static List<StockItem> _stockItems;
        private static List<Order> _orders;
        private static List<StockItem> _dBStockItems;
        private static List<OrderItem> _dbOrderItems;

        private readonly string[] names = new string[] { "Ergonomic chair",
            "Wooden desk", "Dinning table", "Plain chair", "Chaise lounge",
            "Leather loveseat", "Night table", "Large Bookcase",
            "Chinese cupboard", "Acapulco chair" };
        private readonly decimal[] prices = new decimal[] { 56.90m, 34.80m, 124.80m, 69.99m, 78.60m, 210.80m, 78.99m, 46.79m, 67.89m, 54.40m };

        private readonly string[] badNames = new string[] { "Table",
            "Chair", "Sofa", "Wardrobe", "Cupboard",
            "Single Bed", "Double Bed", "Queen Bed",
            "King Bed" };
        private readonly decimal[] booringPrices = new decimal[] { 100, 25, 250, 180, 65, 120, 180, 220, 320 };

        // for generating random dates in the previous 14 days.
        private DateTime lowEndDate = DateTime.Today.AddDays(-14);
        #endregion

        #region Properties
        public List<Order> Orders => _orders;
        public List<StockItem> StockItems => _stockItems;
        public List<StockItem> DbStockItems => _dBStockItems;
        public List<OrderItem> DbOrderItems => _dbOrderItems;
        #endregion

        #region Methods
        public List<StockItem> GetDBTestStockItems()
        {
            List<StockItem> dBstockItems = new List<StockItem>();
            for (int i = 0; i < 9; i++)
            {
                StockItem stockItem = new StockItem()
                {
                    Id = i + 1,
                    Name = badNames[i],
                    Price = booringPrices[i],
                    InStock = i * 2
                };
                dBstockItems.Add(stockItem);
            }
            return dBstockItems;
        }

        /// <summary>
        /// Gets a new empty order with an autogeneretaed Id, the current
        /// DateTime, the status set to 'new' and an empty List of orderItems.
        /// </summary>
        /// <returns></returns>
        public Order GetOrder()
        {
            Order order = new Order();
            return order;
        }

        /// <summary>
        /// Generates a random list of 10 Orders.
        /// </summary>
        /// <returns></returns>
        public List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>(10);
            for (int i = 0; i < 10; i++)
            {
                int orderStateId = random.Next(2, 5);
                DateTime rndmDateTime = GetSampleDateTime(i);
                orders.Add(GetRandomOrder(orderStateId, rndmDateTime));
            }

            return orders;
        }

        /// <summary>
        /// Generates a list of 10 random stockItems.
        /// </summary>
        /// <returns></returns>
        public List<StockItem> GetStockItems()
        {
            List<StockItem> stockItems = new List<StockItem>(10);
            int idStart = random.Next(1000, 9999);
            for (int i = 0; i < 10; i++)
            {
                StockItem item = new StockItem()
                {
                    Id = idStart++,
                    Name = names[i],
                    Price = GetRandomItem(prices),
                    InStock = random.Next(30)
                };
                stockItems.Add(item);
            }
            return stockItems;
        }

        /// <summary>
        /// Submits the given order by adding it to the List and changing its state to 'Pending'.
        /// </summary>
        /// <param name="newOrder"></param>
        public static void SubmitThisOrder(Order newOrder)
        {
            newOrder.OrderStateId = 2;
            _orders.Add(newOrder);
        }

        /// <summary>
        /// Gets a random list of OrderItems.
        /// </summary>
        /// <returns></returns>
        public List<OrderItem> GetRndmOrderItems()
        {
            DateTime dateTime = DateTime.Now;
            Order order = GetRandomOrder(1, dateTime);
            return order.OrderItems;
        }

        private List<OrderItem> GetDbOrderItems(int size)
        {
            List<OrderItem> dbOrderItems = new List<OrderItem>();
            int[] indxs = GetRandomNumbers(size, 9);

            for (int i = 0; i < size; i++)
            {
                dbOrderItems.Add(new OrderItem(DbStockItems[indxs[i]])
                {
                    Quantity = indxs[i] + 1
                }
                );
            }
            return dbOrderItems;
        }

        /// <summary>
        /// Gets a random order item without the OrderHeaderId
        /// </summary>
        /// <returns></returns>
        public OrderItem GetRndmDbOrderItem()
        {
            return new OrderItem(GetRandomItem(DbStockItems.ToArray()));
        }

        /// <summary>
        /// Gets a random object T from an array T[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private T GetRandomItem<T>(T[] data)
        {
            return data[random.Next(0, data.Length)];
        }

        /// <summary>
        /// It provides a new Order with a given date and a given State. It populates 
        /// the Order's List of OrderItems with random stock item information.
        /// </summary>
        /// <param name="stateId">An integer representing the order status</param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public Order GetRandomOrder(int stateId, DateTime dateTime)
        {
            Order order = new Order(stateId, dateTime);
            int itemsInOrder = random.Next(1, 11);
            int[] indxs = GetRandomNumbers(itemsInOrder, 10);
            for (int i = 0; i < itemsInOrder; i++)
            {
                OrderItem item = new OrderItem(order.Id, _stockItems[indxs[i]])
                {
                    Quantity = random.Next(1, 20)
                };
                order.AddItem(item);
            }
            return order;
        }
        /// <summary>
        /// Gets consecutive dates that differ by roughly one day. It
        /// is used in conjunction with a for loop where the index 
        /// is passed as parameter to add one day in minutes to a starting day
        /// that is set in this class 14 days prior the currrent date.
        /// </summary>
        /// <remarks>If more than 14 orders are generated the method will
        /// provide dates in a future date, so be careful</remarks>
        /// <param name="dayIndex">Usually the current index of an iteration</param>
        /// <returns></returns>
        private DateTime GetSampleDateTime(int dayIndex)
        {
            return lowEndDate.AddMinutes((dayIndex * 1440) + random.Next(1440));
        }

        /// <summary>
        /// Generates and returns an array of non repeating random integers within the range 0 to maxValue.
        /// </summary>
        /// <param name="howMany"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public int[] GetRandomNumbers(int howMany, int maxValue)
        {
            List<int> numbers = new List<int>();
            int num;
            for (int i = 0; i < howMany; i++)
            {
                do
                {
                    num = random.Next(maxValue);
                } while (numbers.Contains(num));
                numbers.Add(num);
            }
            return numbers.ToArray();
        }
        #region Not implemented Interface methods
        public void AddNewOrder(Order newOrder)
        {
            throw new NotImplementedException();
        }

        public void InsertOrderItem()
        {
            throw new NotImplementedException();
        }

        public string GetStatusString()
        {
            throw new NotImplementedException();
        }

        public Order GetOrderById()
        {
            throw new NotImplementedException();
        }

        public StockItem GetStockItembyId()
        {
            throw new NotImplementedException();
        }

        public void UpdateOrderState(int orderId, int stateId)
        {
            throw new NotImplementedException();
        }

        public void UpdateStockItemAmount()
        {
            throw new NotImplementedException();
        }

        public void UpdateOrInsertOrderItem(OrderItem orderItem, bool exists)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public void RemoveOrderItem(int orderId, int qty, int itemId)
        {
            throw new NotImplementedException();
        }

        public int StartNewOrder()
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion

    }
}
