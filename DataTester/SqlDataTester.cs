using DataModels;
using DataProvider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DataTester
{
    public class SqlDataTester
    {
        public static ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["OrdersMgmtConnectionString"];
        public static SqlConnection conn = new SqlConnection(settings.ConnectionString);
        public static SqlCommand cmnd = new SqlCommand("", conn);
        public static RandomDataProvider randomData = new RandomDataProvider();
        public static List<StockItem> dBstockItems;
        public static List<OrderItem> DbOrderItems = randomData.DbOrderItems;

        public static List<StockItem> GetDBStockItems()
        {
            OpenConnection();
            List<StockItem> stockItems = new List<StockItem>();
            cmnd.CommandText = "[sp_SelectStockItems]";
            cmnd.Connection = conn;
            SqlDataReader reader = cmnd.ExecuteReader();
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
            foreach (var item in stockItems)
            {
                Console.WriteLine(item);
            }
            dBstockItems = stockItems;
            CloseConnection();
            return stockItems;
        }

        public static void AddNewOrder()
        {
            OpenConnection();
            cmnd.CommandText = "[sp_InsertOrderHeader]";
            cmnd.Connection = conn;
            int Id = 0;

            SqlDataReader reader = cmnd.ExecuteReader();
            while (reader.Read())
            {
                Id = reader.GetInt32(0);
            }
            reader.Close();
            reader.Dispose();


            cmnd.CommandText = "[sp_InsertOrderItem]";
            cmnd.CommandType = CommandType.StoredProcedure;

            cmnd.Parameters.Add("@orderHeaderId", SqlDbType.Int);
            cmnd.Parameters.Add("@stockItemId", SqlDbType.Int);
            cmnd.Parameters.Add("@description", SqlDbType.VarChar);
            cmnd.Parameters.Add("@price", SqlDbType.Decimal);
            cmnd.Parameters.Add("@quantity", SqlDbType.Int);

            foreach (OrderItem item in DbOrderItems)
            {
                cmnd.Parameters[0].Value = Id;
                cmnd.Parameters[1].Value = item.StockItemId;
                cmnd.Parameters[2].Value = item.Description;
                cmnd.Parameters[3].Value = item.Price;
                cmnd.Parameters[4].Value = item.Quantity;

                cmnd.ExecuteNonQuery();
            }

            cmnd.Parameters.Clear();
            cmnd.CommandText = "[sp_UpdateOrderState]";
            cmnd.Parameters.Add("@orderHeaderId", SqlDbType.Int).Value = Id;
            cmnd.Parameters.Add("@stateId", SqlDbType.Int).Value = 2;
            cmnd.ExecuteNonQuery();
            Console.WriteLine($"Order no. {Id}, has been successfully added to the database.");
            CloseConnection();
        }

        public static void OpenConnection()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    Console.WriteLine("The connection is " + conn.State.ToString());
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Open connection failed: " + ex.Message);
            }
        }
        public static void CloseConnection()
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    Console.WriteLine("The connection is " + conn.State.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Close connection error: " + ex.Message);
            }
        }
    }
}
