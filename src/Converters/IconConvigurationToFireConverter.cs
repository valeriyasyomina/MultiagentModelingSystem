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
    public class IconConvigurationToFireConverter
    {
        public static Fire Convert(IconCongifuration configuration)
        {
            var bt = new Bitmap(Image.FromFile(configuration.Path), configuration.Width, configuration.Height);
            return new Fire
            {
                Icon = Icon.FromHandle(bt.GetHicon()),
                Radius = 1,
            };
        }
    }
}
