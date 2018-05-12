using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Configuration
{
    public class AgeDeltaTime
    {
        public DecimalRange AgeRange { get; set; }
        // диапазон времени в секундах, который человек может находиться в дыму
        public int DeltaTime { get; set; }
    }
    public class PersonConfiguration
    {
        public IconCongifuration Icon { get; set; }
        // defines how frequently this driver type will be generated
        public int GenerationWeight { get; set; }
        public Sex[] Sex { get; set; }
        public PersonalityTemperament[] PersonalityTemperament { get; set; }
        public DecimalRange Age { get; set; }
        public Dictionary<Sex, int> MakeDesicionTime { get; set; }
        public Dictionary<Sex, int> DisorientationTime { get; set; }
        public AgeDeltaTime[] InSmokeDeltaTime { get; set; }
    }
}
