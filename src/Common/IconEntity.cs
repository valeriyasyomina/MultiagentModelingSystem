using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawWrapperLib;

namespace Common
{
    public class IconEntity: DrawableEntity
    {
        public Icon Icon { get; set; }
        public override Vector Position { get; set; } 

        public override void Render(IDrawWrapper drawContex)
        {
            drawContex.DrawIcon(Icon, Position.X - Icon.Width / 2, Position.Y - Icon.Height / 2);
        }
    }
}
