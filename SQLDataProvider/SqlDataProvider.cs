using DataModels;
using DataProvider;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void AddNewOrder()
        {
            throw new NotImplementedException();
        }

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

        public List<StockItem> GetStockItems()
        {
            List<StockItem> stockItems = new List<StockItem>();

            SQLServerDataAccess.OpenConnection();
            SqlCommand command = SQLServerDataAccess.GetSqlCommand("[sp_SelectStockItems]");
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
            SQLServerDataAccess.CloseConnection();
            return stockItems;
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
    }
}
