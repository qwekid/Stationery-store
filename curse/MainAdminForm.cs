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
using Microsoft.Office.Interop.Word;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace curse
{
    public partial class MainAdminForm : Form
    {
        public MainAdminForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            label1.Text = UserInfo.userName;
            System.Data.DataTable dt = new System.Data.DataTable();
            dbhelper.LoadDataToDt(dt, $"Select role_name from roles Where id = {UserInfo.role}");
            label2.Text = dt.Rows[0].ItemArray[0].ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminForm a = new AdminForm("products");
            a.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AdminForm a = new AdminForm("sales");
            a.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AdminForm a = new AdminForm("users");
            a.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdminForm a = new AdminForm("categories");
            a.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AdminForm a = new AdminForm("suppliers");
            a.ShowDialog();   
        }
    }
}
