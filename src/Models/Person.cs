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
    public enum PersonState { ALIVE, DIED, DAMAGED, DISORIENTATED }
    public class Person : IconEntity
    {
        public bool IsInVehicle { get; set; }

        public override int X { get; set; }
        public override int Y { get; set; }

        public Sex Sex { get; set; }
        public PersonalityPsychotype PersonalityPsychotype { get; set; }
        public double MaxSpeed { get; set; }
        public double ComfortableSpeed { get; set; }
        public int Age { get; set; }

        public int LastDisorientationTime { get; set; }
        public int LastDecisionTime { get; set; }

        public PersonState State { get; set; }

        public int NearestSmokeCoverX { get; set; }
        public int NearestSmokeCoverY { get; set; }

        public bool NearestSmokeCoverDetected()
        {
            return NearestSmokeCoverX != 0 && NearestSmokeCoverY != 0;
        }

        public void DetectNearestSmokeCover(List<Vector> smokeCoversKoordinates)
        {
            int minDist = Int32.MaxValue;
            for (int i = 0; i < smokeCoversKoordinates.Count; i++)
            {
                var cover = smokeCoversKoordinates[i];
                var dist = Math.Sqrt((X - cover.X) * (X - cover.X) + (Y - cover.Y) * (Y - cover.Y));
                if (dist < minDist)
                {
                    NearestSmokeCoverX = cover.X;
                    NearestSmokeCoverY = cover.Y;
                }
            }
        }

        public void MakeDecision(Tunnel tunnel, int deltaTime, bool considerPsychotype = false)
        {
            
        }
    }
}
