using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace curse
{
    public partial class LocalAdminForm : Form
    {
        public LocalAdminForm()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void LocalAdminForm_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dbhelper.InsertDataOnDb("SELECT table_name FROM information_schema.tables WHERE table_schema = 'officesupplies';");

            foreach (DataRow row in dt.Rows) {
                comboBox1.Items.Add(row.ItemArray[0].ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "SQL Files (*.sql)|*.sql";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    RestoreDatabaseStructure(filePath);
                }
            }
        }

        private void RestoreDatabaseStructure(string filePath)
        {
            string sqlScript = File.ReadAllText(filePath);

            dbhelper.InsertDataOnDb(sqlScript);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    ImportData(filePath);
                }
            }
        }

        private void ImportData(string filePath)
        {
            string connectionString = "YourConnectionStringHere"; // Укажите строку подключения к вашей базе данных

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // Предполагается, что данные разделены запятыми
                        var values = line.Split(',');
                        // Здесь необходимо изменить запрос в зависимости от вашей таблицы
                        string query = "INSERT INTO YourTable (Column1, Column2) VALUES (@value1, @value2)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@value1", values[0]);
                            command.Parameters.AddWithValue("@value2", values[1]);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                MessageBox.Show("Данные успешно импортированы.");
            }
        }
    }
}
