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
            dbhelper.LoadDataToDt(dt,"SELECT table_name FROM information_schema.tables WHERE table_schema = 'officesupplies';");
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
            if (comboBox1.SelectedIndex != -1)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "CSV файлы с раделителем ',' (*.csv)|*.csv";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        dbhelper.ImportData(filePath, comboBox1.SelectedItem.ToString());
                    }
                }
            }
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
