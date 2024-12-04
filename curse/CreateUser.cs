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

        private static bool isRedact;
        private static int Id;

        public CreateUser()
        {
            InitializeComponent();
            this.ControlBox = false;
            isRedact = false;
        }

        public CreateUser(int id):this()
        {
            isRedact = true;
            Id = id;

            DataTable dt = new DataTable();
            query = $"SELECT * FROM officesupplies.users where user_id = {Id};";
            dbhelper.LoadDataToDt(dt, query);
            
            textBox1.Text = dt.Rows[0].ItemArray[1].ToString();
            textBox2.Text = dt.Rows[0].ItemArray[2].ToString();
            textBox3.Text = dt.Rows[0].ItemArray[3].ToString();
            comboBox1.SelectedIndex = Convert.ToInt32(dt.Rows[0].ItemArray[4].ToString()) -1;
        }


        private void CreateUser_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (!isRedact)
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
                else
                {
                    MessageBox.Show("Сначала заполните все данные о пользователе");
                }
            }
            else {
                if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.SelectedIndex != -1)
                {
                    string name = textBox1.Text;
                    string mail = textBox2.Text;
                    string password = textBox3.Text;
                    int role = comboBox1.SelectedIndex + 1;

                    query = $"UPDATE `officesupplies`.`users` SET `username` = '{name}', `email` = '{mail}', `password` = '{password}', `role` = '{role}' WHERE (`user_id` = '{Id}');;";
                    dbhelper.InsertDataOnDb(query);
                    MessageBox.Show("Запись успешно добавлена");

                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    comboBox1.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("Сначала заполните все данные о пользователе");
                }
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
