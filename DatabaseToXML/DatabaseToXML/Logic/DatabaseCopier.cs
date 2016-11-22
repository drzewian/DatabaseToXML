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
        public DatabaseCopier(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string TestConnection(string databaseName)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
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
    }
}
