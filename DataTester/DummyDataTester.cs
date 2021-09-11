using DataModels;
using DataProvider;
using System;
using System.Collections.Generic;

namespace DataTester
{
    internal class DummyDataTester
    {
        static void Main(string[] args)
        {
            RandomDataProvider dataProvider = new RandomDataProvider();
            //List<Order> orders = dataProvider.GetOrders(15);
            List<Order> orders = dataProvider.Orders; // using a property that retrieves data from static field
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
            Console.WriteLine("Press any key to finish...");
            Console.ReadKey(true);
        }
    }
}
