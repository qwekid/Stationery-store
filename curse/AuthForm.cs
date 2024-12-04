using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace curse
{
    public partial class AuthForm : Form
    {
        private static int counter = 0;

        private static string currentcapcha = String.Empty;

        PictureBox PB = new PictureBox { Top = 10, Left = 230 };
        TextBox capchaTextBox = new TextBox { Left = 230, Top = 95 };
        Button confirmCapchaButton = new Button { Left = 230, Top = 130, Width = 100 };
        Button updateCapchaButton = new Button { Left = 230, Top = 165, Width = 100 };

        public AuthForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string login = textBox1.Text;
            string password = textBox2.Text;
            if (counter == 0)
            {
                if (login == LocalAdminAccount.Username && password == LocalAdminAccount.Password)
                {
                    LocalAdminForm laf = new LocalAdminForm();
                    if (laf.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Clear();
                        textBox2.Clear();
                        this.AuthForm_Load(sender, e);
                    }
                    return;
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
                        counter++;
                    }
                }
            }
            if(counter>0){
                this.Width = 400;

                textBox1.Enabled = false;
                textBox2.Enabled = false;


                currentcapcha = Capcha.GenerateCaptcha();
                Bitmap capchaImg = Capcha.DrawCaptcha(currentcapcha);
                PB.Image = capchaImg;


                capchaTextBox.Font = new Font(label1.Font.Name, 14.25F);
                capchaTextBox.MaxLength = 4;

                
                confirmCapchaButton.Text = "Проверить";
                confirmCapchaButton.Click += ConfirmCapchaButton_Click;

                updateCapchaButton.Text = "Обновить капчу";
                updateCapchaButton.Click += UpdateCapchaButton_Click;

                this.Controls.Add(PB);
                this.Controls.Add(capchaTextBox);
                this.Controls.Add(confirmCapchaButton);
                this.Controls.Add(updateCapchaButton);
                
            }
        }

        private void UpdateCapchaButton_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;


            currentcapcha = Capcha.GenerateCaptcha();
            Bitmap capchaImg = Capcha.DrawCaptcha(currentcapcha);
            PB.Image = capchaImg;
        }

        private void ConfirmCapchaButton_Click(object sender, EventArgs e)
        {
            if (currentcapcha == capchaTextBox.Text)
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;

                this.Width = 201;

                this.AuthForm_Load(sender, e);
            }
            else {
                MessageBox.Show("Капча введена не правильно, система заблокиравана на 10 секунд");
                Thread.Sleep(10000);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true) { textBox2.PasswordChar = '*'; }
            else if (checkBox1.Checked == false) { textBox2.PasswordChar = '\0'; }
        }

        private void AuthForm_Load(object sender, EventArgs e)
        {
            counter = 0;
            currentcapcha = Capcha.GenerateCaptcha();
            Bitmap capchaImg = Capcha.DrawCaptcha(currentcapcha);
            PB.Image = capchaImg;
            capchaTextBox.Clear();
        }
    }
}
