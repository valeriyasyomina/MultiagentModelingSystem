using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Configuration
{
    public class TunnelConfiguration
    {
        /// <summary>
        /// Tunnel length in meters
        /// </summary>
        public int Lenght { get; set; }
        public int SmokeCoversNumber { get; set; }
        public List<RoadwayConfiguration> RoadwaysConfiguration { get; set; }
        public IconCongifuration SmokeConfiguration { get; set; }
        public IconCongifuration FireConfiguration { get; set; }
    }
}
