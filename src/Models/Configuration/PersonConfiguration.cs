using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Configuration
{
    public class PersonConfiguration
    {
        public IconCongifuration Icon { get; set; }
        // defines how frequently this driver type will be generated
        public int GenerationWeight { get; set; }
        public Sex[] Sex { get; set; }
        public PersonalityPsychotype[] PersonalityPsychotype { get; set; }
        public DecimalRange Age { get; set; }
    }
}
