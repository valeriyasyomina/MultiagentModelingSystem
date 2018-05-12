using Models;
using Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converters
{
    public class SmokeCoverConfigurationToSmokeCoverConverter
    {
        public static SmokeCover Convert(SmokeCoverConfiguration config)
        {
            return new SmokeCover()
            {
                MaxPeopleAmount = config.MaxPeopleAmount,
                PeopleAmount = 0,
                Width = config.Width,
            };
        }
    }
}
