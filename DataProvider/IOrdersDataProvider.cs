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
        void InsertOrderItem();
        // -- READ
        List<Order> GetOrders();
        List<StockItem> GetStockItems();
        Order GetOrder();
        string GetStatusString();
        Order GetOrderById();
        StockItem GetStockItembyId();
        // -- UPDATE
        void ReturnStockItems(List<OrderItem> orderItems);
        void UpdateOrderState(int orderId, int stateId);
        void UpdateStockItemAmount();
        void UpdateOrInsertOrderItem(OrderItem orderItem);
        void UpdateOrderItems(List<OrderItem> updatedItems);
        void RevertChangesInOrderItems(List<OrderItem> originalList);
        // -- DELETE
        void DeleteOrder(int orderId);
        void RemoveOrderItem(OrderItem orderItem);

    }
}
