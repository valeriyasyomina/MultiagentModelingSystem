using Models;
using Models.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converters
{
    public static class VehicleConfigurationToVehicleConverter
    {
        public static Vehicle Convert(VehicleConfiguration configuration, TravelDirection direction, Size iconSize)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var vehicle = new Vehicle()
            {
                Speed = random.Next(configuration.Speed.Down, configuration.Speed.Up + 1),
                PassengersNumber = random.Next(configuration.PassengersNumber.Down, configuration.PassengersNumber.Up + 1),
                Weight = random.Next(configuration.Weight.Down, configuration.Weight.Up + 1),
                Type = configuration.Type,
                Direction = direction,
            };
            var lengthIndex = random.Next(0, configuration.LenghtList.Count);
            vehicle.Length = configuration.LenghtList[lengthIndex];

            var iconPathes = configuration.IconsConfiguration.Where(c => c.Direction == direction).ToList();
            var iconPathIndex = random.Next(0, iconPathes.Count);
            var bt = new Bitmap(Image.FromFile(iconPathes[iconPathIndex].Path), iconSize.Width, iconSize.Height);
            vehicle.Icon = Icon.FromHandle(bt.GetHicon());
            return vehicle;
        }
    }
}
