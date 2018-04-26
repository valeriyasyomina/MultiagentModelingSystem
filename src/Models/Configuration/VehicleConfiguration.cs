using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Configuration
{
    public class VehicleConfiguration
    {
        public DecimalRange Speed { get; set; }
        public DecimalRange Weight { get; set; }
        public DecimalRange PassengersNumber { get; set; }
        // defines how frequently this vehicle will be generated
        public int GenerationWeight { get; set; }      
        public VehicleType Type { get; set; }
        public List<VehicleIconConfiguration> IconsConfiguration { get; set; }
        public List<int> LenghtList { get; set; }
    }
}
