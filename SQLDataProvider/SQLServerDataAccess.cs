using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SQLDataProvider
{
    /// <summary>
    /// This class establish communication with the DataBase.
    /// </summary>
    internal static class SqlServerDataAccess
    {
        static ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["OrdersMgmtConnectionString"];
        static SqlConnection conn = new SqlConnection(settings.ConnectionString);
        static SqlCommand command = new SqlCommand();

        public static SqlCommand GetSqlCommand(string sql = null)
        {
            command.CommandText = sql;
            command.Connection = conn;
            command.CommandType = CommandType.StoredProcedure;
            return command;
        }

        public static void ClearCommandParams()
        {
            command.Parameters.Clear();
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
