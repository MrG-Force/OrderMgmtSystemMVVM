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
        void AddNewOrder(Order newOrder);
        int StartNewOrder();
        void InsertOrderItem();
        // -- READ
        List<Order> GetOrders();
        List<StockItem> GetStockItems();
        Order GetOrder();
        string GetStatusString();
        Order GetOrderById();
        StockItem GetStockItembyId();
        // -- UPDATE
        void UpdateOrderState(int orderId, int stateId);
        void UpdateStockItemAmount();
        void UpdateOrInsertOrderItem(OrderItem orderItem, bool exists);
        // -- DELETE
        void DeleteOrder(int orderId);
        void RemoveOrderItem(OrderItem item);

    }
}
