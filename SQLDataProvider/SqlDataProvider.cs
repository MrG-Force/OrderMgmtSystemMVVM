using DataModels;
using DataProvider;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SQLDataProvider
{
    /// <summary>
    /// Defines a class to perform CRUD operations on the database and expose the
    /// required data for the application.
    /// </summary>
    public class SqlDataProvider : IOrdersDataProvider
    {
        public SqlDataProvider()
        {
            _stockItems = GetStockItems();
            _orders = GetOrders();
        }

        private List<StockItem> _stockItems;
        private List<Order> _orders;


        public List<Order> Orders => _orders;
        public List<StockItem> StockItems => _stockItems;

        public int StartNewOrder()
        {
            SqlServerDataAccess.OpenConnection();
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("[sp_InsertOrderHeader]");
            int Id = 0;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Id = reader.GetInt32(0);
                }
            }
            SqlServerDataAccess.CloseConnection();
            return Id;
        }
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

        public List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>();

            SqlServerDataAccess.OpenConnection();
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("sp_SelectOrderHeaderIdDateState");
            // Create empty orders and add them to the orders list
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
            // Confgure command for a diferent sp that takes params
            command = SqlServerDataAccess.GetSqlCommand("sp_SelectOrderHeaderById");
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add("@id", System.Data.SqlDbType.Int);
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
            command.Parameters.Clear();
            SqlServerDataAccess.CloseConnection();
            return orders;
        }
        public Order GetOrder()
        {
            SqlServerDataAccess.OpenConnection();
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("[sp_InsertOrderHeader_V3]");
            command.CommandType = System.Data.CommandType.StoredProcedure;
            Order newOrder = null;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    newOrder = new Order(reader.GetInt32(0), reader.GetDateTime(1), 1);
                }
            }
            SqlServerDataAccess.CloseConnection();
            return newOrder;
        }
        public void DeleteOrder(int id) // May need the Order as parameter to return Items to stock
        {
            // and maybe here add a check if order hast items
            SqlServerDataAccess.OpenConnection();
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("[sp_DeleteOrderHeaderAndOrderItems]");
            _ = command.Parameters.AddWithValue("@orderHeaderId", id);
            int rowsAffected = command.ExecuteNonQuery();
            Debug.WriteLine(rowsAffected);
            command.Parameters.Clear();
        }



        #region Not yet implemented

        public void DeleteOrderItem()
        {
            throw new NotImplementedException();
        }


        public Order GetOrderById()
        {
            throw new NotImplementedException();
        }


        public string GetStatusString()
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

        public void UpdateOrderItem()
        {
            throw new NotImplementedException();
        }

        public void UpdateOrderState()
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
    }
}
