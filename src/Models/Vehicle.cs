using Common;
using DrawWrapperLib;
using Models.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Vehicle: IconEntity
    {        
        public int Speed { get; set; }
        public int Weight { get; set; }
        public int PassengersNumber { get; set; }
        public Person Driver { get; set; }
        public VehicleType Type { get; set; }
        public TravelDirection Direction { get; set; }       
        public int Length { get; set; }

        public override int X { get; set; }
        public override int Y { get; set; }

        public Vehicle()
        {
            Driver = new Person();
        }        
    }
}
