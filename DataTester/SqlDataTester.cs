using DataModels;
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
        public static DataSet dataSet;
        public static SqlDataAdapter dataAdapter;
        public string sql;
        
        public static void PrintItems()
        {
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
