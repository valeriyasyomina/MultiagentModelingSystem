using Models;
using Models.Configuration;
using MultiagentModelingEngine.Scene;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiagentModelingEngine.Configuration
{
    public class ModelingEngineConfiguration
    {
        public MultiAgentSceneDrawingConfig DrawingSceneConfiguration { get; set; }
        public TunnelConfiguration TunnelConfiguration { get; set; }            
        public List<VehicleConfiguration> VehicleTypesConfiguration { get; set; }
        public List<DriverConfiguration> DriverTypesConfiguration { get; set; }
        public int StartGenerationTime { get; set; }
        public int EndGenerationTime { get; set; }
        public int AccidentTime { get; set; }
        public int DetlaModelingTime { get; set; }
        public int Sleep { get; set; }

        public static ModelingEngineConfiguration ReadFromFile(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException($"{fileName}");
            return JsonConvert.DeserializeObject<ModelingEngineConfiguration>(File.ReadAllText(fileName));
        }
    }   
}
