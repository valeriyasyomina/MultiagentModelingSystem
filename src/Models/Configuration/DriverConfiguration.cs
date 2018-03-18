using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Configuration
{
    public class DriverConfiguration
    {
        /// Color to display on canvas, for drawing only
        /// </summary>
        public Color DrawColor { get; set; }
        /// <summary>
        /// Width to display on canvas as point, for drawing only
        /// </summary>
        public int DrawWidth { get; set; }
        // defines how frequently this driver type will be generated
        public int GenerationWeight { get; set; }
    }
}
