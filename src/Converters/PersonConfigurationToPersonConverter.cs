using Common;
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
    public class PersonConfigurationToPersonConverter
    {
        public static Person Convert(PersonConfiguration configuration)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var sexIndex = random.Next(0, configuration.Sex.Length);
            var ptIndex = random.Next(0, configuration.PersonalityPsychotype.Length);
            var sex = configuration.Sex[sexIndex];
            var age = random.Next(configuration.Age.Down, configuration.Age.Up + 1);
            var bt = new Bitmap(Image.FromFile(configuration.Icon.Path), configuration.Icon.Width, configuration.Icon.Height);
            
            return new Person
            {
                MaxSpeed = PersonSpeed.GetMaxSpeed(age, sex),
                ComfortableSpeed = PersonSpeed.GetComfortableSpeed(age, sex),
                Sex = sex,
                PersonalityPsychotype = configuration.PersonalityPsychotype[ptIndex],
                Icon = Icon.FromHandle(bt.GetHicon()),
                IsInVehicle = true,
            };
        }
    }
}
