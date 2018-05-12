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
    public class IconConvigurationToSmokeConverter
    {
        public static Smoke Convert(IconCongifuration configuration)
        {
            var bt = new Bitmap(Image.FromFile(configuration.Path), configuration.Width, configuration.Height);
            return new Smoke
            {
                Icon = Icon.FromHandle(bt.GetHicon()),
                IconPath = configuration.Path,
            };
        }
    }
}
