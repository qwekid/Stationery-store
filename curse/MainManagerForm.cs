using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace curse
{
    public partial class MainManagerForm : Form
    {
        public MainManagerForm()
        {
            InitializeComponent();
            this.FormClosing += FormClose;
        }

        private void mainManagerForm_Load(object sender, EventArgs e)
        {
            label1.Text = UserInfo.userName;
            System.Data.DataTable dt = new System.Data.DataTable();
            dbhelper.LoadDataToDt(dt, $"Select role_name from roles Where id = {UserInfo.role}");
            label2.Text = dt.Rows[0].ItemArray[0].ToString();
        }
        private void FormClose(object sender, FormClosingEventArgs e)
        {
            var dialogResult = MessageBox.Show(
                "Экспортировать базу данных перед выходом?",
                "Подтверждение",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (dialogResult == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;

                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

                folderBrowserDialog.Description = "Выберите папку для сохранения";
                folderBrowserDialog.ShowNewFolderButton = true;
                string projectRoot = AppDomain.CurrentDomain.BaseDirectory;
                folderBrowserDialog.SelectedPath = Path.Combine(projectRoot, "dumps");

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowserDialog.SelectedPath;
                    bool dumpCreated = dbhelper.CreateDump(selectedPath);

                    Cursor.Current = Cursors.Default;

                    if (!dumpCreated)
                    {
                        MessageBox.Show("Не удалось создать резервную копию!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }

                    System.Windows.Forms.Application.Exit();
                }
            }
            else if (dialogResult == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                System.Windows.Forms.Application.Exit();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ManagerForm a = new ManagerForm("sales");
            a.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ManagerForm a = new ManagerForm("products");
            a.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {

            ReportForm r = new ReportForm();
            r.ShowDialog();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
