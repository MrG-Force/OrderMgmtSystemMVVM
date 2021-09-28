using DataModels;
using DataProvider;
using SQLDataProvider;
using System;
using System.Collections.Generic;

namespace DataTester
{
    internal class DummyDataTester
    {
        static RandomDataProvider rndmDataProvider = new RandomDataProvider();
        static SqlDataProvider sqlDataProvider = new SqlDataProvider();
        static void Main(string[] args)
        {

            // DisplayRandomOrderList(rndmDataProvider);

            TestGetSTockItems();

            // Add Order
            //SqlDataTester.AddNewOrder();

            Console.WriteLine("Press any key to finish...");
            Console.ReadKey(true);

        }

        /// <summary>
        /// Displays a list of orders with details to the console.
        /// </summary>
        /// <remarks>This function is for testing the RandomDataProvider class</remarks>
        /// <param name="rndmDataProvider"></param>
        private static void DisplayRandomOrderList(RandomDataProvider rndmDataProvider)
        {
            //List<Order> orders = rndmDataProvider.GetOrders(15);
            List<Order> orders = rndmDataProvider.Orders; // using a property that retrieves data from static field
            foreach (var order in orders)
            {
                Console.WriteLine();
                Console.WriteLine($"  ====================" +
                    $"\tOrder ID : {order.Id}\t====================\n\t" +
                    $"Products in this order: {order.ItemsCount}\n\t" +
                    $"Date of order: {order.DateTime.ToLongDateString()}\n\t" +
                    $"Order status: {order.OrderStatus}\n\t" +
                    $"Total: {order.Total:C}\n\t" +
                    $"In this order:\n\t\t");
                List<OrderItem> orderItems = order.OrderItems;
                foreach (var orderItem in orderItems)
                {
                    Console.WriteLine($"\tSKU: {orderItem.StockItemId}" +
                        $"\t| Product: {orderItem.Description}\n" +
                        $"\tPrice: {orderItem.Price:C}\t| Quantity: {orderItem.Quantity}\n" +
                        $"\t---------------------------------------------");
                }
                Console.WriteLine("Press any key to see next record...");
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// A function to test the GetStockItems method in the data provider
        /// </summary>
        private static void TestGetSTockItems()
        {
            //using SqlDataTester
            SqlDataTester.GetDBStockItems();

            //using sqlDataProvider
            List<StockItem> stockItems = sqlDataProvider.GetStockItems();
            foreach (var item in stockItems)
            {
                Console.WriteLine(item);
            }
        }
    }
}