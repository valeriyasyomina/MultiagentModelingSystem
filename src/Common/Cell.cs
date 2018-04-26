using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Cell
    {
        public Cell(int x, int y)
        {
            Center = new Vector(x, y);
        }
        public bool BusyWithPerson { get; set; }
        public Vector Center { get; set; }
    }
}
