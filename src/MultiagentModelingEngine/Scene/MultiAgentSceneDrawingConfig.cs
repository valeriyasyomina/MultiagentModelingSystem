using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiagentModelingEngine.Scene
{
    public class MultiAgentSceneDrawingConfig
    {
        public Color BackgroundColor { get; set; }
        public Color RoadwayDelimiterColor { get; set; }
        public Color RoadBorderColor { get; set; }
        public Color SmokeCoverColor { get; set; }
        public int SmokeCoversNumber { get; set; }
        public int RoadwaysNumber { get; set; }
        public int RoadwayDelimiterWidth { get; set; }
        public int SmokeCoverWidth { get; set; }
        public int RoadBorderWidth { get; set; }      
        public int Scale { get; set; }

        public static MultiAgentSceneDrawingConfig ReadFromFile(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException($"{fileName}");
            return JsonConvert.DeserializeObject<MultiAgentSceneDrawingConfig>(File.ReadAllText(fileName));
        }
    }
}
