using Models;
using Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converters
{
    public class DriverConfigurationToDriverConverter
    {
        public static Driver Convert(DriverConfiguration configuration)
        {
            return new Driver
            {
                DrawColor = configuration.DrawColor,
                DrawWidth = configuration.DrawWidth,
                IsInVehicle = true,
            };
        }
    }
}
