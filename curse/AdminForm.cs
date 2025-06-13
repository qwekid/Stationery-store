using Microsoft.Office.Interop.Word;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;

namespace curse
{
    public partial class AdminForm : Form
    {
        private static string query = string.Empty;
        private static string table = string.Empty;
        private static string id_string = string.Empty;

        private static int pageSize = 5;
        private static int pageNumber = 1;

        private static readonly string viewproductsquery = "SELECT p.product_id, p.product_name AS 'Наименование товара', c.category_name AS 'Категория', s.supplier_name AS 'Поставщик', p.price AS 'Цена', p.stock AS 'Остаток на складе' FROM products p JOIN categories c ON p.category_id = c.category_id JOIN suppliers s ON p.supplier_id = s.supplier_id";
        private static readonly string viewcategoriesquery = "SELECT category_id,  category_name as 'Наименование категории', description as 'Описание категории' FROM categories";
        private static readonly string viewusersquery = "SELECT user_id, username as 'Логин', email as 'Почта', password as 'Пароль', r.role_name as 'Роль' FROM users u JOIN roles r ON u.role = r.id";
        private static readonly string viewsalesquery = "SELECT \r\n u.username AS \"Продавец\", \r\n    GROUP_CONCAT(CONCAT(p.product_name, ': ', c.quantity, ' шт.') SEPARATOR '; ') AS \"Товары\", \r\n    s.sale_date AS \"Дата продажи\", \r\n    s.total_amount AS \"Финальная стоимость\" \r\nFROM \r\n    `check` c \r\nJOIN \r\n    sales s ON c.sales_sale_id = s.sale_id \r\nJOIN  \r\n    products p ON c.products_product_id = p.product_id \r\nJOIN \r\n    users u ON s.user_id = u.user_id ";
        private static readonly string viewsalesqueryend = "GROUP BY \r\n s.sale_id, u.username, s.sale_date, s.total_amount";
        private static ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

        private static bool isProgrammaticClose = false;

        private Timer inactivityTimer;
        private int inactivityLimit = 10 * 60 * 1000;

        public AdminForm(string tableName)
        {
            table = tableName;
            InitializeComponent();
            this.FormClosing += FormClose;

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

        private void load(object sender, EventArgs e) {
            comboBox1.Items.Clear();
            switch (table)
            {
                case "sales":
                    loadSales(sender, e);
                    break;
                case "products":
                    loadProducts(sender, e);
                    break;
                case "users":
                    loadUsers(sender, e);
                    break;
                case "categories":
                    loadCategories(sender, e);
                    break;
                case "suppliers":
                    loadSuppliers(sender, e);
                    break;
            }
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            load(sender, e);
            paginate(sender, e);
            
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            // Проверяем, что двойной клик был внутри границ DataGridView и выбраная таблица не "продажи"
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && table != "sales")
            {
                if (table == "products")
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    int currendrowid = dataGridView1.CurrentRow.Index;

                    contextMenuStrip.Items.Clear();


                    query = $"select product_name from `check` c join products p on c.products_product_id = p.product_id where product_name = '{dataGridView1.Rows[currendrowid].Cells[0].Value}'";
                    dbhelper.LoadDataToDt(dt, query);
                    if (dt.Rows.Count != 0)
                    {
                        return;
                    }
                    else
                    {

                        ToolStripMenuItem updateRowMenuItem = new ToolStripMenuItem("Редактировать строку");
                        updateRowMenuItem.Click += UpdateRowMenuItem_Click;
                        contextMenuStrip.Items.Add(updateRowMenuItem);

                        ToolStripMenuItem deleteRowMenuItem = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem.Click += DeleteRowMenuItem_Click;
                        contextMenuStrip.Items.Add(deleteRowMenuItem);
                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                    }


                }
                else if (table == "categories")
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    int currendrowid = dataGridView1.CurrentRow.Index;
                    query = $"select category_name from `check` c join products p on c.products_product_id = p.product_id join categories cat on p.category_id = cat.category_id where product_name = '{dataGridView1.Rows[currendrowid].Cells[0].Value}' ";
                    dbhelper.LoadDataToDt(dt, query);

                    if (dt.Rows.Count > 0)
                    {
                        return;
                    }
                    else
                    {
                        ToolStripMenuItem updateRowMenuItem = new ToolStripMenuItem("Редактировать строку");
                        updateRowMenuItem.Click += UpdateRowMenuItem_Click;
                        contextMenuStrip.Items.Clear();
                        contextMenuStrip.Items.Add(updateRowMenuItem);

                        ToolStripMenuItem deleteRowMenuItem1 = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem1.Click += DeleteRowMenuItem_ClickError;
                        contextMenuStrip.Items.Add(deleteRowMenuItem1);
                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                    }
                }
                else if (table == "users")
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    int currendrowid = dataGridView1.CurrentRow.Index;
                    query = $"select username from sales s join users u on s.user_id = u.user_id where username = '{dataGridView1.Rows[currendrowid].Cells[0].Value}' ";
                    dbhelper.LoadDataToDt(dt, query);



                    if (dt.Rows.Count > 0)
                    {
                        return;
                    }
                    else
                    {
                        ToolStripMenuItem updateRowMenuItem = new ToolStripMenuItem("Редактировать строку");
                        updateRowMenuItem.Click += UpdateRowMenuItem_Click;
                        contextMenuStrip.Items.Clear();
                        contextMenuStrip.Items.Add(updateRowMenuItem);

                        ToolStripMenuItem deleteRowMenuItem = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem.Click += DeleteRowMenuItem_Click;
                        contextMenuStrip.Items.Add(deleteRowMenuItem);
                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                    }
                }
                if (table == "suppliers")
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    int currendrowid = dataGridView1.CurrentRow.Index;

                    query = $"select supplier_name from suppliers where supplier_name = '{dataGridView1.Rows[currendrowid].Cells[0].Value}'";
                    dbhelper.LoadDataToDt(dt, query);
                    if (dt.Rows.Count > 0)
                    {
                        return;
                    }
                    else
                    {
                        ToolStripMenuItem deleteRowMenuItem = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem.Click += DeleteRowMenuItem_Click;
                        contextMenuStrip.Items.Add(deleteRowMenuItem);
                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);

                        ToolStripMenuItem updateRowMenuItem = new ToolStripMenuItem("Редактировать строку");
                        updateRowMenuItem.Click += UpdateRowMenuItem_Click;
                        contextMenuStrip.Items.Clear();
                        contextMenuStrip.Items.Add(updateRowMenuItem);
                    }


                }
            }

        }

        private void UpdateRowMenuItem_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            switch (table)
            {
                case ("products"):
                    if (MessageBox.Show("Вы уверены, что хотите редактировать этот элемент?", "Подтверждение редактирования", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {

                        query = $"Select category_id from categories where category_name = '{dataGridView1.CurrentRow.Cells[2].Value}'";
                        dbhelper.LoadDataToDt(dt, query);

                        int category = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                        dt = new System.Data.DataTable();

                        query = $"Select supplier_id from suppliers where supplier_name = '{dataGridView1.CurrentRow.Cells[3].Value}'";
                        dbhelper.LoadDataToDt(dt, query);
                        int suplier = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                        dt = new System.Data.DataTable();

                        int price = Convert.ToInt32(dataGridView1.CurrentRow.Cells[4].Value);
                        int quantity = Convert.ToInt32(dataGridView1.CurrentRow.Cells[5].Value);
                        CreateProduct p = new CreateProduct(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value), dataGridView1.CurrentRow.Cells[1].Value.ToString(), category, suplier, price, quantity);

                        if (p.ShowDialog() == DialogResult.OK)
                        {
                            // Обновляем данные на первой форме
                            this.AdminForm_Load(sender, e);
                        }

                    }
                    break;
                case ("categories"):
                    if (MessageBox.Show("Вы уверены, что хотите редактировать этот элемент?", "Подтверждение редактирования", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                        string name = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                        string desc = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                        CreateCategory c = new CreateCategory(id, name, desc);

                        if (c.ShowDialog() == DialogResult.OK)
                        {
                            // Обновляем данные на первой форме
                            this.AdminForm_Load(sender, e);
                        }
                    }
                    break;
                case ("users"):
                    if (MessageBox.Show("Вы уверены, что хотите редактировать этот элемент?", "Подтверждение редактирования", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                        CreateUser u = new CreateUser(id);
                        if (u.ShowDialog() == DialogResult.OK)
                        {
                            // Обновляем данные на первой форме
                            this.AdminForm_Load(sender, e);
                        }
                    }
                    break;
                case ("suppliers"):
                    if (MessageBox.Show("Вы уверены, что хотите редактировать этот элемент?", "Подтверждение редактирования", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                        CreateSuppliers s = new CreateSuppliers(id);
                        if (s.ShowDialog() == DialogResult.OK)
                        {
                            // Обновляем данные на первой форме
                            this.AdminForm_Load(sender, e);
                        }
                    }
                    break;
            }

        }

        private void DeleteRowMenuItem_ClickError(object sender, EventArgs e)
        {
            MessageBox.Show("Невозможно удалить эту строку!");
        }

        private void DeleteRowMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, какие строки выбраны в DataGridView и удаляем их
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0 && MessageBox.Show("Вы уверены, что хотите удалить этот элемент?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int currendrowid = dataGridView1.CurrentRow.Index - 1;
                dataGridView1.Rows.RemoveAt(currendrowid);
                query = $"DELETE FROM `officesupplies`.`{table}` WHERE (`{id_string}` = '{dataGridView1.Rows[currendrowid].Cells[0].Value}');";
                dbhelper.InsertDataOnDb(query);
                if (table == "products") { query = viewproductsquery; }
                else if (table == "categories") { query = viewcategoriesquery; }
                else if (table == "sales") { query = viewsalesquery; }
                else if (table == "users") { query = viewusersquery; }
                else if (table == "suppliers") { query = $"SELECT supplier_name as 'Наименование компании', contact_email as 'Электронная почта', phone as 'Контактный телефон' FROM suppliers"; }
                dbhelper.LoadDataToDGV(dataGridView1, query, pageNumber, pageSize);
                dataGridView1.Columns[0].Visible = false;
            }

        }
        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string txt = textBox1.Text;
            if (table == "products") { query = viewproductsquery + $" WHERE p.product_name LIKE '%{txt}%' OR c.category_name LIKE '%{txt}%' OR s.supplier_name LIKE '%{txt}%' OR p.price LIKE '%{txt}%' OR p.stock LIKE '%{txt}%';"; }
            else if (table == "categories") { query = viewcategoriesquery + $" WHERE category_name LIKE '%{txt}%' OR description LIKE '%{txt}%'"; }
            else if (table == "sales") { query = viewsalesquery + $" WHERE \r\n    u.username LIKE '%{txt}%' \r\n    OR p.product_name LIKE '%{txt}%' \r\n    OR c.quantity LIKE '%{txt}%' \r\n    OR s.sale_date LIKE '%{txt}%' \r\n    OR s.total_amount LIKE '%{txt}%'" + viewsalesqueryend; }
            else if (table == "users") { query = viewusersquery + $" Where username Like '%{txt}%'"; }
            else if (table == "suppliers") { query = $"SELECT supplier_name as 'Наименование компании', contact_email as 'Электронная почта', phone as 'Контактный телефон' FROM suppliers Where supplier_name Like '%{txt}%' OR contact_email Like '%{txt}%' OR phone Like '%{txt}%'"; }

            dbhelper.LoadDataToDGV(dataGridView1, query, pageNumber, pageSize);
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            switch (table)
            {
                case "sales":
                    CreateOrder sales = new CreateOrder();
                    if (sales.ShowDialog() == DialogResult.OK)
                    {

                        this.AdminForm_Load(sender, e);
                    }

                    break;
                case "products":
                    CreateProduct products = new CreateProduct();
                    if (products.ShowDialog() == DialogResult.OK)
                    {
                        this.AdminForm_Load(sender, e);
                    }
                    break;
                case "categories":
                    CreateCategory c = new CreateCategory();
                    if (c.ShowDialog() == DialogResult.OK)
                    {
                        this.AdminForm_Load(sender, e);
                    }
                    break;
                case "users":
                    CreateUser users = new CreateUser();
                    if (users.ShowDialog() == DialogResult.OK)
                    {
                        this.AdminForm_Load(sender, e);
                    }
                    break;
                case "suppliers":
                    CreateSuppliers sup = new CreateSuppliers();
                    if (sup.ShowDialog() == DialogResult.OK)
                    {
                        this.AdminForm_Load(sender, e);
                    }
                    break;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            comboBox1.Items.Clear();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

            comboBox1.Items.Clear();
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("По продавцу");
            comboBox1.Items.Add("По сумме");
            comboBox1.Items.Add("По дате");
            query = viewsalesquery + viewsalesqueryend;
            table = "sales";
            id_string = "sale_id";
            dbhelper.LoadDataToDGV(dataGridView1, query, pageNumber, pageSize);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            textBox1.Text = "";
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            ReportForm r = new ReportForm();
            r.ShowDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        {

            if (table == "categories")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Наименование категории"], System.ComponentModel.ListSortDirection.Ascending);
                }
            }
            else if (table == "sales")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Продавец"], System.ComponentModel.ListSortDirection.Ascending);
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Финальная стоимость"], System.ComponentModel.ListSortDirection.Ascending);
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Дата продажи"], System.ComponentModel.ListSortDirection.Ascending);
                }
            }
            else if (table == "products")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Наименование товара"], System.ComponentModel.ListSortDirection.Ascending);
                }
                if (comboBox1.SelectedIndex == 1)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Категория"], System.ComponentModel.ListSortDirection.Ascending);
                }
                if (comboBox1.SelectedIndex == 2)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Поставщик"], System.ComponentModel.ListSortDirection.Ascending);
                }
                if (comboBox1.SelectedIndex == 3)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Цена"], System.ComponentModel.ListSortDirection.Ascending);
                }
                if (comboBox1.SelectedIndex == 4)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Остаток на складе"], System.ComponentModel.ListSortDirection.Ascending);
                }
            }
            else if (table == "users")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Логин"], System.ComponentModel.ListSortDirection.Ascending);
                }
                if (comboBox1.SelectedIndex == 1)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Роль"], System.ComponentModel.ListSortDirection.Ascending);
                }
            }
            else if (table == "suppliers")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Наименование компании"], System.ComponentModel.ListSortDirection.Ascending);
                }
                if (comboBox1.SelectedIndex == 1)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Электронная почта"], System.ComponentModel.ListSortDirection.Ascending);
                }
                if (comboBox1.SelectedIndex == 2)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Контактный телефон"], System.ComponentModel.ListSortDirection.Ascending);
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {

            if (table == "categories")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Наименование категории"], System.ComponentModel.ListSortDirection.Descending);
                }
            }
            else if (table == "sales")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Продавец"], System.ComponentModel.ListSortDirection.Descending);
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Финальная стоимость"], System.ComponentModel.ListSortDirection.Descending);
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Дата продажи"], System.ComponentModel.ListSortDirection.Descending);
                }



            }
            else if (table == "products")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Наименование товара"], System.ComponentModel.ListSortDirection.Descending);
                }
                if (comboBox1.SelectedIndex == 1)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Категория"], System.ComponentModel.ListSortDirection.Descending);
                }
                if (comboBox1.SelectedIndex == 2)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Поставщик"], System.ComponentModel.ListSortDirection.Descending);
                }
                if (comboBox1.SelectedIndex == 3)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Цена"], System.ComponentModel.ListSortDirection.Descending);
                }
                if (comboBox1.SelectedIndex == 4)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Остаток на складе"], System.ComponentModel.ListSortDirection.Descending);
                }
            }
            else if (table == "users")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Логин"], System.ComponentModel.ListSortDirection.Descending);
                }
                if (comboBox1.SelectedIndex == 1)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Роль"], System.ComponentModel.ListSortDirection.Descending);
                }
            }
            else if (table == "suppliers")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Наименование компании"], System.ComponentModel.ListSortDirection.Descending);
                }
                if (comboBox1.SelectedIndex == 1)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Электронная почта"], System.ComponentModel.ListSortDirection.Descending);
                }
                if (comboBox1.SelectedIndex == 2)
                {
                    dataGridView1.Sort(dataGridView1.Columns["Контактный телефон"], System.ComponentModel.ListSortDirection.Descending);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            folderBrowserDialog.Description = "Выберите папку для сохранения";
            folderBrowserDialog.ShowNewFolderButton = true;
            string projectRoot = AppDomain.CurrentDomain.BaseDirectory;
            folderBrowserDialog.SelectedPath = Path.Combine(projectRoot, "dumps");

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;
                if (dbhelper.CreateDump(selectedPath))
                {
                    MessageBox.Show("Резерная копия успешно создана! \n Её можно найти в папке dumps", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void loadSales(object sender, EventArgs e)
        {
            comboBox1.Items.Add("По продавцу");
            comboBox1.Items.Add("По сумме");
            comboBox1.Items.Add("По дате");
            query = viewsalesquery + viewsalesqueryend;
            table = "sales";
            id_string = "sale_id";
            dbhelper.LoadDataToDGV(dataGridView1, query, pageNumber, pageSize);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            textBox1.Text = "";

        }

        private void loadProducts(object sender, EventArgs e) {
            comboBox1.Items.Add("По наименованию");
            comboBox1.Items.Add("по категории");
            comboBox1.Items.Add("По поставщику");
            comboBox1.Items.Add("По цене");
            comboBox1.Items.Add("По отатку на складе");
            query = viewproductsquery;
            table = "products";
            id_string = "product_id";

            dbhelper.LoadDataToDGV(dataGridView1, query, pageNumber, pageSize);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns[0].Visible = false;
            textBox1.Text = "";

        }

        private void loadUsers(object sender, EventArgs e) {
            comboBox1.Items.Add("По логину");
            comboBox1.Items.Add("По роли");
            query = viewusersquery;
            table = "users";
            id_string = "user_id";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dbhelper.LoadDataToDGV(dataGridView1, query, pageNumber, pageSize);
            dataGridView1.Columns[0].Visible = false;
            textBox1.Text = "";
        }

        private void loadCategories(object sender, EventArgs e) {
            comboBox1.Items.Add("По наименованию");
            query = viewcategoriesquery;
            table = "categories";
            id_string = "category_id";
            dbhelper.LoadDataToDGV(dataGridView1, query, pageNumber, pageSize);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns[0].Visible = false;
            textBox1.Text = "";
        }

        private void loadSuppliers(object sender, EventArgs e) {
            comboBox1.Items.Add("По наименованию");
            comboBox1.Items.Add("По электоронной почте");
            comboBox1.Items.Add("По телефону");
            query = "SELECT supplier_id, supplier_name as 'Наименование компании', contact_email as 'Электронная почта', phone as 'Контактный телефон' FROM suppliers";
            table = "suppliers";
            id_string = "supplier_id";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dbhelper.LoadDataToDGV(dataGridView1, query, pageNumber, pageSize);
            textBox1.Text = "";
            dataGridView1.Columns[0].Visible = false;
        }

        private void paginate(object sender, EventArgs e)
        {

            var oldflowPanel = tableLayoutPanel2.Controls.OfType<FlowLayoutPanel>()
                             .FirstOrDefault(flp => flp.Name == "flp1");

            if (oldflowPanel != null)
            {
                tableLayoutPanel2.Controls.Remove(oldflowPanel);
                oldflowPanel.Dispose();
            }

            FlowLayoutPanel flowPanel = new FlowLayoutPanel();
            flowPanel.Name = "flp1";
            flowPanel.Dock = DockStyle.Fill;
            tableLayoutPanel2.Controls.Add(flowPanel, 0, 3);

            Button decrBut = new Button();
            decrBut.Text = "<<";
            decrBut.Height = flowPanel.Height - 8;
            decrBut.Width = flowPanel.Height*2 - 8;
            decrBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            decrBut.Click += decrButtonClick;
            flowPanel.Controls.Add(decrBut);

            int buttonsCount = dbhelper.maxPage;
            for (int i = 0; i < buttonsCount; i++)
            {
                Button button = new Button();
                button.Text = (i + 1).ToString();
                button.Click += pageButtonClick;
                button.Name = i.ToString() + "_paginateBtn";
                button.Height = flowPanel.Height - 8;
                button.Width = flowPanel.Height - 8;
                button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                flowPanel.Controls.Add(button);
            }

            Button implBut = new Button();
            implBut.Text = ">>";
            implBut.Height = flowPanel.Height - 8;
            implBut.Width = flowPanel.Height*2 - 8;
            implBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            implBut.Click += implButtonClick;
            flowPanel.Controls.Add(implBut);

        }
        private void pageButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Match match = Regex.Match(button.Name, @"\d+");
            if (match.Success)
            {
                pageNumber = int.Parse(match.Value) + 1;
            }

            load(sender, e);
        }

        private void decrButtonClick(object sender, EventArgs e)
        {
            if (pageNumber - 1 != 0)
            {
                pageNumber--;
            }
            else
            {
                MessageBox.Show("Вы достигли лимита страниц!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            load(sender, e);
        }

        private void implButtonClick(object sender, EventArgs e)
        {
            if (pageNumber + 1 <= dbhelper.maxPage)
            {
                pageNumber++;
            }
            else
            {
                MessageBox.Show("Вы достигли лимита страниц!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            load(sender, e);
        }

        private void FormClose(object sender, FormClosingEventArgs e)
        {
            if (isProgrammaticClose)
            {
                this.DialogResult = DialogResult.Abort;
            }
        }

    }
}