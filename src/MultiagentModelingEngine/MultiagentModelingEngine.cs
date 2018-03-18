using Common;
using Converters;
using Models;
using Models.Configuration;
using MultiagentModelingEngine.Configuration;
using MultiagentModelingEngine.Scene;
using SceneRendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiagentModelingEngine
{
    public class MultiagentModelingEngine
    {
        public ModelingEngineConfiguration Configuration { get; protected set; }
        public ISceneRenderer SceneRenderer { get; protected set; }

        protected MultiAgentScene scene = null;
        protected List<int> gerenationTimes = new List<int>();
        protected Size vehicleIconSize = new Size();

        /// roadway width (helphull variables)
        protected int roadwayWidth = 0;
        protected int halfRoadwayWidth = 0;
        protected int scale = 0;

        public MultiagentModelingEngine(ModelingEngineConfiguration configuration, ISceneRenderer sceneRenderer)
        {
            Configuration = configuration ?? throw new ArgumentNullException($"{configuration}");
            SceneRenderer = sceneRenderer ?? throw new ArgumentNullException($"{sceneRenderer}");
        }

        public void InitializeMultiAgentScene()
        {
            SceneRenderer.InitializeContext();
            scene = new MultiAgentScene(TunnelConfigurationToTunnelConverter.Convert(Configuration.TunnelConfiguration));
            vehicleIconSize.Width = Configuration.DrawingSceneConfiguration.VehicleIconWidth;
            vehicleIconSize.Height = Configuration.DrawingSceneConfiguration.VehicleIconHeight;
            InitializeGenerationTimesList();

            roadwayWidth = SceneRenderer.ContextHeigth / Configuration.TunnelConfiguration.RoadwaysConfiguration.Count;
            halfRoadwayWidth = roadwayWidth / 2;
            scale = Configuration.TunnelConfiguration.Lenght / SceneRenderer.ContextWidth;
        }

        public void StartModeling()
        {
            int startTime = Configuration.StartGenerationTime;
            int endTime = Configuration.EndGenerationTime;           

            int time = startTime;
            while (time <= endTime)
            {
                GenerateVehicles(time);
                RenderScene();
                ModelingStep(time);
                Thread.Sleep(Configuration.Sleep);
                time += Configuration.DetlaModelingTime;
            }
        }

        private void RenderScene()
        {
            SceneRenderer.InitializeContext();
            SceneRenderer.Render(scene);
        }

        private void InitializeGenerationTimesList()
        {
            gerenationTimes = scene.Tunnel.Roadways.Select(r => r.RandomValueGenerator.GetNextGenerationTime()).ToList();
        }

        private void GenerateVehicles(int currentTime)
        {         
            for (int index = 0; index < gerenationTimes.Count; index++)
            {
                if (currentTime == gerenationTimes[index])
                {
                    var vehicle = GenerateVehicleWithDriver(index);
                    scene.Tunnel.Roadways[index].Vehicles.Add(vehicle);
                    var nextGenerationTime = currentTime + scene.Tunnel.Roadways[index].RandomValueGenerator.GetNextGenerationTime();
                    gerenationTimes[index] = nextGenerationTime;
                }
            }
        }

        private void ModelingStep(int currentTime)
        {
            var roarways = scene.Tunnel.Roadways;
            for (int roadway = 0; roadway < roarways.Count; roadway++)
            {
                var vehicles = roarways[roadway].Vehicles;
                for (int vehicle = 0; vehicle < vehicles.Count; vehicle++)
                {
                    var currentVehicle = vehicles[vehicle];
                    int distance = Configuration.DetlaModelingTime * currentVehicle.Speed;
                    // the first vehicle need not orient at ahead transports
                    if (vehicle != 0)
                    {
                        int aheadVehicleX = vehicles[vehicle - 1].X;
                        int aheadVehicleDistance = Configuration.DetlaModelingTime * vehicles[vehicle - 1].Speed;
                        int aheadVehicleLength = vehicles[vehicle - 1].Length;
                        int enabledDistance = Math.Abs(aheadVehicleX - currentVehicle.X) - Configuration.DrawingSceneConfiguration.VehicleIconWidth - 10;
                        if (enabledDistance >= distance)                       
                            currentVehicle.X = currentVehicle.Direction == TravelDirection.LEFT_TO_RIGHT ? currentVehicle.X + distance : currentVehicle.X - distance;
                        else
                            currentVehicle.Speed = vehicles[vehicle - 1].Speed;                     
                    }
                    else
                        currentVehicle.X = currentVehicle.Direction == TravelDirection.LEFT_TO_RIGHT ? currentVehicle.X + distance : currentVehicle.X - distance;
                }
            }
        }

        private Vehicle GenerateVehicleWithDriver(int roadwayNumber)
        {
            var direction = roadwayNumber < Configuration.TunnelConfiguration.RoadwaysConfiguration.Count / 2
                ? TravelDirection.LEFT_TO_RIGHT : TravelDirection.RIGHT_TO_LEFT;
  
            var vehicle = VehicleConfigurationToVehicleConverter.Convert(GetVehicleConfiguration(), direction, vehicleIconSize); 
            vehicle.Driver = DriverConfigurationToDriverConverter.Convert(GetDriverConfiguration());
            
            vehicle.X = direction == TravelDirection.LEFT_TO_RIGHT ? -Configuration.DrawingSceneConfiguration.VehicleIconWidth : SceneRenderer.ContextWidth;
            vehicle.Y = roadwayWidth * roadwayNumber + halfRoadwayWidth - Configuration.DrawingSceneConfiguration.VehicleIconHeight / 2;
            return vehicle;
        }

        private DriverConfiguration GetDriverConfiguration()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var index = random.Next(0, Configuration.DriverTypesConfiguration.Count);
            return Configuration.DriverTypesConfiguration[index];
        }

        private VehicleConfiguration GetVehicleConfiguration()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var index = random.Next(0, Configuration.VehicleTypesConfiguration.Count);
            return Configuration.VehicleTypesConfiguration[index];
        }        
    }
}
