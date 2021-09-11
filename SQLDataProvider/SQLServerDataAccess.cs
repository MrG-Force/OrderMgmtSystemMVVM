using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataProvider
{
    /// <summary>
    /// This class establish communication with the DataBase.
    /// </summary>
    internal static class SQLServerDataAccess
    {
        static ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["OrdersMgmtConnectionString"];
        static SqlConnection conn = new SqlConnection(settings.ConnectionString);
        static SqlCommand cmnd = new SqlCommand();

        public static SqlCommand GetSqlCommand(string sql)
        {
            cmnd.CommandText = sql;
            cmnd.Connection = conn;
            return cmnd;
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
