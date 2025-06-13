using System;
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
    public partial class Captcha : Form
    {
        private string captchaText = String.Empty;

        public Captcha()
        {
            InitializeComponent();
            textBox1.Height = button1.Height;
        }

        private void Captcha_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = CreateCaptchaImage(pictureBox1.Width, pictureBox1.Height);
        }

        private Bitmap CreateCaptchaImage(int width, int height)
        {
            Random rnd = new Random();
            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);

            
            g.Clear(Color.LightGray);

            
            string chars = "1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
            captchaText = "";
            for (int i = 0; i < 5; i++)
                captchaText += chars[rnd.Next(chars.Length)];

            
            int xPos = rnd.Next(height/2);
            int yPos = rnd.Next(width/2);

            // кисти для текста
            Brush[] brushes = { Brushes.Black, Brushes.Red, Brushes.Blue, Brushes.Green };

            
            g.DrawString(captchaText, new Font("Arial", 35, FontStyle.Bold), brushes[rnd.Next(brushes.Length)], new PointF(xPos, yPos));

            // линии
            Pen[] pens = { Pens.Black, Pens.Red, Pens.Blue, Pens.Green };
            for (int i = 0; i < 3; i++)
            {
                Point p1 = new Point(rnd.Next(width), rnd.Next(height));
                Point p2 = new Point(rnd.Next(width), rnd.Next(height));
                g.DrawLine(pens[rnd.Next(pens.Length)], p1, p2);
            }

            // точки
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (rnd.Next(20) == 0)
                        bitmap.SetPixel(i, j, Color.White);

            g.Dispose();
            return bitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = CreateCaptchaImage(pictureBox1.Width, pictureBox1.Height);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == captchaText)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Вы неправильно ввели CAPTCHA", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
