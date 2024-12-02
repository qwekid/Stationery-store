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
        public CreateSuppliers()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void button12_Click(object sender, EventArgs e)
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
