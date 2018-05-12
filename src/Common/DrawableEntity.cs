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
        public DrawableEntity()
        {
            Position = new Vector(0, 0);
        }
        public abstract void Render(IDrawWrapper drawContex);
    }
}
