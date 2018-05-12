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
        public const int DEFAULT_IN_SMOKE_DELTA_TIME = 1200;
        private static int GetInSmokeDeltaTime(PersonConfiguration configuration, int age)
        {
            int result = 0;
            var element = configuration.InSmokeDeltaTime.Where(e => age >= e.AgeRange.Down && age <= e.AgeRange.Up).ToList()[0];
            if (element != null)
                result = element.DeltaTime;
            return result;
        }
        public static Person Convert(PersonConfiguration configuration, bool considerPsychotype)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var sexIndex = random.Next(0, configuration.Sex.Length);
            var ptIndex = random.Next(0, configuration.PersonalityTemperament.Length);
            var sex = configuration.Sex[sexIndex];
            var age = random.Next(configuration.Age.Down, configuration.Age.Up + 1);
            var bt = new Bitmap(Image.FromFile(configuration.Icon.Path), configuration.Icon.Width, configuration.Icon.Height);

            var personalityTemperament = configuration.PersonalityTemperament[ptIndex];
            var makeDecisionDeltaTime = configuration.MakeDesicionTime[sex];
            var disorientationDeltaTime = configuration.DisorientationTime[sex];
            if (considerPsychotype)
            {
                if (personalityTemperament == PersonalityTemperament.CHOLERIC)
                {
                    makeDecisionDeltaTime /= 2;
                    disorientationDeltaTime /= 2;
                } else if (personalityTemperament == PersonalityTemperament.PHLEGMATIC)
                {
                    makeDecisionDeltaTime += 1;
                    disorientationDeltaTime *= 2;
                }
            }

            return new Person
            {
                MaxSpeed = PersonSpeed.GetMaxSpeed(age, sex),
                ComfortableSpeed = PersonSpeed.GetComfortableSpeed(age, sex),
                Sex = sex,
                Age = age,
                InSmokeDeltaTime = GetInSmokeDeltaTime(configuration, age),
                MakeDecisionDeltaTime = makeDecisionDeltaTime,
                DisorientationDeltaTime = disorientationDeltaTime,
                PersonalityTemperament = personalityTemperament,
                Icon = Icon.FromHandle(bt.GetHicon()),
                IsInVehicle = true,
            };
        }
    }
}
