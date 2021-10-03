using DataModels;
using DataProvider;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SQLDataProvider
{
    /// <summary>
    /// Defines a class to perform CRUD operations on the database and provide the
    /// required data for the application.
    /// </summary>
    public class SqlDataProvider : IOrdersDataProvider
    {
        public SqlDataProvider()
        {
            StockItems = GetStockItems();
            Orders = GetOrders();
        }

        public List<Order> Orders { get; }
        public List<StockItem> StockItems { get; }


        /// <summary>
        /// Gets the list of StockItems from the database.
        /// </summary>
        /// <returns>The list with the current inventory of StockItems</returns>
        public List<StockItem> GetStockItems()
        {
            SqlServerDataAccess.OpenConnection();
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("[sp_SelectStockItems]");
            List<StockItem> stockItems = new List<StockItem>();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    StockItem item = new StockItem()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        InStock = reader.GetInt32(3)
                    };
                    stockItems.Add(item);
                }
            }
            SqlServerDataAccess.CloseConnection();

            return stockItems;
        }

        /// <summary>
        /// Gets all the submitted orders currently in the database.
        /// </summary>
        /// <remarks>Only orders with OrderItems can be submitted</remarks>
        /// <returns></returns>
        public List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>();
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("sp_SelectOrderHeaderIdDateState");

            SqlServerDataAccess.OpenConnection();
            
            using(SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Order order = new Order(
                        reader.GetInt32(0),
                        reader.GetDateTime(1),
                        reader.GetInt32(2));
                    orders.Add(order);
                }
            }
            // Change StorePocedure
            command.CommandText = "sp_SelectOrderHeaderById";
            _ = command.Parameters.Add("@id", System.Data.SqlDbType.Int);
            // Populate the orderItems list in each order.
            foreach (Order o in orders)
            {
                command.Parameters[0].Value = o.Id;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OrderItem orderItem = new OrderItem()
                        {
                            OrderHeaderId = reader.GetInt32(0),
                            StockItemId = reader.GetInt32(3),
                            Description = reader.GetString(4),
                            Price = reader.GetDecimal(5),
                            Quantity = reader.GetInt32(6)
                        };
                        o.AddItem(orderItem);
                    }
                }
            }
            SqlServerDataAccess.CloseConnection();
            SqlServerDataAccess.ClearCommandParams();
            return orders;
        }

        public Order GetOrder()
        {
            SqlServerDataAccess.OpenConnection();
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("sp_InsertOrderHeader_V3");
            Order newOrder = null;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    newOrder = new Order(reader.GetInt32(0), reader.GetDateTime(1), 1);
                    Debug.WriteLine(newOrder.HasItemsOnBackOrder);
                    Debug.WriteLine(newOrder.DateTime);
                }
            }
            SqlServerDataAccess.CloseConnection();
            return newOrder;
        }

        /// <summary>
        /// Deletes the order with the passed Id from the database.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteOrder(int orderId)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("sp_DeleteOrderHeaderAndOrderItems");
            _ = command.Parameters.AddWithValue("@orderHeaderId", orderId);

            SqlServerDataAccess.OpenConnection();

            int rowsAffected = command.ExecuteNonQuery();
            Debug.WriteLine($"Rows affected:{rowsAffected}");

            SqlServerDataAccess.CloseConnection();
            SqlServerDataAccess.ClearCommandParams();
        }

        /// <summary>
        /// Removes the passed OrderItem from the Database and updates the corresponding StockItem.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveOrderItem(OrderItem orderItem)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("DeleteOrUpdateOrderItemAndUpdateStock");
            _ = command.Parameters.AddWithValue("@orderHeaderId",orderItem.OrderHeaderId);
            _ = command.Parameters.AddWithValue("@quantity", orderItem.Quantity - orderItem.OnBackOrder);
            _ = command.Parameters.AddWithValue("@stockItemId", orderItem.StockItemId);

            SqlServerDataAccess.OpenConnection();

            int rowsAffected = command.ExecuteNonQuery();
            Debug.WriteLine($"Records updated:{rowsAffected}");
            Debug.WriteLine($"OrderItem with StockItemId :{orderItem.StockItemId}, and Quantity: {orderItem.Quantity - orderItem.OnBackOrder} was deleted or updated in DB");

            SqlServerDataAccess.CloseConnection();
            SqlServerDataAccess.ClearCommandParams();
        }


        /// <summary>
        /// Updates the quantity of an OrderItem if the item already exists in the Order,
        /// otherwise adds the new OrderItem to the Order.
        /// </summary>
        /// <param name="orderItem"></param>
        /// <param name="orderItemExists"></param>
        public void UpdateOrInsertOrderItem(OrderItem orderItem)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("UpsertOrderItemAndUpdateStock");
            _ = command.Parameters.AddWithValue("@orderHeaderId", orderItem.OrderHeaderId);
            _ = command.Parameters.AddWithValue("@stockItemId", orderItem.StockItemId);
            _ = command.Parameters.AddWithValue("@description", orderItem.Description);
            _ = command.Parameters.AddWithValue("@price", orderItem.Price);
            _ = command.Parameters.AddWithValue("@quantity", orderItem.Quantity - orderItem.OnBackOrder);

            SqlServerDataAccess.OpenConnection();

            int rowsAffected = command.ExecuteNonQuery();
            Debug.WriteLine($"Records updated:{rowsAffected}");

            SqlServerDataAccess.CloseConnection();
            SqlServerDataAccess.ClearCommandParams();
        }

        /// <summary>
        /// Updates the state of the order with the passed Id to the passed StateId.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="stateId"></param>
        public void UpdateOrderState(int orderId, int stateId)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("sp_UpdateOrderState");
            _ = command.Parameters.AddWithValue("@orderHeaderId", orderId);
            _ = command.Parameters.AddWithValue("@stateId", stateId);

            SqlServerDataAccess.OpenConnection();

            _ = command.ExecuteNonQuery();

            SqlServerDataAccess.CloseConnection();
            SqlServerDataAccess.ClearCommandParams();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="orderItems"></param>
        public void ReturnStockItems(List<OrderItem> orderItems)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("sp_UpdateStockItemAmount");
            _ = command.Parameters.Add("@id", System.Data.SqlDbType.Int);
            _ = command.Parameters.Add("@amount", System.Data.SqlDbType.Int);

            SqlServerDataAccess.OpenConnection();

            foreach (var item in orderItems)
            {
                command.Parameters["@id"].Value = item.StockItemId;
                command.Parameters["@amount"].Value = item.Quantity - item.OnBackOrder;
                _ = command.ExecuteNonQuery();
            }

            SqlServerDataAccess.CloseConnection();
            SqlServerDataAccess.ClearCommandParams();
        }

        public void UpdateOrderItems(List<OrderItem> updatedItems)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("RevertUpdatedOrderItemAndUpdateStock");
            _ = command.Parameters.Add("@orderHeaderId", System.Data.SqlDbType.Int);
            _ = command.Parameters.Add("@stockItemId", System.Data.SqlDbType.Int);
            _ = command.Parameters.Add("@quantity", System.Data.SqlDbType.Int);

            SqlServerDataAccess.OpenConnection();

            foreach (var item in updatedItems)
            {
                command.Parameters["@orderHeaderId"].Value = item.OrderHeaderId;
                command.Parameters["@stockItemId"].Value = item.StockItemId;
                command.Parameters["@quantity"].Value = item.Quantity - item.OnBackOrder;
                _ = command.ExecuteNonQuery();
            }

            SqlServerDataAccess.CloseConnection();
            SqlServerDataAccess.ClearCommandParams();
        }

        public void RevertChangesInOrderItems(List<OrderItem> originalList)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("RevertChangesOnOrderItemAndUpdateStock2");// CHANGE this SP name
            _ = command.Parameters.Add("@orderHeaderId", System.Data.SqlDbType.Int);
            _ = command.Parameters.Add("@stockItemId", System.Data.SqlDbType.Int);
            _ = command.Parameters.Add("@description", System.Data.SqlDbType.VarChar);
            _ = command.Parameters.Add("@price", System.Data.SqlDbType.Decimal);
            _ = command.Parameters.Add("@oldQuantity", System.Data.SqlDbType.Int);

            SqlServerDataAccess.OpenConnection();
            try
            {
                foreach (var item in originalList)
                {
                    command.Parameters["@orderHeaderId"].Value = item.OrderHeaderId;
                    command.Parameters["@stockItemId"].Value = item.StockItemId;
                    command.Parameters["@description"].Value = item.Description;
                    command.Parameters["@price"].Value = item.Price;
                    command.Parameters["@oldQuantity"].Value = item.Quantity;
                    _ = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Execute non-query failed: " + ex.Message);
            }
            finally
            {
                SqlServerDataAccess.CloseConnection();
            }

            SqlServerDataAccess.ClearCommandParams();
        }

        #region Not yet implemented


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

        public void InsertOrderItem()
        {
            throw new NotImplementedException();
        }

        public void UpdateStockItemAmount()
        {
            throw new NotImplementedException();
        }

        public void AddNewOrder(Order newOrder)
        {
            throw new NotImplementedException();
        }


        #endregion
        /// <summary>
        /// DEPRECATED.
        /// </summary>
        /// <param name="orderItem"></param>
        /// <param name="orderItemExists"></param>
        public void UpdateOrInsertOrderItem(OrderItem orderItem, bool orderItemExists)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("");
            _ = command.Parameters.AddWithValue("@orderHeaderId", orderItem.OrderHeaderId);
            _ = command.Parameters.AddWithValue("@stockItemId", orderItem.StockItemId);
            _ = command.Parameters.AddWithValue("@quantity", orderItem.Quantity);

            SqlServerDataAccess.OpenConnection();
            if (orderItemExists)
            {
                command.CommandText = "UpdateOrderItemAndUpdateStock";
                int rowsAffected = command.ExecuteNonQuery();
                Debug.WriteLine($"Records updated:{rowsAffected}");
            }
            else
            {
                command.CommandText = "InsertOrderItemAndUpdateStock";
                _ = command.Parameters.AddWithValue("@description", orderItem.Description);
                _ = command.Parameters.AddWithValue("@price", orderItem.Price);
                int rowsAffected = command.ExecuteNonQuery();
                Debug.WriteLine($"Records updated:{rowsAffected}");
            }
            SqlServerDataAccess.CloseConnection();
            SqlServerDataAccess.ClearCommandParams();
        }
    }
}
