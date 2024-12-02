using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;

namespace curse
{
    public class dbhelper
    {
        public static int maxTC = 0;
        private static string connectionString = "server=localhost;user=root;database=officesupplies;password=root;";
        static public void LoadDataToDGV(DataGridView dataGridView, string query)
        {
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = connectionString;
                con.Open();

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                dataGridView.DataSource = dt;
                con.Close();
            }
        }

        

        static public int CheckUserRole(string login, string password)
        {
            try
            {
                int role = -1;

                MySqlConnection con = new MySqlConnection(connectionString);
                con.Open();

                MySqlCommand cmd = new MySqlCommand($"Select * From users Where username = '{login}'", con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (password == dt.Rows[0].ItemArray.GetValue(3).ToString())
                {
                    if (Convert.ToInt32(dt.Rows[0].ItemArray.GetValue(4)) == 2)
                    {
                        role = 2;
                    }
                    if (Convert.ToInt32(dt.Rows[0].ItemArray.GetValue(4)) == 1)
                    {
                        role = 1;
                    }

                    UserInfo.id = Convert.ToInt32(dt.Rows[0].ItemArray.GetValue(0));
                    UserInfo.userName = dt.Rows[0].ItemArray.GetValue(1).ToString();
                    UserInfo.email = dt.Rows[0].ItemArray.GetValue(2).ToString();
                    UserInfo.password = dt.Rows[0].ItemArray.GetValue(3).ToString();
                    UserInfo.role = Convert.ToInt32(dt.Rows[0].ItemArray.GetValue(4));
                }
                else return 0;
                con.Close();
                return role;
            }
            catch(IndexOutOfRangeException) { return 0; }
        }

        static public void LoadDataToDt(DataTable dt, string query)
        {
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = connectionString;
                con.Open();

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                
                da.Fill(dt);

                con.Close();
            }
        }

        static public long InsertDataOnDb(string query)
        {
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = connectionString;
                con.Open();

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();
                
                con.Close();
                return cmd.LastInsertedId;
            }
        }

        static public int AddProductRowToDGV(DataGridView dataGridView, string query, int quantity)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                // Получаем значение из первого столбца
                var newValue = dt.Rows[0].ItemArray[0];

                // Проверяем, существует ли уже строка с таким же значением в первом столбце
                bool rowExists = false;
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells[0].Value.ToString() == newValue.ToString())
                    {
                        return 0;
                    }
                }
                // Если строки с таким значением нет, добавляем новую
                if (!rowExists)
                {
                    dataGridView.Rows.Add(dt.Rows[0].ItemArray.GetValue(0), dt.Rows[0].ItemArray.GetValue(1), dt.Rows[0].ItemArray.GetValue(2), dt.Rows[0].ItemArray.GetValue(3), quantity);
                    
                }

                con.Close();
                return 1;
            }
        }
    }
}
