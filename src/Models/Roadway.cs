using RandomValueGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Roadway
    {
        public Roadway()
        {
            Vehicles = new List<Vehicle>();
        }
        public List<Vehicle> Vehicles { get; set; }
        public IRandomValueGenerator RandomValueGenerator { get; set; }
    }
}
