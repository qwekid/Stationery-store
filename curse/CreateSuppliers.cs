using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace curse
{
    public partial class CreateSuppliers : Form
    {
        private static string query = string.Empty;
        private static bool isRedact;
        private static int Id;
        public CreateSuppliers()
        {
            InitializeComponent();
            this.ControlBox = false;
            isRedact = false;
        }

        public CreateSuppliers(int id):this()
        {
            isRedact = true;
            Id = id;

            DataTable dt = new DataTable();

            query = $"SELECT * FROM officesupplies.suppliers where supplier_id = {Id};";
            dbhelper.LoadDataToDt(dt,query);

            textBox1.Text = dt.Rows[0].ItemArray[1].ToString();
            textBox2.Text = dt.Rows[0].ItemArray[2].ToString();
            maskedTextBox1.Text = dt.Rows[0].ItemArray[3].ToString();
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (!isRedact)
            {
                if (textBox1.Text != "" && textBox2.Text != "" && maskedTextBox1.TextLength == 17)
                {
                    query = $"INSERT INTO `officesupplies`.`suppliers` (`supplier_name`, `contact_email`, `phone`)  VALUES ('{textBox1.Text}', '{textBox2.Text}', '{maskedTextBox1.Text}');";
                    dbhelper.InsertDataOnDb(query);
                    MessageBox.Show("Запись успешно добавлена");

                    textBox1.Clear();
                    textBox2.Clear();
                    maskedTextBox1.Clear();
                }
                else
                {
                    MessageBox.Show("Сначала заполните все данные о компании-поставщике");
                }
            }
            else {
                if (textBox1.Text != "" && textBox2.Text != "" && maskedTextBox1.TextLength == 17)
                {
                    query = $"UPDATE `officesupplies`.`suppliers` SET `supplier_name` = '{textBox1.Text}', `contact_email` = '{textBox2.Text}', `phone` = '{maskedTextBox1.Text}' WHERE (`supplier_id` = '{Id}');";
                    dbhelper.InsertDataOnDb(query);
                    MessageBox.Show("Запись успешно добавлена");

                    textBox1.Clear();
                    textBox2.Clear();
                    maskedTextBox1.Clear();
                }
                else
                {
                    MessageBox.Show("Сначала заполните все данные о компании-поставщике");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CreateSuppliers_Load(object sender, EventArgs e)
        {

        }
    }
}
