using Common;
using DrawWrapperLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Fire: IconEntity
    {
        private const int SECONDS_IN_MINUTE = 60;
        // fire stages duration times in minutes
        private const int FIRST_PHASE_DURATION = 3;
        private const int SECOND_PHASE_DURATION = 30;     

        private const int DELTA_MODELING_TIME = 7;
        private const double DELTA_RADIUS = 1.05;
       
        public override Vector Position { get; set; }
        public int Diamiter { get; set; }
        public int LastModelingTime { get; set; }
        public int GenerationTime { get; set; }

        public string IconPath { get; set; }

        public void Modeling(int currentTime)
        {
            if (currentTime - LastModelingTime == DELTA_MODELING_TIME)
            {
                LastModelingTime = currentTime;               
                if (IsFirstaPhase(currentTime))
                {
                    int width = (int)(Icon.Width * DELTA_RADIUS);
                    int height = (int)(Icon.Height * DELTA_RADIUS);
                    var bt = new Bitmap(Image.FromFile(IconPath), width, height);
                    Icon = Icon.FromHandle(bt.GetHicon());        
                }
                else if (IsSecondStage(currentTime))
                {

                }
                else
                {
                    int width = (int)(Icon.Width / DELTA_RADIUS);
                    int height = (int)(Icon.Height / DELTA_RADIUS);
                    var bt = new Bitmap(Image.FromFile(IconPath), width, height);
                    Icon = Icon.FromHandle(bt.GetHicon());           
                }                
            } 
        }

        private bool IsFirstaPhase(int currentTime)
        {
            return currentTime - GenerationTime <= FIRST_PHASE_DURATION * SECONDS_IN_MINUTE;
        }

        private bool IsSecondStage(int currentTime)
        {
            return currentTime - GenerationTime <= FIRST_PHASE_DURATION * SECONDS_IN_MINUTE + SECONDS_IN_MINUTE * SECONDS_IN_MINUTE;
        }
    }
}
