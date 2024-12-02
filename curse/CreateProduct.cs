using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace curse
{
    
    public partial class CreateProduct : Form
    {
        private static string query = string.Empty;

        public CreateProduct()
        {
            InitializeComponent();
        }

        private void CreateProduct_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            comboBox1.Items.Clear();
            query = "SELECT category_name FROM categories";
            dbhelper.LoadDataToDt(dt, query);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBox1.Items.Add(dt.Rows[i].ItemArray.GetValue(0).ToString());
            }

            comboBox2.Items.Clear();
            query = "SELECT supplier_name FROM suppliers";
            dt = new DataTable();
            dbhelper.LoadDataToDt(dt, query);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBox2.Items.Add(dt.Rows[i].ItemArray.GetValue(0).ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {


            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.SelectedIndex != -1 && comboBox2.SelectedIndex != -1)
            {
                DataTable dt = new DataTable();
                string pn = textBox1.Text;
                string c_name = comboBox1.SelectedItem.ToString();
                query = $"select category_id from categories where category_name = '{c_name}'";
                dbhelper.LoadDataToDt(dt, query);
                int c_id = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                int s_id = Convert.ToInt32(comboBox2.SelectedIndex + 1);
                double p = 0;
                if (Double.TryParse(textBox2.Text.Replace('.', ','), out p)) { }
                else { MessageBox.Show("Некоректно указана цена"); textBox2.Clear(); }
                int s = Convert.ToInt32(textBox3.Text);
                query = $"INSERT INTO `officesupplies`.`products` (`product_name`, `category_id`, `supplier_id`, `price`, `stock`) VALUES ( '{pn}', {c_id}, {s_id}, {Convert.ToString(p).Replace(',', '.')}, {s} )";
                dbhelper.InsertDataOnDb(query);

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("Заполните все данные о продукте");
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string input = textBox2.Text;

            // Ограничить количество символов перед запятой
            int commaIndex = input.IndexOf(',');
            if (commaIndex == -1)
            {
                commaIndex = input.Length; // Если запятых нет, считаем всю строку
            }

            // Проверка на количество знаков перед запятой
            if (commaIndex > 5)
            {
                textBox2.Text = input.Substring(0, 5);
                textBox2.SelectionStart = textBox1.Text.Length; // Поставить курсор в конец
            }

            // Проверка на количество знаков после запятой
            if (commaIndex + 3 < input.Length) // +3 для учета знака и двух цифр
            {
                textBox2.Text = input.Substring(0, commaIndex + 3);
                textBox2.SelectionStart = textBox1.Text.Length; // Поставить курсор в конец
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешить только цифры, запятую и клавишу Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true; // Запретить ввод
            }

            // Разрешить только одну запятую
            if (e.KeyChar == ',' && textBox1.Text.Contains(","))
            {
                e.Handled = true;
            }


        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else { e.Handled = false; }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
