using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SmokeCover: Entity
    {
        public SmokeCover()
        {
            Position = new Vector(0, 0);
        }
        public override Vector Position { get; set; }
        public int Width { get; set; }
        public int MaxPeopleAmount { get; set; }
        public int PeopleAmount { get; set; }
    }
}
