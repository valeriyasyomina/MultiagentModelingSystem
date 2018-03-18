using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawWrapperLib;
using System.Drawing;

namespace Models
{
    public class Driver: DrawableEntity
    {
        public bool IsInVehicle { get; set; }

        public override int X { get; set; }
        public override int Y { get; set; }

        public Color DrawColor { get; set; }   
        public int DrawWidth { get; set; }

        public override void Render(IDrawWrapper drawContex)
        {
            throw new NotImplementedException();
        }
    }
}
