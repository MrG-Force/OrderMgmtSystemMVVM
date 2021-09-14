using DataModels;
using DataProvider;
using SQLDataProvider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace DataTester
{
    internal class DummyDataTester
    {
        static void Main(string[] args)
        {
            #region Random data tester
            //IOrdersDataProvider dataProvider = new RandomDataProvider();
            ////List<Order> orders = dataProvider.GetOrders(15);
            //List<Order> orders = dataProvider.Orders; // using a property that retrieves data from static field

            //foreach (var order in orders)
            //{
            //    Console.WriteLine();
            //    Console.WriteLine($"  ====================" +
            //        $"\tOrder ID : {order.Id}\t====================\n\t" +
            //        $"Products in this order: {order.ItemsCount}\n\t" +
            //        $"Date of order: {order.DateTime.ToLongDateString()}\n\t" +
            //        $"Order status: {order.OrderStatus}\n\t" +
            //        $"Total: {order.Total:C}\n\t" +
            //        $"In this order:\n\t\t");
            //    List<OrderItem> orderItems = order.OrderItems;
            //    foreach (var orderItem in orderItems)
            //    {
            //        Console.WriteLine($"\tSKU: {orderItem.StockItemId}" +
            //            $"\t| Product: {orderItem.Description}\n" +
            //            $"\tPrice: {orderItem.Price:C}\t| Quantity: {orderItem.Quantity}\n" +
            //            $"\t---------------------------------------------");
            //    }
            //    Console.WriteLine("Press any key to see next record...");
            //    Console.ReadKey(true);
            //}
            #endregion

            //List<StockItem> stockItems = new List<StockItem>();

            //ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["OrdersMgmtConnectionString"];
            //SqlConnection connection = new SqlConnection(settings.ConnectionString);
            //using (connection)
            //{
            //    connection.Open();
            //    SqlCommand command = new SqlCommand("[sp_SelectStockItems]", connection);
            //    SqlDataReader reader = command.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        StockItem item = new StockItem()
            //        {
            //            Id = reader.GetInt32(0),
            //            Name = reader.GetString(1),
            //            Price = reader.GetDecimal(2),
            //            InStock = reader.GetInt32(3)
            //        };
            //        stockItems.Add(item);
            //    }
            //}
            //foreach (var item in stockItems)
            //{
            //    Console.WriteLine(item);
            //}

            SqlDataTester.OpenConnection();
            SqlDataTester.PrintItems();
            SqlDataTester.CloseConnection();

            SqlDataProvider dataProvider = new SqlDataProvider();
            List<StockItem> items = dataProvider.GetStockItems();
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("Press any key to finish...");
            Console.ReadKey(true);

        }
    }
}
