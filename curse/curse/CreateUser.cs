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
    public partial class CreateUser : Form
    {

        private static string query = string.Empty;


        public CreateUser()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void CreateUser_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.SelectedIndex != -1)
            {
                query = $"INSERT INTO `officesupplies`.`users` (`username`, `email`, `password`, `role`) VALUES ('{textBox1.Text}', '{textBox2.Text}', '{textBox3.Text}', '{comboBox1.SelectedIndex + 1}');";
                dbhelper.InsertDataOnDb(query);
                MessageBox.Show("Запись успешно добавлена");

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                comboBox1.SelectedIndex = -1;
            }
            else {
                MessageBox.Show("Сначала заполните все данные о пользователе");
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
