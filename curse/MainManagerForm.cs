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
        private static bool isProgrammaticClose = false;

        private Timer inactivityTimer;
        private int inactivityLimit = 10 * 1000;

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


            inactivityTimer = new Timer();
            inactivityTimer.Interval = inactivityLimit;
            inactivityTimer.Tick += InactivityTimer_Tick;
            inactivityTimer.Start();

            this.MouseMove += ResetTimer;
            this.KeyPress += ResetTimer;
            this.MouseClick += ResetTimer;
        }

        private void ResetTimer(object sender, EventArgs e)
        {
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }

        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            inactivityTimer.Stop();
            isProgrammaticClose = true;

            this.Close();
        }

        private void FormClose(object sender, FormClosingEventArgs e)
        {
            if (!isProgrammaticClose)
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
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ManagerForm a = new ManagerForm("sales");
            if (a.ShowDialog() == DialogResult.Abort) { isProgrammaticClose = true; this.Close(); };
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ManagerForm a = new ManagerForm("products");
            if (a.ShowDialog() == DialogResult.Abort) { isProgrammaticClose = true; this.Close(); };
        }

        private void button9_Click(object sender, EventArgs e)
        {

            ReportForm r = new ReportForm();
            if (r.ShowDialog() == DialogResult.Abort) { isProgrammaticClose = true; this.Close(); };
        }

        private void button16_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
