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
using System.Diagnostics;
using System.IO;
using Microsoft.Office.Interop.Word;
using System.Drawing.Printing;

namespace curse
{
    public class dbhelper
    {
        public static int maxPage;
        private static string connectionString = "server=localhost;user=root;database=officesupplies;password=root;";
        static public void LoadDataToDGV(DataGridView dataGridView, string query, int pageNumber, int pageSize)
        {
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = connectionString;
                con.Open();

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                System.Data.DataTable dt = new System.Data.DataTable();

                da.Fill(dt);
                dt = paginate(dt, pageNumber, pageSize);
                dataGridView.DataSource = dt;
                con.Close();
            }
        }

        private static System.Data.DataTable paginate(System.Data.DataTable dt,int pageNumber,int pageSize)
        {
            int totalRows = dt.Rows.Count;
            maxPage = (int)Math.Ceiling((double)totalRows / pageSize);
            return dt.AsEnumerable()
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .CopyToDataTable();
           
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
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);

                if (Hasher.VerifyPassword(password, dt.Rows[0].ItemArray.GetValue(3).ToString()))
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

        static public void LoadDataToDt(System.Data.DataTable dt, string query)
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
                System.Data.DataTable dt = new System.Data.DataTable();

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

        /// <summary>
        /// Создает дамп базы данных MySQL
        /// </summary>
        /// <returns>True если успешно, False в случае ошибки</returns>
        public static bool CreateDump(string selectedPath)
        {
            string server = "localhost";
            string database = "officesupplies";
            string userId = "root";
            string password = "root";
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string dumpDirPath = selectedPath;
            string outputFile = Path.Combine(dumpDirPath, $"db_dump_{timestamp}.sql");
            try
            {
                string mysqldumpPath = FindMysqldumpPath();

                if (string.IsNullOrEmpty(mysqldumpPath))
                {
                    throw new Exception("mysqldump не найден. Убедитесь, что MySQL Server установлен.");
                }
                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                string arguments = $"--host={server} --user={userId} --password={password} " +
                                   $"--opt --routines --triggers --events --single-transaction " +
                                   $"--result-file=\"{outputFile}\" {database}";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = mysqldumpPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Ошибка при создании дампа: {error}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Пытается найти путь к mysqldump.exe
        /// </summary>
        private static string FindMysqldumpPath()
        {
            string[] possiblePaths =
            {
            @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe",
            @"C:\Program Files\MySQL\MySQL Server 5.7\bin\mysqldump.exe",
            @"C:\Program Files (x86)\MySQL\MySQL Server 5.6\bin\mysqldump.exe",
            @"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysqldump.exe"
        };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            string pathVariable = Environment.GetEnvironmentVariable("PATH");
            foreach (string path in pathVariable.Split(';'))
            {
                string fullPath = Path.Combine(path, "mysqldump.exe");
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }
    }
}

