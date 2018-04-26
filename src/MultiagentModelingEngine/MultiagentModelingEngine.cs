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
        public Cell[,] RoadMatrix { get; set; }
        public int RoadMatrixRows { get; set; }
        public int RoadMatrixColumns { get; set; }

        public const int CELL_SIZE = 10;
        public const int SECONDS_IN_MINUTE = 60;

        public ModelingEngineConfiguration Configuration { get; protected set; }
        public ISceneRenderer SceneRenderer { get; protected set; }

        protected MultiAgentScene scene = null;
        protected List<int> gerenationTimes = new List<int>();
        protected List<Vector> SmokeCoversKoordinates = null;
        
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
            InitializeRoadMatrix();
            SceneRenderer.InitializeContext();
            SmokeCoversKoordinates = SceneRenderer.GetKoordinates();
            scene = new MultiAgentScene(TunnelConfigurationToTunnelConverter.Convert(Configuration.TunnelConfiguration));         
            InitializeGenerationTimesList();

            roadwayWidth = SceneRenderer.ContextHeigth / Configuration.TunnelConfiguration.RoadwaysConfiguration.Count;
            halfRoadwayWidth = roadwayWidth / 2;
            scale = Configuration.TunnelConfiguration.Lenght / SceneRenderer.ContextWidth;
        }

        private void InitializeRoadMatrix()
        {
            RoadMatrixColumns = SceneRenderer.ContextWidth / CELL_SIZE;
            RoadMatrixRows = SceneRenderer.ContextHeigth / CELL_SIZE;

            RoadMatrix = new Cell[RoadMatrixRows, RoadMatrixColumns];

            for (int i = 0; i < RoadMatrixRows; i++)            
                for (int j = 0; j < RoadMatrixColumns; j++)
                    RoadMatrix[i, j] = new Cell(j * CELL_SIZE + CELL_SIZE / 2, i * CELL_SIZE + CELL_SIZE / 2);
            
        }

        public void StartModeling()
        {
            int startTime = Configuration.StartGenerationTime;
            int endTime = Configuration.EndGenerationTime;
            int accidentTime = Configuration.AccidentTime;

            int time = startTime;
            while (time <= endTime)
            {
                if (time == accidentTime)
                {
                    GenerateFire();
                    GenerateSmoke();
                }
                if (time < accidentTime)
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

        private void ReleaseVehicles()
        {
          //  scene.Tunnel.Roadways
        }

        private void InitializeGenerationTimesList()
        {
            gerenationTimes = scene.Tunnel.Roadways.Select(r => r.RandomValueGenerator.GetNextGenerationTime()).ToList();
        }

        private void GenerateSmoke()
        {
            var smoke = IconConvigurationToFireConverter.Convert(Configuration.TunnelConfiguration.SmokeConfiguration);

            smoke.X = scene.Tunnel.Fire.X;
            smoke.Y = scene.Tunnel.Fire.Y;
            smoke.Radius = 1;

            scene.Tunnel.Smoke = smoke;
        }

        private void GenerateFire()
        {
            var fire = IconConvigurationToFireConverter.Convert(Configuration.TunnelConfiguration.FireConfiguration);
            var random = new Random(DateTime.Now.Millisecond);
            var roadwayNumber = random.Next(0, Configuration.TunnelConfiguration.RoadwaysConfiguration.Count);
            var roadway = scene.Tunnel.Roadways[roadwayNumber];
            var vehicleIndex = random.Next(0, roadway.Vehicles.Count);
            var vehicle = roadway.Vehicles[vehicleIndex];
            fire.X = vehicle.X; // - vehicle.Icon.Width / 2;
            fire.Y = vehicle.Y - Configuration.TunnelConfiguration.FireConfiguration.Height / 2;
            fire.Radius = 1;

            scene.Tunnel.Fire = fire;
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
            ModelingVehicles(currentTime);
            ModelingPeople(currentTime);
            ModelingFire(currentTime);
        }

        private void ModelingFire(int currentTime)
        {
            if (scene.Tunnel.IsFire())
            {
                var accidentTime = Configuration.AccidentTime;
                var minutes = Math.Abs(currentTime - accidentTime) / SECONDS_IN_MINUTE;
                if (minutes != 0 && Math.Abs(currentTime - accidentTime) % SECONDS_IN_MINUTE == 0)
                {
                    scene.Tunnel.Fire.Radius = minutes;
                }
            }
        }

        private void ModelingSmoke(int currentTime)
        {

        }

        private void ModelingPeople(int currentTime)
        {
            for (int i = 0; i < scene.Tunnel.People.Count; i++)
            {
                var person = scene.Tunnel.People[i];
                if (!person.NearestSmokeCoverDetected())
                {
                    person.DetectNearestSmokeCover(SmokeCoversKoordinates);
                }
                person.MakeDecision(scene.Tunnel, Configuration.DetlaModelingTime);
            }
        }

        private void ModelingVehicles(int currentTime)
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
                        int enabledDistance = Math.Abs(aheadVehicleX - currentVehicle.X) - vehicles[vehicle - 1].Icon.Width - 10;
                        if (IsFireAhead(currentVehicle) || vehicles[vehicle - 1].Speed == 0)
                        {
                            currentVehicle.Speed = 0;
                            PeopeleLeaveTheVehicle(currentVehicle);
                        }
                        else
                        {
                            if (enabledDistance >= distance)
                                currentVehicle.X = currentVehicle.Direction == TravelDirection.LEFT_TO_RIGHT ? currentVehicle.X + distance : currentVehicle.X - distance;
                            else
                                currentVehicle.Speed = vehicles[vehicle - 1].Speed;
                        }
                    }
                    else
                    {
                        if (IsFireAhead(currentVehicle))
                        {
                            currentVehicle.Speed = 0;
                            PeopeleLeaveTheVehicle(currentVehicle);
                        }
                        else
                            currentVehicle.X = currentVehicle.Direction == TravelDirection.LEFT_TO_RIGHT ? currentVehicle.X + distance : currentVehicle.X - distance;
                    }
                }
            }
        }

        private bool IsFireAhead(Vehicle vehicle)
        {
            var result = false;
            if (scene.Tunnel.IsFire())
            {
                /* var canMove = vehicle.Direction == TravelDirection.LEFT_TO_RIGHT ?
                     scene.Tunnel.Fire.X < vehicle.X :
                     scene.Tunnel.Fire.X > vehicle.X; */
                var canMove = Math.Abs(scene.Tunnel.Fire.X - vehicle.X) > vehicle.Icon.Width;
                result = !canMove && Math.Abs(Math.Abs(scene.Tunnel.Fire.Y) - vehicle.Y) <= 25;
            }    
            return result;
        }

        private void PeopeleLeaveTheVehicle(Vehicle vehicle)
        {
            if (vehicle.PassengersNumber != 0)
            {
                var driver = vehicle.Driver;
                driver.IsInVehicle = false;
                driver.X = vehicle.X;
                driver.Y = vehicle.Y - vehicle.Icon.Height / 2;
                // driver will be dead by default
            //    scene.Tunnel.People.Add(driver);
               
                for (int i = 0, delta = 10; i < vehicle.PassengersNumber - 1; i++, delta += 10)
                {
                    var person = PersonConfigurationToPersonConverter.Convert(GetPersonConfiguration());
                    person.X = vehicle.X + delta;
                    person.Y = vehicle.Y - vehicle.Icon.Height / 2;
                    scene.Tunnel.People.Add(person);
                }
                vehicle.PassengersNumber = 0;
            }
        }

        private Vehicle GenerateVehicleWithDriver(int roadwayNumber)
        {
            var direction = roadwayNumber < Configuration.TunnelConfiguration.RoadwaysConfiguration.Count / 2
                ? TravelDirection.RIGHT_TO_LEFT : TravelDirection.LEFT_TO_RIGHT;
  
            var vehicle = VehicleConfigurationToVehicleConverter.Convert(GetVehicleConfiguration(), direction); 
            vehicle.Driver = PersonConfigurationToPersonConverter.Convert(GetPersonConfiguration());
            
            vehicle.X = direction == TravelDirection.LEFT_TO_RIGHT ? -vehicle.Icon.Width : SceneRenderer.ContextWidth;
            vehicle.Y = roadwayWidth * roadwayNumber + halfRoadwayWidth;// - vehicle.Icon.Height / 2;
            return vehicle;
        }

        private PersonConfiguration GetPersonConfiguration()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var index = random.Next(0, Configuration.PersonTypesConfiguration.Count);
            return Configuration.PersonTypesConfiguration[index];
        }

        private VehicleConfiguration GetVehicleConfiguration()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var index = random.Next(0, Configuration.VehicleTypesConfiguration.Count);
            return Configuration.VehicleTypesConfiguration[index];
        }        
    }
}
