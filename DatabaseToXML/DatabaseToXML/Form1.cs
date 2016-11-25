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
using System.IO;
using System.Xml;

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

            dataCopier = new DatabaseCopier(connectionString, tbDatabase.Text);

            listBox1.Items.Add(dataCopier.TestConnection());

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

        private void ButtonDatabaseToXml_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "xml files (*.xml)|*.xml";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = saveFileDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            listBox1.Items.Add(dataCopier.GetDatabase());
                            dataCopier.myDataSet.WriteXml(myStream, XmlWriteMode.WriteSchema);                          
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd: Nie udało się zapisać danych do pliku. Błąd: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            listBox1.Items.Add("Poprawnie zapisano bazę danych do pliku.");
        }

        private void ButtonXmlToDatabase_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "xml files (*.xml)|*.xml";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            listBox1.Items.Add(dataCopier.SaveDatabase(myStream));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd: Nie udało się odtworzyć danych. Błąd: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }            
        }
    }
}
