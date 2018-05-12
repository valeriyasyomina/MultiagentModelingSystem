using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class DecimalRange
    {
        public int Up { get; set; }
        public int Down { get; set; }

        public override bool Equals(object obj)
        {
            var k = obj as DecimalRange;
            if (k != null)
            {
                return Up == k.Up && Down == k.Down;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Up + Down;
        }
    }
}
