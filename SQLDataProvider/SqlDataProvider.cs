using DataModels;
using DataProvider;
using System;
using System.Collections.Generic;
using System.Data;
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
        #region Constructor
        public SqlDataProvider()
        {
            StockItems = GetStockItems();
            Orders = GetOrders();
        }
        #endregion

        #region Properties
        public List<Order> Orders { get; }
        public List<StockItem> StockItems { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the list of StockItems from the database.
        /// </summary>
        /// <returns>The list with the current inventory of StockItems</returns>
        public List<StockItem> GetStockItems()
        {
            List<StockItem> stockItems = new List<StockItem>();
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("[sp_SelectStockItems]");

            SqlServerDataAccess.OpenConnection();
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StockItem item = new StockItem()
                        {
                            Id = reader.GetFieldValue<int>(0),
                            Name = reader.GetFieldValue<string>(1),
                            Price = reader.GetFieldValue<decimal>(2),
                            InStock = reader.GetFieldValue<int>(3)
                        };
                        stockItems.Add(item);
                    }
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
            return stockItems;
        }

        /// <summary>
        /// Gets the StockItem with the given Id.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public StockItem GetStockItembyId(int itemId)
        {
            StockItem stockItem = null;
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("sp_SelectStockItemById");
            _ = command.Parameters.AddWithValue("@id", itemId);

            SqlServerDataAccess.OpenConnection();
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stockItem = new StockItem()
                        {
                            Id = reader.GetFieldValue<int>(0),
                            Name = reader.GetFieldValue<string>(1),
                            Price = reader.GetFieldValue<decimal>(2),
                            InStock = reader.GetFieldValue<int>(3)
                        };
                    }
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
            return stockItem;
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
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order order = new Order(
                            reader.GetFieldValue<int>(0),
                            reader.GetFieldValue<DateTime>(1),
                            reader.GetFieldValue<int>(2));
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
                                OrderHeaderId = reader.GetFieldValue<int>(0),
                                StockItemId = reader.GetFieldValue<int>(3),
                                Description = reader.GetFieldValue<string>(4),
                                Price = reader.GetFieldValue<decimal>(5),
                                Quantity = reader.GetFieldValue<int>(6)
                            };
                            o.AddItem(orderItem);
                        }
                    }
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
            return orders;
        }

        /// <summary>
        /// Starts a new order in the database and returns a new empty Order object with the current date
        /// and the corresponding OrderId number.
        /// </summary>
        /// <returns></returns>
        public Order GetOrder()
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("my_InsertOrderHeader");
            Order newOrder = null;
            SqlServerDataAccess.OpenConnection();
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        newOrder = new Order(reader.GetFieldValue<int>(0), reader.GetFieldValue<DateTime>(1), 1);
                    }
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
            return newOrder;
        }

        /// <summary>
        /// Gets an Order object with the given id from the database.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Order GetOrderById(int Id)
        {
            Order order = null;
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("my_SelectAllOrderHeaderDetailsById");
            _ = command.Parameters.AddWithValue("@id", Id);

            SqlServerDataAccess.OpenConnection();
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        order = new Order(
                            reader.GetFieldValue<int>(0),
                            reader.GetFieldValue<DateTime>(1),
                            reader.GetFieldValue<int>(2));
                    }
                }

                command.CommandText = "sp_SelectOrderHeaderById";
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OrderItem orderItem = new OrderItem()
                        {
                            OrderHeaderId = reader.GetFieldValue<int>(0),
                            StockItemId = reader.GetFieldValue<int>(3),
                            Description = reader.GetFieldValue<string>(4),
                            Price = reader.GetFieldValue<decimal>(5),
                            Quantity = reader.GetFieldValue<int>(6)
                        };
                        order.AddItem(orderItem);
                    }
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
            return order;
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
            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                Debug.WriteLine($"Records updated:{rowsAffected}");
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

        /// <summary>
        /// Updates the quantity of an OrderItem if the item already exists in the Order,
        /// otherwise adds the new OrderItem to the Order.
        /// </summary>
        /// <param name="orderItem"></param>
        /// <param name="orderItemExists"></param>
        public void UpdateOrInsertOrderItem(OrderItem orderItem)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("my_UpsertOrderItemAndUpdateStock");
            _ = command.Parameters.AddWithValue("@orderHeaderId", orderItem.OrderHeaderId);
            _ = command.Parameters.AddWithValue("@stockItemId", orderItem.StockItemId);
            _ = command.Parameters.AddWithValue("@description", orderItem.Description);
            _ = command.Parameters.AddWithValue("@price", orderItem.Price);
            _ = command.Parameters.AddWithValue("@quantity", orderItem.Quantity - orderItem.OnBackOrder);

            SqlServerDataAccess.OpenConnection();
            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"Records updated:{rowsAffected}");
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

        /// <summary>
        /// Removes the passed OrderItem from the Database and updates the corresponding StockItem.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveOrderItem(OrderItem orderItem)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("my_DeleteOrUpdateOrderItemAndUpdateStock");
            _ = command.Parameters.AddWithValue("@orderHeaderId", orderItem.OrderHeaderId);
            _ = command.Parameters.AddWithValue("@quantity", orderItem.Quantity - orderItem.OnBackOrder);
            _ = command.Parameters.AddWithValue("@stockItemId", orderItem.StockItemId);

            SqlServerDataAccess.OpenConnection();
            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                Debug.WriteLine($"Records updated:{rowsAffected}");
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
            try
            {
                _ = command.ExecuteNonQuery();
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

        /// <summary>
        /// Returns the given OrderItems back into the StockItems list.
        /// </summary>
        /// <remarks>It is used when an order with OrderItems is deleted or rejected.</remarks>
        /// <param name="orderItems"></param>
        public void ReturnStockItems(List<OrderItem> orderItems)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("sp_UpdateStockItemAmount");
            _ = command.Parameters.Add("@id", SqlDbType.Int);
            _ = command.Parameters.Add("@amount", SqlDbType.Int);

            SqlServerDataAccess.OpenConnection();
            try
            {
                foreach (var item in orderItems)
                {
                    command.Parameters["@id"].Value = item.StockItemId;
                    command.Parameters["@amount"].Value = item.Quantity - item.OnBackOrder;
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

        /// <summary>
        /// Restores the OrderItems list in an order to its previous state.
        /// </summary>
        /// <param name="originalList"></param>
        public void RevertChangesInOrderItems(List<OrderItem> originalList)
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("my_RevertChangesOnOrderItemAndUpdateStock");
            _ = command.Parameters.Add("@orderHeaderId", SqlDbType.Int);
            _ = command.Parameters.Add("@stockItemId", SqlDbType.Int);
            _ = command.Parameters.Add("@description", SqlDbType.VarChar);
            _ = command.Parameters.Add("@price", SqlDbType.Decimal);
            _ = command.Parameters.Add("@oldQuantity", SqlDbType.Int);

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

        /// <summary>
        /// Returns a count of all the OrderHeaders currently in the data base regardless
        /// they have OrderItems or not.
        /// </summary>
        /// <returns></returns>
        public int CountAllOrderHeaders()
        {
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("my_SelectCountAllOrderHeaders");
            command.Parameters.Add(new SqlParameter { ParameterName = "@Total", IsNullable = false, DbType = DbType.Int32, Direction = ParameterDirection.Output });

            SqlServerDataAccess.OpenConnection();
            try
            {
                _ = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Execute non-query failed: " + ex.Message);
            }
            finally
            {
                SqlServerDataAccess.CloseConnection();
            }

            int total = (int)command.Parameters["@Total"].Value;
            SqlServerDataAccess.ClearCommandParams();
            return total;
        }
        #endregion
    }
}
