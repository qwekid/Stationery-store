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
        public CreateCategory()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                string category_name = textBox1.Text;
                string catefory_desc = textBox2.Text;

                string query = $"INSERT INTO `officesupplies`.`categories` (`category_name`, `description`) VALUES ('{category_name}', '{catefory_desc}');";
                dbhelper.InsertDataOnDb(query);

                MessageBox.Show("Категория успешно добавлена");
            }
            else{
                MessageBox.Show("Заполните все поля!"); 
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
