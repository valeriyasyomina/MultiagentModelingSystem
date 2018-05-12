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
    // моделирование проводится либо по времени, либо по максимальному количеству человек (MaxPeopleAmount), если оно задано
    public class ModelingEngineConfiguration
    {
        public MultiAgentSceneDrawingConfig DrawingSceneConfiguration { get; set; }
        public TunnelConfiguration TunnelConfiguration { get; set; }            
        public List<VehicleConfiguration> VehicleTypesConfiguration { get; set; }
        public List<PersonConfiguration> PersonTypesConfiguration { get; set; }
        public int StartModelingTime { get; set; }
        public int EndModelingTime { get; set; }
        public int AccidentTime { get; set; }
        // максимальное количество людей, которое будет сгенерировано
        public int MaxPeopleAmount { get; set; }
        public int DetlaModelingTime { get; set; }
        public int Sleep { get; set; }
        public bool ConsiderTemperament { get; set; }
        public bool LeaveVehiclesImmediately { get; set; }

        public static ModelingEngineConfiguration ReadFromFile(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException($"{fileName}");
            return JsonConvert.DeserializeObject<ModelingEngineConfiguration>(File.ReadAllText(fileName));
        }
    }   
}
