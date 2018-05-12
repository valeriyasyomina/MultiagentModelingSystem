using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawWrapperLib;
using System.Drawing;

namespace Models
{
    public class Smoke: IconEntity
    {
        private const int DELTA_MODELING_TIME = 3;
        private const double DELTA_RADIUS = 1.2;

        public override Vector Position { get; set; }
        public int LastModelingTime { get; set; }
        public string IconPath { get; set; }

        public void Modeling(int currentTime, int contextWidth, int contextHeight)
        {
            if (currentTime - LastModelingTime == DELTA_MODELING_TIME)
            {
                if (Icon.Height < contextHeight || Icon.Width < 2 * contextWidth)
                {                    
                    int width = (int)(Icon.Width * DELTA_RADIUS);
                    int height = (int)(Icon.Height * DELTA_RADIUS);
                    var bt = new Bitmap(Image.FromFile(IconPath), width, height);
                    Icon = Icon.FromHandle(bt.GetHicon());
                }
                LastModelingTime = currentTime;
            }
        }
    }
}
