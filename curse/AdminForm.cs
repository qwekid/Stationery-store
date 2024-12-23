﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace curse
{
    public partial class AdminForm : Form
    {
        private static string query = string.Empty;
        private static string table = string.Empty;
        private static string id_string = string.Empty;

        private ContextMenuStrip contextMenuStrip;
        public AdminForm()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("По наименованию");
            query = "SELECT category_name as 'Наименование категории', description as 'Описание категории' FROM categories";
            table = "categories";
            id_string = "category_name";
            dbhelper.LoadDataToDGV(dataGridView1, query);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            textBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("По продавцу");
            comboBox1.Items.Add("По сумме");
            comboBox1.Items.Add("По дате");
            query = "SELECT \r\n    u.username AS 'Продавец',\r\n    GROUP_CONCAT(CONCAT(p.product_name, ': ', c.quantity, ' шт.') SEPARATOR '; ') AS 'Товары',\r\n    s.sale_date AS 'Дата продажи',\r\n    s.total_amount AS 'Финальная стоимость' \r\nFROM \r\n    `check` c \r\n \r\nJOIN \r\n    sales s ON c.sales_sale_id = s.sale_id\r\nJOIN \r\n    products p ON c.products_product_id = p.product_id \r\nJOIN \r\n    users u ON s.user_id = u.user_id\r\nGROUP BY \r\n    s.sale_id, u.username, s.sale_date, s.total_amount\r\n";
            table = "sales";
            dbhelper.LoadDataToDGV(dataGridView1, query);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            textBox1.Text = "";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ReportForm r = new ReportForm();
            r.ShowDialog();
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("По наименованию");
            comboBox1.Items.Add("по категории");
            comboBox1.Items.Add("По поставщику");
            comboBox1.Items.Add("По цене");
            comboBox1.Items.Add("По отатку на складе");
            query = "SELECT p.product_name AS 'Наименование товара', c.category_name AS 'Категория', s.supplier_name AS 'Поставщик', p.price AS 'Цена', p.stock AS 'Остаток на складе' FROM products p JOIN categories c ON p.category_id = c.category_id JOIN suppliers s ON p.supplier_id = s.supplier_id;";
            table = "products";
            id_string = "product_name";
            dbhelper.LoadDataToDGV(dataGridView1, query);

            contextMenuStrip = new ContextMenuStrip();

        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            // Проверяем, что двойной клик был внутри границ DataGridView и выбраная таблица не "продажи"
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && table!= "sales")
            {
                if (table == "products") 
                {
                    DataTable dt = new DataTable();
                    int currendrowid = dataGridView1.CurrentRow.Index;


                    ToolStripMenuItem updateRowMenuItem = new ToolStripMenuItem("Редактировать строку");
                    updateRowMenuItem.Click += UpdateRowMenuItem_Click;
                    contextMenuStrip.Items.Clear();
                    contextMenuStrip.Items.Add(updateRowMenuItem);

                    query = $"select product_name from `check` c join products p on c.products_product_id = p.product_id where product_name = '{dataGridView1.Rows[currendrowid].Cells[0].Value}'";
                    dbhelper.LoadDataToDt(dt, query);
                    if (dt.Rows.Count > 0)
                    {
                        ToolStripMenuItem deleteRowMenuItem1 = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem1.Click += DeleteRowMenuItem_ClickError;
                        contextMenuStrip.Items.Add(deleteRowMenuItem1);

                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                        return;
                    }
                    else {
                        ToolStripMenuItem deleteRowMenuItem = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem.Click += DeleteRowMenuItem_Click;
                        contextMenuStrip.Items.Add(deleteRowMenuItem);
                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                    }


                }
                else if (table == "categories")
                {
                    DataTable dt = new DataTable();
                    int currendrowid = dataGridView1.CurrentRow.Index;
                    query = $"select category_name from `check` c join products p on c.products_product_id = p.product_id join categories cat on p.category_id = cat.category_id where product_name = '{dataGridView1.Rows[currendrowid].Cells[0].Value}' ";
                    dbhelper.LoadDataToDt(dt, query);

                    ToolStripMenuItem updateRowMenuItem = new ToolStripMenuItem("Редактировать строку");
                    updateRowMenuItem.Click += UpdateRowMenuItem_Click;
                    contextMenuStrip.Items.Clear();
                    contextMenuStrip.Items.Add(updateRowMenuItem);

                    if (dt.Rows.Count > 0)
                    {
                        ToolStripMenuItem deleteRowMenuItem1 = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem1.Click += DeleteRowMenuItem_ClickError;
                        contextMenuStrip.Items.Add(deleteRowMenuItem1);
                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                        return;
                    }
                    else
                    {
                        ToolStripMenuItem deleteRowMenuItem = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem.Click += DeleteRowMenuItem_Click;
                        contextMenuStrip.Items.Add(deleteRowMenuItem);
                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                    }
                }
                else if (table == "users")
                {
                    DataTable dt = new DataTable();
                    int currendrowid = dataGridView1.CurrentRow.Index;
                    query = $"select username from sales s join users u on s.user_id = u.user_id where username = '{dataGridView1.Rows[currendrowid].Cells[0].Value}' ";
                    dbhelper.LoadDataToDt(dt, query);

                    ToolStripMenuItem updateRowMenuItem = new ToolStripMenuItem("Редактировать строку");
                    updateRowMenuItem.Click += UpdateRowMenuItem_Click;
                    contextMenuStrip.Items.Clear();
                    contextMenuStrip.Items.Add(updateRowMenuItem);

                    if (dt.Rows.Count > 0)
                    {
                        ToolStripMenuItem deleteRowMenuItem1 = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem1.Click += DeleteRowMenuItem_ClickError;
                        contextMenuStrip.Items.Add(deleteRowMenuItem1);
                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                        return;
                    }
                    else
                    {
                        ToolStripMenuItem deleteRowMenuItem = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem.Click += DeleteRowMenuItem_Click;
                        contextMenuStrip.Items.Add(deleteRowMenuItem);
                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                    }
                }
                if (table == "suppliers")
                {
                    DataTable dt = new DataTable();
                    int currendrowid = dataGridView1.CurrentRow.Index;


                    ToolStripMenuItem updateRowMenuItem = new ToolStripMenuItem("Редактировать строку");
                    updateRowMenuItem.Click += UpdateRowMenuItem_Click;
                    contextMenuStrip.Items.Clear();
                    contextMenuStrip.Items.Add(updateRowMenuItem);

                    

                    query = $"select supplier_name from suppliers where supplier_name = '{dataGridView1.Rows[currendrowid].Cells[0].Value}'";
                    dbhelper.LoadDataToDt(dt, query);
                    if (dt.Rows.Count > 0)
                    {
                        ToolStripMenuItem deleteRowMenuItem1 = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem1.Click += DeleteRowMenuItem_ClickError;
                        contextMenuStrip.Items.Add(deleteRowMenuItem1);

                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                        return;
                    }
                    else
                    {
                        ToolStripMenuItem deleteRowMenuItem = new ToolStripMenuItem("Удалить строку");
                        deleteRowMenuItem.Click += DeleteRowMenuItem_Click;
                        contextMenuStrip.Items.Add(deleteRowMenuItem);
                        // Показываем контекстное меню
                        contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                    }


                }
                else
                {
                    ToolStripMenuItem updateRowMenuItem = new ToolStripMenuItem("Редактировать строку");
                    updateRowMenuItem.Click += UpdateRowMenuItem_Click;
                    contextMenuStrip.Items.Clear();
                    contextMenuStrip.Items.Add(updateRowMenuItem);

                    ToolStripMenuItem deleteRowMenuItem = new ToolStripMenuItem("Удалить строку");
                    deleteRowMenuItem.Click += DeleteRowMenuItem_Click;
                    contextMenuStrip.Items.Clear();
                    contextMenuStrip.Items.Add(deleteRowMenuItem);
                    // Показываем контекстное меню
                    contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                }
            }
            
        }

        private void UpdateRowMenuItem_Click(object sender, EventArgs e)
        {
            this.dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystroke;
            
        }
        
        private void DeleteRowMenuItem_ClickError(object sender, EventArgs e) {
            MessageBox.Show("Невозможно удалить эту строку!");
        }

        private void DeleteRowMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, какие строки выбраны в DataGridView и удаляем их
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                int currendrowid = dataGridView1.CurrentRow.Index - 1;
                dataGridView1.Rows.RemoveAt(currendrowid);
                query = $"DELETE FROM `officesupplies`.`{table}` WHERE (`{id_string}` = '{dataGridView1.Rows[currendrowid].Cells[0].Value}');";
                dbhelper.InsertDataOnDb(query);
                if (table == "products") { query = $"SELECT p.product_name AS \"Наименование товара\", c.category_name AS \"Категория\", s.supplier_name AS \"Поставщик\", p.price AS \"Цена\", p.stock AS \"Остаток на складе\" FROM products p JOIN categories c ON p.category_id = c.category_id JOIN suppliers s ON p.supplier_id = s.supplier_id;"; }
                else if (table == "categories") { query = $"SELECT category_name as 'Наименование категории', description as 'Описание категории' FROM categories"; }
                else if (table == "sales") { query = $" SELECT u.username AS 'Продавец',GROUP_CONCAT(CONCAT(p.product_name, ': ', c.quantity, ' шт.') SEPARATOR '; ') AS 'Товары',s.sale_date AS 'Дата продажи',s.total_amount AS 'Финальная стоимость' FROM sales s JOIN users u ON s.user_id = u.user_id JOIN `check` c ON s.check_check_id = c.sales_sale_id JOIN products p ON c.products_product_id = p.product_id;"; }
                else if (table == "users") { query = $"SELECT username as 'Логин', email as 'Почта', password as 'Пароль', r.role_name as 'Роль' FROM users u JOIN roles r ON u.role = r.id"; }
                else if (table == "suppliers") { query = $"SELECT supplier_name as 'Наименование компании', contact_email as 'Электронная почта', phone as 'Контактный телефон' FROM suppliers"; }
                dbhelper.LoadDataToDGV(dataGridView1, query);
            }

        }
        private void button4_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string txt = textBox1.Text;
            if (table == "products") { query = $"SELECT p.product_name AS \"Наименование товара\", c.category_name AS \"Категория\", s.supplier_name AS \"Поставщик\", p.price AS \"Цена\", p.stock AS \"Остаток на складе\" FROM products p JOIN categories c ON p.category_id = c.category_id JOIN suppliers s ON p.supplier_id = s.supplier_id WHERE p.product_name LIKE '%{txt}%' OR s.supplier_name LIKE '%{txt}%' OR p.price LIKE '%{txt}%' OR p.stock LIKE '%{txt}%';"; }
            else if (table == "categories") { query = $"SELECT category_name as 'Наименование категории', description as 'Описание категории' FROM categories Where category_name Like '%{txt}%'"; }
            else if (table == "sales") { query = $" SELECT u.username AS 'Продавец',GROUP_CONCAT(CONCAT(p.product_name, ': ', c.quantity, ' шт.') SEPARATOR '; ') AS 'Товары',s.sale_date AS 'Дата продажи',s.total_amount AS 'Финальная стоимость' FROM sales s JOIN users u ON s.user_id = u.user_id JOIN `check` c ON s.check_check_id = c.sales_sale_id JOIN products p ON c.products_product_id = p.product_id WHERE u.username LIKE '%{txt}%' OR p.product_name LIKE '%{txt}%' OR c.quantity LIKE '%{txt}%' OR s.sale_date LIKE '%{txt}%' OR s.total_amount LIKE '%{txt}%' GROUP BY s.sale_id, u.username, u.email, s.sale_date, s.total_amount;"; }
            else if (table == "users") { query = $"SELECT username as 'Логин', email as 'Почта', password as 'Пароль', r.role_name as 'Роль' FROM users u JOIN roles r ON u.role = r.id Where username Like '%{txt}%'"; ; }
            else if (table == "suppliers") { query = $"SELECT supplier_name as 'Наименование компании', contact_email as 'Электронная почта', phone as 'Контактный телефон' FROM suppliers Where supplier_name Like '%{txt}%' OR contact_email Like '%{txt}%' OR phone Like '%{txt}%'"; }
           

            dbhelper.LoadDataToDGV(dataGridView1, query);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("По наименованию");
            comboBox1.Items.Add("по категории");
            comboBox1.Items.Add("По поставщику");
            comboBox1.Items.Add("По цене");
            comboBox1.Items.Add("По отатку на складе");
            query = "SELECT p.product_name AS 'Наименование товара', c.category_name AS 'Категория', s.supplier_name AS 'Поставщик', p.price AS 'Цена', p.stock AS 'Остаток на складе' FROM products p JOIN categories c ON p.category_id = c.category_id JOIN suppliers s ON p.supplier_id = s.supplier_id";
            table = "products";
            id_string = "product_name";

            dbhelper.LoadDataToDGV(dataGridView1, query);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            textBox1.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("По логину");
            comboBox1.Items.Add("По роли");
            query = "SELECT username as 'Логин', email as 'Почта', password as 'Пароль', r.role_name as 'Роль' FROM users u JOIN roles r ON u.role = r.id";
            table = "users";
            id_string = "username";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dbhelper.LoadDataToDGV(dataGridView1, query);
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("По наименованию");
            comboBox1.Items.Add("По электоронной почте");
            comboBox1.Items.Add("По телефону");
            query = "SELECT supplier_id, supplier_name as 'Наименование компании', contact_email as 'Электронная почта', phone as 'Контактный телефон' FROM suppliers";
            table = "suppliers";
            id_string = "supplier_name";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dbhelper.LoadDataToDGV(dataGridView1, query);
            textBox1.Text = "";
            dataGridView1.Columns[0].Visible = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                else if (comboBox1.SelectedIndex == 1)
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

        private void button12_Click(object sender, EventArgs e)
        {
            if (dataGridView1.EditMode == DataGridViewEditMode.EditProgrammatically)
            {
                switch (table)
                {
                    case "sales":
                        CreateOrder sales = new CreateOrder();
                        if (sales.ShowDialog() == DialogResult.OK)
                        {
                            // Обновляем данные на первой форме
                            this.AdminForm_Load(sender, e);
                        }

                        break;
                    case "products":
                        CreateProduct products = new CreateProduct();
                        if (products.ShowDialog() == DialogResult.OK)
                        {
                            // Обновляем данные на первой форме
                            this.AdminForm_Load(sender, e);
                        }
                        break;
                    case "categories":
                        CreateCategory c = new CreateCategory();
                        if (c.ShowDialog() == DialogResult.OK)
                        {
                            // Обновляем данные на первой форме
                            this.AdminForm_Load(sender, e);
                        }
                        break;
                    case "users":
                        CreateUser users = new CreateUser();
                        if (users.ShowDialog() == DialogResult.OK)
                        {
                            // Обновляем данные на первой форме
                            this.AdminForm_Load(sender, e);
                        }
                        break;
                    case "suppliers":
                        CreateSuppliers sup = new CreateSuppliers();
                        if (sup.ShowDialog() == DialogResult.OK)
                        {
                            // Обновляем данные на первой форме
                            this.AdminForm_Load(sender, e);
                        }
                        break;

                }
            }
            else if (dataGridView1.EditMode == DataGridViewEditMode.EditOnKeystroke) {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    switch (table)
                    {
                        case "suppliers":
                            query = $"UPDATE `officesupplies`.`suppliers` SET `supplier_name` = '{row.Cells[1].Value}', `contact_email` = '{row.Cells[2].Value}', `phone` = '{row.Cells[3].Value}' WHERE (`supplier_id` = '{row.Cells[0].Value}');";
                            break;
                        case "products":

                            break;
                        case "categories":

                            break;
                        case "users":

                            break;

                    }
                }
                dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            }
        }
    }
}
