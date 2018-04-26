using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawWrapperLib
{
    public class PictureBoxDrawWrapper : IDrawWrapper
    {
        public PictureBox PictureBox { get; protected set; }
        public Graphics Graphics { get; protected set; }

        public int Width { get => PictureBox?.Width ?? 0; }
        public int Height { get => PictureBox?.Height ?? 0; }

        public PictureBoxDrawWrapper(PictureBox picture)
        {
            PictureBox = picture ?? throw new ArgumentNullException($"{picture}");
            Graphics = PictureBox.CreateGraphics();
        }

        public void DrawPoint(Pen pen, int x, int y)
        {
            Graphics.DrawEllipse(pen, x - pen.Width / 2, y - pen.Width / 2, pen.Width, pen.Width);
        }

        public void DrawIcon(Icon icon, int x, int y)
        {
            Graphics.DrawIcon(icon, x, y);
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            Graphics.DrawLine(pen, x1, y1, x2, y2);
        }

        public void SetBackgroundColor(Color color)
        {
            Graphics.Clear(color);
          //  PictureBox.BackColor = color;
        }

    }
}
