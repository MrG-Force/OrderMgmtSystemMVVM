To run this application using the random sample data generator,
open App.xaml.cs and uncomment line 32.

If you want to connect to a local DB you need to configure the connection string
in App.config.

The Database must be created using either the DB_Script.sql or DB_Test_Data_Script.sql
and also you have to execute the file DB_StoredProcedures_GOrtiz.sql to install the required
stored procedures that do not exist in the original DB.