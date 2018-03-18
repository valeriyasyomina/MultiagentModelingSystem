using DrawWrapperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public abstract class DrawableEntity: Entity
    {
        public abstract int X { get; set; }
        public abstract int Y { get; set; }
    
        public abstract void Render(IDrawWrapper drawContex);
    }
}
