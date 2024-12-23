﻿using System;
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
        private static int tablecounter;
        private static string query = string.Empty;
        public ManagerForm()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            query = "SELECT p.product_name AS 'Наименование товара', c.category_name AS 'Категория', s.supplier_name AS 'Поставщик', p.price AS 'Цена', p.stock AS 'Остаток на складе' FROM products p JOIN categories c ON p.category_id = c.category_id JOIN suppliers s ON p.supplier_id = s.supplier_id;";
            tablecounter = 0;
            dbhelper.LoadDataToDGV(dataGridView1, query);
        }

        private void ManagerForm_Load(object sender, EventArgs e)
        {
            query = "SELECT p.product_name AS 'Наименование товара', c.category_name AS 'Категория', s.supplier_name AS 'Поставщик', p.price AS 'Цена', p.stock AS 'Остаток на складе' FROM products p JOIN categories c ON p.category_id = c.category_id JOIN suppliers s ON p.supplier_id = s.supplier_id;";
            tablecounter = 0;
            dbhelper.LoadDataToDGV(dataGridView1, query);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (tablecounter > 0) { tablecounter--; }
            dbhelper.LoadDataToDGV(dataGridView1, query);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (tablecounter < dbhelper.maxTC - 1) { tablecounter++; }
            dbhelper.LoadDataToDGV(dataGridView1, query);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            query = "SELECT \r\n    u.username AS 'Продавец',\r\n    GROUP_CONCAT(CONCAT(p.product_name, ': ', c.quantity, ' шт.') SEPARATOR '; ') AS 'Товары',\r\n    s.sale_date AS 'Дата продажи',\r\n    s.total_amount AS 'Финальная стоимость' \r\nFROM \r\n    `check` c \r\n \r\nJOIN \r\n    sales s ON c.sales_sale_id = s.sale_id\r\nJOIN \r\n    products p ON c.products_product_id = p.product_id \r\nJOIN \r\n    users u ON s.user_id = u.user_id\r\nGROUP BY \r\n    s.sale_id, u.username, s.sale_date, s.total_amount\r\n";
            tablecounter = 0;
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
    }
}
