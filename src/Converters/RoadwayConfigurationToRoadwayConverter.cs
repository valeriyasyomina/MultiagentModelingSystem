using Models;
using Models.Configuration;
using RandomValueGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converters
{
    public class RoadwayConfigurationToRoadwayConverter
    {
        public static Roadway Convert(RoadwayConfiguration configuration)
        {
            var roadway = new Roadway();            
            switch (configuration.RandomGeneratorType)
            {
                case RandomValueGenerator.RandomGeneratorType.UNIFORM:
                    roadway.RandomValueGenerator = new UniformValueGenerator(configuration.Parametrs[0], configuration.Parametrs[1]);
                    break;
                default:
                    break;
            }
            return roadway;
        }
    }
}
