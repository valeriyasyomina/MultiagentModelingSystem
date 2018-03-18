using RandomValueGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Configuration
{
    public class RoadwayConfiguration
    {
        public RandomGeneratorType RandomGeneratorType { get; set; }
        public List<int> Parametrs { get; set; }
    }
}
