using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawWrapperLib
{
    public interface IDrawWrapper
    {
        void DrawIcon(Icon icon, int x, int y);
        void SetBackgroundColor(Color color);
        void DrawLine(Pen pen, int x1, int y1, int x2, int y2);
        void DrawPoint(Pen pen, int x, int y);
        int Width { get; }
        int Height { get; }
    }
}
