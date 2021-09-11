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
        //-- CREATE
        void AddNewOrder();
        void InsertOrderItem();
        // -- READ
        List<Order> GetOrders();
        List<StockItem> GetStockItems();
        Order GetOrder();
        string GetStatusString();
        Order GetOrderById();
        StockItem GetStockItembyId();
        // -- UPDATE
        void UpdateOrderState();
        void UpdateStockItemAmount();
        void UpdateOrderItem();
        // -- DELETE
        void DeleteOrder();
        void DeleteOrderItem();

    }
}
