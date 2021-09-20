using DataModels;
using DataProvider;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SQLDataProvider
{
    /// <summary>
    /// Defines a class to perform CRUD operations on the database and expose the
    /// required data for the application.
    /// </summary>
    public class SqlDataProvider : IOrdersDataProvider
    {
        public List<Order> Orders => throw new NotImplementedException();

        public List<StockItem> StockItems => throw new NotImplementedException();

        public int StartNewOrder()
        {
            SqlServerDataAccess.OpenConnection();
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("[sp_InsertOrderHeader]");
            SqlDataReader reader = command.ExecuteReader();
            int Id = 0;
            while (reader.Read())
            {
                Id = reader.GetInt32(0);
            }
            reader.Dispose();
            SqlServerDataAccess.CloseConnection();
            return Id;
        }
        public List<StockItem> GetStockItems()
        {
            List<StockItem> stockItems = new List<StockItem>();

            SqlServerDataAccess.OpenConnection();
            SqlCommand command = SqlServerDataAccess.GetSqlCommand("[sp_SelectStockItems]");
            SqlDataReader reader = command.ExecuteReader();
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
            SqlServerDataAccess.CloseConnection();
            return stockItems;
        }
        #region Not yet implemented
        public void DeleteOrder()
        {
            throw new NotImplementedException();
        }

        public void DeleteOrderItem()
        {
            throw new NotImplementedException();
        }

        public Order GetOrder()
        {
            throw new NotImplementedException();
        }

        public Order GetOrderById()
        {
            throw new NotImplementedException();
        }

        public List<Order> GetOrders()
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
