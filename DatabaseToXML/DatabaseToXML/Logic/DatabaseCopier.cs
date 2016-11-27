using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Xml;

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
                            adapter.FillSchema(myDataSet.Tables[tempDataSet.Tables[0].Rows[i].ItemArray[0].ToString()], SchemaType.Mapped);
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

        public string SaveDatabase(Stream myStream)
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

                        myDataSet = new DataSet();                        
                        myDataSet.ReadXml(myStream, XmlReadMode.ReadSchema);

                        command.CommandText = CreateTables(myDataSet);
                        command.ExecuteNonQuery();

                        for (int i = 0; i < myDataSet.Tables.Count; i++)
                        {                                                     
                            command.CommandText = "SELECT * FROM " + myDataSet.Tables[i].TableName;
                            command.CommandType = CommandType.Text;
                            adapter.SelectCommand = command;
                            var builder = new MySqlCommandBuilder(adapter);

                            adapter.InsertCommand = builder.GetInsertCommand();
                            adapter.Update(myDataSet, myDataSet.Tables[i].TableName);
                            
                        }
                        return "Pomyślnie odtworzono bazę danych.";
                    }

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "Nie udało się odtworzyć bazy danych.";
        }

        private string CreateTables(DataSet table)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("BEGIN; {0}   ", Environment.NewLine);

            for (int i = 0; i < myDataSet.Tables.Count; i++)
            {
                result.AppendFormat(CreateTableSql(myDataSet.Tables[i]) + "{0}", Environment.NewLine);
            }
            result.Append("COMMIT;");

            return result.ToString();            
        }
        private string CreateTableSql(DataTable table)
        {   
            StringBuilder result = new StringBuilder();
            result.AppendFormat("CREATE TABLE IF NOT EXISTS {1} ({0}   ", Environment.NewLine, table.TableName);

            bool FirstTime = true;
            foreach (DataColumn column in table.Columns.OfType<DataColumn>())
            {
                if (FirstTime) FirstTime = false;
                else
                    result.Append("   ,");

                result.AppendFormat("{0} {1} {2} {3}",
                    column.ColumnName, // 0
                    GetSQLType(column.DataType, column.MaxLength), // 1
                    column.AllowDBNull ? "" : "NOT NULL", // 2
                    Environment.NewLine // 3
                    );
            }

            if (table.PrimaryKey.Length > 0)
            {
                result.Append(GetPrimaryKey(table));
            }
            result.AppendLine();
            result.AppendFormat("); {0}", Environment.NewLine);            

            return result.ToString();
        }

        private string GetPrimaryKey(DataTable table)
        {
            StringBuilder result = new StringBuilder();            
            result.AppendLine();
            result.Append(",PRIMARY KEY (");

            bool FirstTime = true;
            foreach (DataColumn column in table.PrimaryKey)
            {
                if (FirstTime) FirstTime = false;
                else
                    result.Append(",");

                result.Append(column.ColumnName);
            }
            result.Append(")");
            return result.ToString();
        }      
                
        private string GetSQLType(Type DataType, int columnSize)
        {
            switch (DataType.Name)
            {
                case "Boolean": return "boolean";
                case "Char": return "char";
                case "SByte": return "tinyint";
                case "Int16": return "smallint";
                case "Int32": return "int";
                case "Int64": return "bigint";
                case "Byte": return "tinyint UNSIGNED";
                case "UInt16": return "smallint UNSIGNED";
                case "UInt32": return "int UNSIGNED";
                case "UInt64": return "bigint UNSIGNED";
                case "Single": return "float";
                case "Double": return "double";
                case "Decimal": return "decimal";
                case "DateTime": return "datetime";                
                case "String": return "VARCHAR(" + ((columnSize == -1) ? 255 : columnSize) + ")"; ;
                default: throw new Exception(DataType.ToString() + " not implemented."); ;
            }
        }
    }
}
