using DataModels;
using System.Collections.Generic;

namespace DataProvider
{
    /// <summary>
    /// Provides data objects for the application.
    /// </summary>
    public interface IOrdersDataProvider
    {
        List<Order> Orders { get; }
        List<StockItem> StockItems { get; }
        List<Order> GetOrders();
        List<StockItem> GetStockItems();
        Order GetOrder();
    }
}
