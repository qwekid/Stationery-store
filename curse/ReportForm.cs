using Microsoft.Office.Interop.Word;
using MySqlX.XDevAPI.Relational;
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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Word = Microsoft.Office.Interop.Word;

namespace curse
{
    public partial class ReportForm : Form
    {
        private static System.Data.DataTable dt = new System.Data.DataTable();
        public ReportForm()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {

            string query = "SELECT MIN(sale_date) FROM sales;";
            dbhelper.LoadDataToDt(dt, query);

            DateTime start = (DateTime)dt.Rows[0].ItemArray[0];
            DateTime end = DateTime.Now;
            monthCalendar1.MaxDate = end;
            monthCalendar1.MinDate = start;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query = $"SELECT u.username AS 'Продавец',GROUP_CONCAT(CONCAT(p.product_name, ': ', c.quantity, ' шт.') SEPARATOR '; ') AS 'Товары',s.sale_date AS 'Дата продажи',s.total_amount AS 'Финальная стоимость' FROM `check` c JOIN sales s ON c.sales_sale_id = s.sale_id JOIN products p ON c.products_product_id = p.product_id JOIN users u ON s.user_id = u.user_id WHERE s.sale_date >= '{monthCalendar1.SelectionStart.ToString("yyyy-MM-dd HH:mm:ss")}' AND s.sale_date <= '{monthCalendar1.SelectionEnd.ToString("yyyy-MM-dd HH:mm:ss")}' GROUP BY s.sale_id, u.username, s.sale_date, s.total_amount";

            dt = new System.Data.DataTable();
            dbhelper.LoadDataToDt(dt, query);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Не удаётся найти продажи за указанный период");
                return;
            }

            int rows = dt.Rows.Count + 1; // Увеличиваем на 1 для заголовков
            int columns = dt.Columns.Count;

            Word.Application wordApp = new Word.Application();
            wordApp.Visible = true; // Чтобы увидеть Word

            // Создаем новый документ
            Word.Document document = wordApp.Documents.Add();


            Range headerRange = document.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
            headerRange.Text = $"Отчет по продажам в периоде с {monthCalendar1.SelectionStart.ToString("dd-MM-yyyy")} по {monthCalendar1.SelectionEnd.ToString("dd-MM-yyyy")}";


            Range range = document.Range();
            Word.Table table = document.Tables.Add(range, rows, columns);
            table.Borders.Enable = 1; // Включаем границы таблицы

            table.Rows[1].Cells[1].Range.Text = "Продавец";
            table.Rows[1].Cells[2].Range.Text = "Товары";
            table.Rows[1].Cells[3].Range.Text = "Дата продажи";
            table.Rows[1].Cells[4].Range.Text = "Финальная стоимость";

            // Заполняем таблицу данными
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    table.Cell(i + 2, j + 1).Range.Text = dt.Rows[i][j].ToString();
                }
            }



            // Создаем полный путь к папке приложения в папке "docs"
            //string appFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Проверяем,_exists ли папка, и создаем ее, если нет
            //if (!Directory.Exists(appFolderPath))
            //{
             //   Directory.CreateDirectory(appFolderPath);
            //}

            // Указываем имя файла, который хотим сохранить
            string fileName = @"report" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".docx";

            // Полный путь к файлу
            //string filePath = Path.Combine(appFolderPath, fileName);
            //document.SaveAs2(filePath); // Сохраняем документ
            //document.Save();
            // Закрываем документ
            //document.Close();
            //wordApp.Quit(); // Закрываем приложение Word

            // Освобождаем COM объекты
            System.Runtime.InteropServices.Marshal.ReleaseComObject(document);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            document = null; // Помечаем объект для сборщика мусора
            wordApp = null;  // Помечаем объект для сборщика мусора
            GC.Collect(); // Запускаем сборщик мусора
        }
    }
}
