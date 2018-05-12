using Models;
using Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converters
{
    public class TunnelConfigurationToTunnelConverter
    {
        public static Tunnel Convert(TunnelConfiguration configuration)
        {
            return new Tunnel()
            {
                People = new List<Person>(),
                RoadwayMatrix = new Common.RoadwayMatrix(),
                Length = configuration.Lenght,
                Roadways = configuration.RoadwaysConfiguration
                .Select(r => RoadwayConfigurationToRoadwayConverter.Convert(r)).ToList()
            };
        }
    }
}
