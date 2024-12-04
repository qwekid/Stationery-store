using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace curse
{
    public static class Capcha
    {
        public static string GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string captcha = new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return captcha;
        }

        public static Bitmap DrawCaptcha(string captcha)
        {
            string capthastr1 = $"{captcha[0]}    {captcha[2]}";
            string capthastr2 = $"  {captcha[1]}      {captcha[3]}";
            int Width = 100;
            int Height = 40;

            int newX, newY;

            Random rnd = new Random();

            Bitmap bitmap = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                
                g.Clear(Color.White);
                Font font = new Font("Arial", 15, FontStyle.Bold);
                g.DrawString(capthastr1, font, Brushes.Black, new PointF(10, 5));
                g.DrawString(capthastr2, font, Brushes.Black, new PointF(10, 10));

                g.DrawLine(Pens.Black,
              new Point(0, 0),
              new Point(Width - 1, Height - 1));
                g.DrawLine(Pens.Black,
                           new Point(0, Height - 1),
                           new Point(Width - 1, 0));

                for (int i = 0; i < Width; ++i)
                {
                    for (int j = 0; j < Height; ++j)
                    {
                        if (rnd.Next() % 20 == 0)
                        {
                            bitmap.SetPixel(i, j, Color.White);
                        }

                        newX = (int)(Width + (10 * Math.Sin(Math.PI * Height / 64.0)));
                        newY = (int)(Height + (10 * Math.Cos(Math.PI * Width / 64.0)));
                        if (newX < 0 || newX >= Width) newX = 0;
                        if (newY < 0 || newY >= Height) newY = 0;
                        bitmap.SetPixel(newX, newY, Color.AliceBlue);
                    }
                }
                
            }
            return bitmap;
        }
    }
}
