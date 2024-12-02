using Microsoft.Office.Interop.Word;
using MySqlX.XDevAPI.Relational;
using System;
using System.Data;
using System.Windows.Forms;

namespace curse
{
    public partial class CreateOrder : Form
    {
        private static string query = string.Empty;
        private static int totalAmount = 0;
        private static int[] productCount = new int[0];


        private ContextMenuStrip contextMenuStrip;
        public CreateOrder()
        {
            InitializeComponent();

            contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem deleteRowMenuItem = new ToolStripMenuItem("Удалить строку");
            deleteRowMenuItem.Click += DeleteRowMenuItem_Click;
            contextMenuStrip.Items.Add(deleteRowMenuItem);
            this.ControlBox = false;
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            totalAmount = 0;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else { e.Handled = false; }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void CreateOrder_Load(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            comboBox1.Items.Clear();
            query = "SELECT product_name FROM products";
            dbhelper.LoadDataToDt(dt, query);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBox1.Items.Add(dt.Rows[i].ItemArray.GetValue(0).ToString());
                int[] np = new int[productCount.Length + 1];
                for (int x = 0; x < productCount.Length;)
                {
                    np[x] = productCount[x];
                    np[np.Length - 1] = 0;
                    x++;
                }
                productCount = np;
            }

            label2.Text = 0.ToString();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            int totalamount =0;
            if (label2.Text != "") { totalamount = Convert.ToInt32(label2.Text); }
            if (comboBox1.SelectedIndex != -1 && textBox2.Text != "")
            {
                string productName = comboBox1.SelectedItem.ToString();
                int quantity = Convert.ToInt32(textBox2.Text);
                dbhelper.LoadDataToDt(dt, $"select price from products where product_name = '{productName}'");
                int price = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                query = $"select stock from products where product_name = '{productName}'";
                dbhelper.LoadDataToDt(dt, query);
                query = $"SELECT p.product_name AS 'Наименование товара', c.category_name AS 'Категория', s.supplier_name AS 'Поставщик', p.price AS 'Цена' FROM products p JOIN categories c ON p.category_id = c.category_id JOIN suppliers s ON p.supplier_id = s.supplier_id where product_name = '{productName}'";
                if (dbhelper.AddProductRowToDGV(dataGridView1, query, quantity) == 1)
                {
                    totalamount += price * quantity;
                    label2.Text = totalamount.ToString();
                }
            }

        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Проверяем, что двойной клик был внутри границ DataGridView
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Показываем контекстное меню
                contextMenuStrip.Show(dataGridView1, dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
            }
        }

        private void DeleteRowMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, какие строки выбраны в DataGridView и удаляем их
            if (dataGridView1.CurrentRow != null)
            {
                totalAmount = Convert.ToInt32(label2.Text);
                int price = Convert.ToInt32(dataGridView1.CurrentRow.Cells[3].Value);
                int quantity = Convert.ToInt32(dataGridView1.CurrentRow.Cells[4].Value);
                totalAmount = totalAmount - (price * quantity);
                label2.Text = totalAmount.ToString();
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                DateTime dateTime = DateTime.Now;
                string saleDate = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                int userId = UserInfo.id;
                totalAmount = Convert.ToInt32(label2.Text);

                string insertSalesrQuery = $"INSERT INTO `officesupplies`.`sales` (`user_id`, `sale_date`, `total_amount`) VALUES ('{userId}', '{saleDate}', '{totalAmount}');";
                long salesId = dbhelper.InsertDataOnDb(insertSalesrQuery);
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    dt = new System.Data.DataTable();
                    query = $"select product_id from products where product_name = '{row.Cells[0].Value}'";
                    dbhelper.LoadDataToDt(dt, query);
                    int quantity = Convert.ToInt32(row.Cells[4].Value);
                    int product_id = Convert.ToInt32(dt.Rows[0].ItemArray.GetValue(0));


                    query = $"Select stock from products where product_id = {product_id}";
                    dt.Clear();
                    dbhelper.LoadDataToDt(dt, query);
                    int stock = Convert.ToInt32(dt.Rows[0].ItemArray.GetValue(1));
                    int new_stock = stock - quantity;

                    if (new_stock < 0)
                    {
                        MessageBox.Show($"Количество товара: '{row.Cells[0].Value}', который вы хотите продать меньше, чем есть на складе ");
                        comboBox1.SelectedIndex = -1;
                        textBox2.Text = "";
                        label2.Text = "";
                        dataGridView1.Rows.Clear();
                        return;
                    }
                    else
                    {
                        string insertCheckQuery = $"INSERT INTO `officesupplies`.`check` (`products_product_id`, `sales_sale_id`, `quantity`) VALUES ('{product_id}', '{salesId}', '{quantity}');";
                        dbhelper.InsertDataOnDb(insertCheckQuery);

                        query = $"UPDATE `officesupplies`.`products` SET `stock` = '{new_stock}' WHERE (`product_id` = '{product_id}');";
                        dbhelper.InsertDataOnDb(query);

                        

                        //создание чека

                        Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                        wordApp.Visible = true; // Чтобы увидеть Word

                        // Создаем новый документ
                        Microsoft.Office.Interop.Word.Document document = wordApp.Documents.Add();

                        int rows = dataGridView1.Rows.Count + 2; // Увеличиваем на 1 для заголовков
                        int columns = 4;

                        Range headerRange = document.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                        headerRange.Text = $"Товарный чек по продаже №{salesId}";

                        Range range = document.Range();
                        Microsoft.Office.Interop.Word.Table table = document.Tables.Add(range, rows, columns);
                        table.Borders.Enable = 1; // Включаем границы таблицы


                        table.Rows[1].Cells[1].Range.Text = "Наименование товара";
                        table.Rows[1].Cells[2].Range.Text = "Стоимость товара";
                        table.Rows[1].Cells[3].Range.Text = "Количество товара";
                        table.Rows[1].Cells[4].Range.Text = "Итого";

                        

                        // Заполняем таблицу данными
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            table.Cell(i + 2, 1).Range.Text = dataGridView1.Rows[i].Cells[0].Value.ToString();
                            table.Cell(i + 2, 2).Range.Text = dataGridView1.Rows[i].Cells[3].Value.ToString();
                            table.Cell(i + 2, 3).Range.Text = dataGridView1.Rows[i].Cells[4].Value.ToString();
                            table.Cell(i + 2, 4).Range.Text = Convert.ToString(Convert.ToInt32(dataGridView1.Rows[i].Cells[4].Value) * Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value));
                        }

                        

                        table.Cell(dataGridView1.Rows.Count + 2, 4).Range.Text = totalAmount.ToString();

                        MessageBox.Show("Заказ успешно создан");

                        comboBox1.SelectedIndex = -1;
                        textBox2.Text = "";
                        dataGridView1.Rows.Clear();
                    }
                }
            }
            else { MessageBox.Show("Вы не добавили товаров!"); }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
