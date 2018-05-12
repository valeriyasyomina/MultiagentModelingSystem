using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Tunnel
    {
        public Tunnel()
        {
            People = new List<Person>();
        }
        public int Length { get; set; }
        public List<Roadway> Roadways { get; set; }
        public Fire Fire { get; set; }
        public Smoke Smoke { get; set; }
        public List<Person> People { get; set; }
        public RoadwayMatrix RoadwayMatrix { get; set; }
        public List<SmokeCover> SmokeCovers { get; set; }

        public List<Vehicle> GetVehicles()
        {
            return Roadways.SelectMany(r => r.Vehicles.Select(v => v)).ToList();
        }

        public SmokeCover FindSmokeCover(Vector position)
        {
            var covers = SmokeCovers.Where(s => s.Position.X == position.X && s.Position.Y == position.Y).ToList();
            return covers.Count != 0 ? covers[0] : null;
        }

        public int FindSmokeCoverIndex(Vector position)
        {
            return SmokeCovers.FindIndex(c => c.Position.X == position.X && c.Position.Y == position.Y);
        }

        public bool IsFire()
        {
            return Fire != null;
        }

        public bool IsSmoke()
        {
            return Smoke != null;
        }
    }
}
