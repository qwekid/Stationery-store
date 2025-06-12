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
    public partial class CreateCategory : Form
    {
        private static bool isRedact;
        private static int Id;
        public CreateCategory()
        {
            InitializeComponent();
            this.ControlBox = false;
            isRedact = false;
        }

        public CreateCategory(int id,string name, string desc) : this()
        {
            Id = id;
            textBox1.Text = name;
            textBox2.Text = desc;
            isRedact = true;
            button1.Text = "Внести изменения";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isRedact)
            {
                if (textBox1.Text != "" && textBox2.Text != "")
                {
                    string category_name = textBox1.Text;
                    string catefory_desc = textBox2.Text;

                    string query = $"INSERT INTO `officesupplies`.`categories` (`category_name`, `description`) VALUES ('{category_name}', '{catefory_desc}');";
                    dbhelper.InsertDataOnDb(query);

                    MessageBox.Show("Категория успешно добавлена");

                    textBox1.Clear();
                    textBox2.Clear();
                }
                else
                {
                    MessageBox.Show("Заполните все поля!");
                }
            }
            else {
                if (textBox1.Text != "" && textBox2.Text != "")
                {
                    string category_name = textBox1.Text;
                    string catefory_desc = textBox2.Text;

                    string query = $"UPDATE `officesupplies`.`categories` SET `category_name` = '{category_name}', `description` = '{catefory_desc}' WHERE (`category_id` = '{Id}');";
                    dbhelper.InsertDataOnDb(query);

                    MessageBox.Show("Категория успешно отредактирована");
                    textBox1.Clear();
                    textBox2.Clear();
                }
                else
                {
                    MessageBox.Show("Заполните все поля!");
                }
            }
        }

        private void CreateCategory_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
