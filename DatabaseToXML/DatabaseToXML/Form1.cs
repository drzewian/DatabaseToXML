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
using DatabaseToXML.Logic;

namespace DatabaseToXML
{
    public partial class Form1 : Form
    {
        DatabaseCopier dataCopier;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbDatabase.Text))
            {
                listBox1.Items.Add("Niepoprawna nazwa bazy danych. Spróbuj ponownie.");
                return;
            }

            //string connectionString = "Server=localhost;Port=3306;Database=imprezy;Uid=root;Pwd=";
            string connectionString = "Server=" + tbServer.Text + ";Port=" + numericUpDownPort.Value.ToString() + ";Database=" + tbDatabase.Text + ";Uid=" + tbLogin.Text + ";Pwd=" + tbPassword.Text + ";";

            dataCopier = new DatabaseCopier(connectionString);

            listBox1.Items.Add(dataCopier.TestConnection(tbDatabase.Text));

            if (dataCopier.IsConnected)
            {
                ButtonDatabaseToXml.Enabled = true;
                ButtonXmlToDatabase.Enabled = true;
                tbServer.Enabled = false;
                numericUpDownPort.Enabled = false;
                tbDatabase.Enabled = false;
                tbLogin.Enabled = false;
                tbPassword.Enabled = false;
                ButtonConnect.Visible = false;
                ButtonDisconnect.Visible = true;
            }


            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.            
            //using (MySqlConnection connection = new MySqlConnection(connectionString))
            //{   
            //    // Open the connection in a try/catch block. 
            //    // Create and execute the DataReader, writing the result
            //    // set to the console window.
            //    try
            //    {
            //        connection.Open();
            //        if (connection.State == ConnectionState.Open)
            //        {
            //            listBox1.Items.Add("Nawiązano połączenie");

            //            MySqlDataAdapter adapter = new MySqlDataAdapter();

            //            MySqlCommand command = new MySqlCommand();
            //            command.Connection = connection;
            //            command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = '" + tbDatabase.Text + "'";
            //            command.CommandType = CommandType.Text;
                        
            //            adapter.SelectCommand = command;

            //            DataSet dataSet = new DataSet("DateBases");

            //            adapter.Fill(dataSet);

            //            listBox1.Items.Add("Pobrano dane z bazydanych");

            //            dataGridView1.DataSource = dataSet.Tables[0].DefaultView;
            //            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            //                comboBox1.Items.Add(dataSet.Tables[0].Rows[i].ItemArray[0]);                                              
            //        }
                    
            //    }
            //    catch (Exception ex)
            //    {
            //        listBox1.Items.Add(ex.Message);
            //    }              
            //}
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string connectionString = "Server=localhost;Port=3306;Database=imprezy;Uid=root;Pwd=";

            //// Create and open the connection in a using block. This
            //// ensures that all resources will be closed and disposed
            //// when the code exits.            
            //using (MySqlConnection connection = new MySqlConnection(connectionString))
            //{
            //    // Open the connection in a try/catch block. 
            //    // Create and execute the DataReader, writing the result
            //    // set to the console window.
            //    try
            //    {
            //        connection.Open();
            //        if (connection.State == ConnectionState.Open)
            //        {
            //            listBox1.Items.Add("Nawiązano połączenie");

            //            MySqlDataAdapter adapter = new MySqlDataAdapter();

            //            MySqlCommand command = new MySqlCommand("SELECT * FROM " + comboBox1.SelectedItem, connection);
            //            command.CommandType = CommandType.Text;

            //            adapter.SelectCommand = command;

            //            DataSet dataSet = new DataSet("DateBases");

            //            adapter.Fill(dataSet);

            //            listBox1.Items.Add("Pobrano dane z bazydanych");

            //            dataGridView1.DataSource = dataSet.Tables[0].DefaultView;
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        listBox1.Items.Add(ex.Message);
            //    }
            //}
        }

        private void checkBoxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowPassword.Checked == true)
                tbPassword.UseSystemPasswordChar = false;
            else
                tbPassword.UseSystemPasswordChar = true;
        }

        private void ButtonDisconnect_Click(object sender, EventArgs e)
        {
            dataCopier = null;
            
            ButtonDatabaseToXml.Enabled = false;
            ButtonXmlToDatabase.Enabled = false;
            tbServer.Enabled = true;
            numericUpDownPort.Enabled = true;
            tbDatabase.Enabled = true;
            tbLogin.Enabled = true;
            tbPassword.Enabled = true;
            ButtonConnect.Visible = true;
            ButtonDisconnect.Visible = false;

            listBox1.Items.Add("Rozłączono połączenie z serwerem.");
        }
    }
}
