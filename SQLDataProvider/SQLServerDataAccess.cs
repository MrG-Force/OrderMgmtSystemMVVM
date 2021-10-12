using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SQLDataProvider
{
    /// <summary>
    /// This class provides methods to open and close a connection with the DataBase
    /// and provides a SqlCommand object to execute queries.
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

        /// <summary>
        /// Gets a connection string to work with a local instance of SqlServer2019.
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            SqlConnectionStringBuilder builder =
                new SqlConnectionStringBuilder
                {
                    ApplicationName = "XYZ & Co Order Management System",
                    DataSource = @"DESKTOP-HMEUNEN\SQLSERVER2019",
                    InitialCatalog = "OrderManagementDbTestData",
                    IntegratedSecurity = true
                };
            Debug.WriteLine(builder.ToString());
            return builder.ToString();
        }

        /// <summary>
        /// Clears the parameters in the static SqlCommand object.
        /// </summary>
        public static void ClearCommandParams()
        {
            command.Parameters.Clear();
        }

        /// <summary>
        /// Opens a connection with the database.
        /// </summary>
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

        /// <summary>
        /// Closes the connection with the database.
        /// </summary>
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
