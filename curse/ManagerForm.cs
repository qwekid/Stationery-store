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

namespace curse
{
    public partial class ManagerForm : Form
    {
        private static string query = string.Empty;

        private static string table = string.Empty;
        private static string id_string = string.Empty;


        private static readonly string viewproductsquery = "SELECT p.product_id, p.product_name AS 'Наименование товара', c.category_name AS 'Категория', s.supplier_name AS 'Поставщик', p.price AS 'Цена', p.stock AS 'Остаток на складе' FROM products p JOIN categories c ON p.category_id = c.category_id JOIN suppliers s ON p.supplier_id = s.supplier_id";
        private static readonly string viewcategoriesquery = "SELECT category_id,  category_name as 'Наименование категории', description as 'Описание категории' FROM categories";
        private static readonly string viewsalesquery = "SELECT \r\n u.username AS \"Продавец\", \r\n    GROUP_CONCAT(CONCAT(p.product_name, ': ', c.quantity, ' шт.') SEPARATOR '; ') AS \"Товары\", \r\n    s.sale_date AS \"Дата продажи\", \r\n    s.total_amount AS \"Финальная стоимость\" \r\nFROM \r\n    `check` c \r\nJOIN \r\n    sales s ON c.sales_sale_id = s.sale_id \r\nJOIN  \r\n    products p ON c.products_product_id = p.product_id \r\nJOIN \r\n    users u ON s.user_id = u.user_id ";
        private static readonly string viewsalesqueryend = "GROUP BY \r\n s.sale_id, u.username, s.sale_date, s.total_amount";
        private ContextMenuStrip contextMenuStrip;

        public ManagerForm()
        {
            InitializeComponent();
            this.FormClosing += FormClose;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            query = "SELECT p.product_name AS 'Наименование товара', c.category_name AS 'Категория', s.supplier_name AS 'Поставщик', p.price AS 'Цена', p.stock AS 'Остаток на складе' FROM products p JOIN categories c ON p.category_id = c.category_id JOIN suppliers s ON p.supplier_id = s.supplier_id;";
            dbhelper.LoadDataToDGV(dataGridView1, query);
        }

        private void ManagerForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("По наименованию");
            comboBox1.Items.Add("по категории");
            comboBox1.Items.Add("По поставщику");
            comboBox1.Items.Add("По цене");
            comboBox1.Items.Add("По отатку на складе");
            query = viewproductsquery;
            table = "products";
            id_string = "product_name";
            dbhelper.LoadDataToDGV(dataGridView1, query);
            dataGridView1.Columns[0].Visible = false;
            contextMenuStrip = new ContextMenuStrip();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            dbhelper.LoadDataToDGV(dataGridView1, query);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            query = "SELECT \r\n    u.username AS 'Продавец',\r\n    GROUP_CONCAT(CONCAT(p.product_name, ': ', c.quantity, ' шт.') SEPARATOR '; ') AS 'Товары',\r\n    s.sale_date AS 'Дата продажи',\r\n    s.total_amount AS 'Финальная стоимость' \r\nFROM \r\n    `check` c \r\n \r\nJOIN \r\n    sales s ON c.sales_sale_id = s.sale_id\r\nJOIN \r\n    products p ON c.products_product_id = p.product_id \r\nJOIN \r\n    users u ON s.user_id = u.user_id\r\nGROUP BY \r\n    s.sale_id, u.username, s.sale_date, s.total_amount\r\n";
            dbhelper.LoadDataToDGV(dataGridView1, query);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateOrder a = new CreateOrder();
            a.ShowDialog();
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
        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            string txt = textBox1.Text;
            if (table == "products") { query = viewproductsquery + $" WHERE p.product_name LIKE '%{txt}%' OR c.category_name LIKE '%{txt}%' OR s.supplier_name LIKE '%{txt}%' OR p.price LIKE '%{txt}%' OR p.stock LIKE '%{txt}%';"; }
            else if (table == "categories") { query = viewcategoriesquery + $" WHERE category_name LIKE '%{txt}%' OR description LIKE '%{txt}%'"; }
            else if (table == "sales") { query = viewsalesquery + $" WHERE \r\n    u.username LIKE '%{txt}%' \r\n    OR p.product_name LIKE '%{txt}%' \r\n    OR c.quantity LIKE '%{txt}%' \r\n    OR s.sale_date LIKE '%{txt}%' \r\n    OR s.total_amount LIKE '%{txt}%'" + viewsalesqueryend; }
            else if (table == "suppliers") { query = $"SELECT supplier_name as 'Наименование компании', contact_email as 'Электронная почта', phone as 'Контактный телефон' FROM suppliers Where supplier_name Like '%{txt}%' OR contact_email Like '%{txt}%' OR phone Like '%{txt}%'"; }

            dbhelper.LoadDataToDGV(dataGridView1, query);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
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
            dbhelper.LoadDataToDGV(dataGridView1, query);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            textBox1.Text = "";
        }

        private void button7_Click_1(object sender, EventArgs e)
        {

            comboBox1.Items.Clear();
            comboBox1.Items.Add("По наименованию");
            comboBox1.Items.Add("по категории");
            comboBox1.Items.Add("По поставщику");
            comboBox1.Items.Add("По цене");
            comboBox1.Items.Add("По отатку на складе");
            query = viewproductsquery;
            table = "products";
            id_string = "product_id";

            dbhelper.LoadDataToDGV(dataGridView1, query);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns[0].Visible = false;
            textBox1.Text = "";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ReportForm r = new ReportForm();
            r.ShowDialog();
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

                bool dumpCreated = dbhelper.CreateDump();

                Cursor.Current = Cursors.Default;

                if (!dumpCreated)
                {
                    MessageBox.Show("Не удалось создать резервную копию!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }

                Application.Exit();
            }
            else if (dialogResult == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                Application.Exit();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            switch (table)
            {
                case "sales":
                    CreateOrder sales = new CreateOrder();
                    if (sales.ShowDialog() == DialogResult.OK)
                    {

                        this.ManagerForm_Load(sender, e);
                    }

                    break;
                case "products":
                    CreateProduct products = new CreateProduct();
                    if (products.ShowDialog() == DialogResult.OK)
                    {
                        this.ManagerForm_Load(sender, e);
                    }
                    break;
                case "categories":
                    CreateCategory c = new CreateCategory();
                    if (c.ShowDialog() == DialogResult.OK)
                    {
                        this.ManagerForm_Load(sender, e);
                    }
                    break;
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
