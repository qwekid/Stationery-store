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

            string hashedPassword = Hasher.HashPassword(password);

            int role = dbhelper.CheckUserRole(login, password);

            if (role == 2)
            {
                MainAdminForm a = new MainAdminForm();
                this.Hide();
                if (a.ShowDialog() == DialogResult.OK) 
                {
                    this.Show();

                    textBox1.Text = "";
                    textBox2.Text = "";
                };
            }
            else if (role == 1)
            {
                MainManagerForm a = new MainManagerForm();
                this.Hide();
                if (a.ShowDialog() == DialogResult.OK)
                {
                    this.Show();

                    textBox1.Text = "";
                    textBox2.Text = "";
                };
            }
            else {
                textBox1.Text = "";
                textBox2.Text = "";

                MessageBox.Show("Неверный логин или пароль!", "Предупреждание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Captcha captcha = new Captcha();
                if (captcha.ShowDialog() != DialogResult.OK) 
                {
                    
                    this.Enabled = false;

                    
                    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                    timer.Interval = 10000; 
                    timer.Tick += (s, end) =>
                    {
                        timer.Stop();
                        timer.Dispose();

                        this.Enabled = true;
                    };
                    timer.Start();

                    textBox1.Text = "";
                    textBox2.Text = "";
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
            textBox1.Text = "";
            textBox2.Text = "";
        }
    }
}
