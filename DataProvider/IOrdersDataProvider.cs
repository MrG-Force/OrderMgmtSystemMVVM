using DataModels;
using System.Collections.Generic;

namespace DataProvider
{
    /// <summary>
    /// Provides data objects and CRUD methods for the application.
    /// </summary>
    public interface IOrdersDataProvider
    {
        List<Order> Orders { get; }
        List<StockItem> StockItems { get; }
        //-- CREATE
        int CountAllOrderHeaders();
        // -- READ
        List<Order> GetOrders();
        List<StockItem> GetStockItems();
        Order GetOrder();
        Order GetOrderById(int OrderId);
        StockItem GetStockItembyId(int itemId);
        // -- UPDATE
        void ReturnStockItems(List<OrderItem> orderItems);
        void UpdateOrderState(int orderId, int stateId);
        void UpdateOrInsertOrderItem(OrderItem orderItem);
        void RevertChangesInOrderItems(List<OrderItem> originalList);
        // -- DELETE
        void DeleteOrder(int orderId);
        void RemoveOrderItem(OrderItem orderItem);

    }
}
