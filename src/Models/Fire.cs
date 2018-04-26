using Common;
using DrawWrapperLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Fire: IconEntity
    {
        private const int FIRE_DIST = 10;

        public override int X { get; set; }
        public override int Y { get; set; }
        public int Radius { get; set; }

        public override void Render(IDrawWrapper drawContex)
        {
            for (int i = 0; i < Radius; i++)
                drawContex.DrawIcon(Icon, X, Y);
        }
    }
}
