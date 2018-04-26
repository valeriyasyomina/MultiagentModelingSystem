using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Configuration
{
    public enum TravelDirection { LEFT_TO_RIGHT, RIGHT_TO_LEFT }
    public class VehicleIconConfiguration: IconCongifuration
    {
        public TravelDirection Direction { get; set; }
    }
}
