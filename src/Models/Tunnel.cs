using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Tunnel
    {
        public Tunnel()
        {
            People = new List<Person>();
        }
        public int Length { get; set; }
        public List<Roadway> Roadways { get; set; }
        public Fire Fire { get; set; }
        public IconEntity Smoke { get; set; }
        public List<Person> People { get; set; }

        public bool IsFire()
        {
            return Fire != null;
        }

        public bool IsSmoke()
        {
            return Smoke != null;
        }
    }
}
