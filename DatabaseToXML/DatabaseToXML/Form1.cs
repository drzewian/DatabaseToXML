using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DatabaseToXML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=mysql.cba.pl;Port=3306;Database=imprezymasowe_cba_pl;Uid=wsb123;Pwd=wsb123haslo";            

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.            
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {   
                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        listBox1.Items.Add("Nawiązano połączenie");

                        MySqlDataAdapter adapter = new MySqlDataAdapter();

                        MySqlCommand command = new MySqlCommand("SHOW DATABASES", connection);
                        command.CommandType = CommandType.Text;

                        adapter.SelectCommand = command;

                        DataSet dataSet = new DataSet("DateBases");

                        adapter.Fill(dataSet);

                        listBox1.Items.Add("Pobrano dane z bazydanych");

                        dataGridView1.DataSource = dataSet.Tables[0].DefaultView;

                        var table = dataSet.Tables[0].Rows;

                        for(int i = 0; i<table.Count; i++)
                        {
                            listBox1.Items.Add(table[i].ItemArray[0].ToString());
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    listBox1.Items.Add(ex.Message);
                }
                
                               
            }
        }
    }
}
