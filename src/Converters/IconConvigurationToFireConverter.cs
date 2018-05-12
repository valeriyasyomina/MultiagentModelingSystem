﻿using Common;
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
            var icon = Icon.FromHandle(bt.GetHicon());
            return new Fire
            {
                Icon = icon,
                Diamiter = icon.Width,
                IconPath = configuration.Path,
            };
        }
    }
}
