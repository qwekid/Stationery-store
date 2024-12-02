using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace curse
{
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;

            if (login == LocalAdminAccount.Username && password ==LocalAdminAccount.Password)
            {
                LocalAdminForm laf = new LocalAdminForm();
                if (laf.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Clear();
                    textBox2.Clear();
                    this.AuthForm_Load(sender, e);
                }
                
            }
            else
            {
                int role = dbhelper.CheckUserRole(login, password);
                if (role == 2)
                {
                    AdminForm a = new AdminForm();
                    a.Show();
                    this.Hide();
                }
                else if (role == 1)
                {
                    ManagerForm a = new ManagerForm();
                    a.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!");
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true) { textBox2.PasswordChar = '*'; }
            else if (checkBox1.Checked == false) { textBox2.PasswordChar = '\0'; }
        }

        private void AuthForm_Load(object sender, EventArgs e)
        {

        }
    }
}
