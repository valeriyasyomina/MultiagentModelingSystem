using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Tunnel
    {
        public int Length { get; set; }
        public List<Roadway> Roadways { get; set; }
    }
}
