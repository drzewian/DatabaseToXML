using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace DatabaseToXML.Logic
{
    class DatabaseCopier
    {
        private string ConnectionString { get; set; }
        public bool IsConnected { get; private set; } = false;
        public DataSet myDataSet { get; private set; }
        private string DatabaseName { get; set; }

        public DatabaseCopier(string connectionString, string databaseName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
        }

        public string TestConnection()
        {
            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits. 
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                // Open the connection in a try/catch block.               
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        IsConnected = true;
                        return "Nawiązano połączenie z serwerem.";                        
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "Nieudane połączenie z serwerem.";
        }
        
        public string GetDatabase()
        {
            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.            
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                // Open the connection in a try/catch block.                 
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {   
                        MySqlDataAdapter adapter = new MySqlDataAdapter();

                        MySqlCommand command = new MySqlCommand();
                        command.Connection = connection;
                        command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = '" + DatabaseName + "'";                        
                        command.CommandType = CommandType.Text;
                        adapter.SelectCommand = command;
                        DataSet tempDataSet = new DataSet("TablesNames");
                        adapter.Fill(tempDataSet);

                        myDataSet = new DataSet("Database");
                        
                        for (int i = 0; i < tempDataSet.Tables[0].Rows.Count; i++)
                        {
                            command.CommandText = "SELECT * FROM " + tempDataSet.Tables[0].Rows[i].ItemArray[0];
                            command.CommandType = CommandType.Text;
                            adapter.SelectCommand = command;
                            myDataSet.Tables.Add(tempDataSet.Tables[0].Rows[i].ItemArray[0].ToString());
                            adapter.Fill(myDataSet.Tables[tempDataSet.Tables[0].Rows[i].ItemArray[0].ToString()]);
                        }                            
                        return "Pobrano bazę danych.";
                    }

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "Nie udało się pobrać danych z bazy.";
        }
    }
}
