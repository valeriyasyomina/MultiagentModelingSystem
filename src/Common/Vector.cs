using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Vector
    {
        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }

        public static double Distance(Vector vector1, Vector vector2)
        {
            return Math.Sqrt((vector1.X - vector2.X) * (vector1.X - vector2.X) + (vector1.Y - vector2.Y) * (vector1.Y - vector2.Y));
        }
    }
}
