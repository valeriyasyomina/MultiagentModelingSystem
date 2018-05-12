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
        public delegate void ModelingResultChange(ModelingResult result);
        public event ModelingResultChange OnModelingResultChange;
        public const int SECONDS_IN_MINUTE = 60;

        public ModelingEngineConfiguration Configuration { get; protected set; }
        public ISceneRenderer SceneRenderer { get; protected set; }

        protected MultiAgentScene scene = null;
        protected ModelingResult ModelingResult = null;
        protected List<int> gerenationTimes = new List<int>();
   
        /// roadway width (helphull variables)
        protected int roadwayWidth = 0;
        protected int halfRoadwayWidth = 0;
        protected int scale = 0;

        protected int accidentTime = 0;

        // запомниаем номер АТС, на котором началось возгорание
        protected int generateFireVehicleIndex = 0;
        protected int generateFireRoadwayIndex = 0;

        public MultiagentModelingEngine(ModelingEngineConfiguration configuration, ISceneRenderer sceneRenderer)
        {
            Configuration = configuration ?? throw new ArgumentNullException($"{configuration}");
            SceneRenderer = sceneRenderer ?? throw new ArgumentNullException($"{sceneRenderer}");
        }

        public void WriteModelingResultToFile(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException();
            ModelingResult.WriteFile(fileName, Configuration);
        }

        public void InitializeMultiAgentScene()
        {
            SceneRenderer.InitializeContext();
            scene = new MultiAgentScene(TunnelConfigurationToTunnelConverter.Convert(Configuration.TunnelConfiguration));

            InitializeRoadMatrix();
            scene.Tunnel.SmokeCovers = SceneRenderer.GetSmokeCovers();
            InitializeGenerationTimesList();

            ModelingResult = new ModelingResult();
            roadwayWidth = SceneRenderer.ContextHeigth / Configuration.TunnelConfiguration.RoadwaysConfiguration.Count;
            halfRoadwayWidth = roadwayWidth / 2;
            scale = Configuration.TunnelConfiguration.Lenght / SceneRenderer.ContextWidth;
        }

        private void InitializeRoadMatrix()
        {
            int columns = SceneRenderer.ContextWidth / RoadwayMatrix.CELL_SIZE;
            int rows = SceneRenderer.ContextHeigth / RoadwayMatrix.CELL_SIZE;

            scene.Tunnel.RoadwayMatrix.Initialize(rows, columns);
        }

        public void StartModeling()
        {
            int startTime = Configuration.StartModelingTime;
            int endTime = Configuration.EndModelingTime;
            accidentTime = Configuration.AccidentTime;

            int time = startTime;
            while (!EvacuationCompleted(time))
            {
                if (!scene.Tunnel.IsFire() && CanGenerateFire(time))
                {
                    GenerateFire(time);
                    GenerateSmoke(time);
                    accidentTime = time;
                    ModelingResult.AccidentTime = time;
                }
                if (!scene.Tunnel.IsFire())
                    GenerateVehicles(time);
                RenderScene();
                ModelingStep(time);
                Thread.Sleep(Configuration.Sleep);
                time += Configuration.DetlaModelingTime;
                // remove vehicles that left tunnel
                RemoveVehicles();
                // remove persons who are in smoke cover or died
                RemovePeople(time);
            }
            ModelingResult.EvacuationTime = time - accidentTime;
        }

        private bool CanGenerateFire(int currentTime)
        {
            return Configuration.MaxPeopleAmount == 0 ? currentTime == accidentTime : ModelingResult.AllPeopleAmount - ModelingResult.SavedInCarsPeopleAmount >= Configuration.MaxPeopleAmount;
        }

        private bool EvacuationCompleted(int currentTime)
        {
            bool result = false;
            if (Configuration.MaxPeopleAmount == 0)
            {
                result = currentTime == Configuration.EndModelingTime;
            }
            else
            {
                result = scene.Tunnel.IsFire() && scene.Tunnel.People.Count == 0 && (scene.Tunnel.GetVehicles().Count == 0 || scene.Tunnel.GetVehicles().All(v => v.Passengers.Count == 0));
            }
            return result;          
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

        private void GenerateSmoke(int currentTime)
        {
            var smoke = IconConvigurationToSmokeConverter.Convert(Configuration.TunnelConfiguration.SmokeConfiguration);

            smoke.Position.X = scene.Tunnel.Fire.Position.X;
            smoke.Position.Y = scene.Tunnel.Fire.Position.Y;
            smoke.LastModelingTime = currentTime;

            scene.Tunnel.Smoke = smoke;
        }

        private void GenerateFire(int currentTime)
        {
            var fire = IconConvigurationToFireConverter.Convert(Configuration.TunnelConfiguration.FireConfiguration);
            var random = new Random(DateTime.Now.Millisecond);
            generateFireRoadwayIndex = random.Next(0, Configuration.TunnelConfiguration.RoadwaysConfiguration.Count);
            var roadway = scene.Tunnel.Roadways[generateFireRoadwayIndex];
            generateFireVehicleIndex = roadway.Vehicles.Count / 2; // random.Next(0, roadway.Vehicles.Count);
            var vehicle = roadway.Vehicles[generateFireVehicleIndex];
            fire.Position.X = vehicle.Position.X; // - vehicle.Icon.Width / 2;
            fire.Position.Y = vehicle.Position.Y; // - Configuration.TunnelConfiguration.FireConfiguration.Height / 2;
            fire.LastModelingTime = currentTime;
            fire.GenerationTime = currentTime;
  
            scene.Tunnel.Fire = fire;
        }

        private void GenerateVehicles(int currentTime)
        {
            for (int index = 0; index < gerenationTimes.Count; index++)
            {
                if (currentTime == gerenationTimes[index])
                {
                    var vehicle = GenerateVehicleWithPassengers(index);
                    // СТАТИСТИКА 
                    ModelingResult.AllVehiclesAmount++;
                    ModelingResult.AllPeopleAmount += vehicle.PassengersNumber;
                    OnModelingResultChange?.Invoke(ModelingResult);
                    //
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

            if (scene.Tunnel.IsFire())
                scene.Tunnel.Fire.Modeling(currentTime);

            if (scene.Tunnel.IsSmoke())
                scene.Tunnel.Smoke.Modeling(currentTime, SceneRenderer.ContextWidth, SceneRenderer.ContextHeigth);
        }

        private void RemovePeople(int currentTime)
        {
            for (int i = 0; i < scene.Tunnel.People.Count; i++)
            {
                var person = scene.Tunnel.People[i];
                if (person.IsInSmokeCover || person.State == PersonState.DIED)
                {
                    if (person.State != PersonState.DIED)
                    {
                        var smokeCoverIndex = scene.Tunnel.FindSmokeCoverIndex(scene.Tunnel.People[i].NearestSmokeCover);
                        // СТАТИСТИКА по спасшимся людям
                        ModelingResult.SavedInSmokeCoverPeopleAmount++;
                        ModelingResult.AddPersonBySmokeCover(smokeCoverIndex);
                        ModelingResult.AddEvacuationTime(scene.Tunnel.People[i].EndEvacuationTime - scene.Tunnel.People[i].StartEvacuationTime);
                    }
                    else
                        ModelingResult.DiedPeople++;
                    OnModelingResultChange?.Invoke(ModelingResult);
                    //
                    scene.Tunnel.People.RemoveAt(i);
                }
            }            
        }

        private void RemoveVehicles()
        {
            var roarways = scene.Tunnel.Roadways;
            for (int roadway = 0; roadway < roarways.Count; roadway++)
            {
                var vehicles = roarways[roadway].Vehicles;
                for (int vehicle = 0; vehicle < vehicles.Count; vehicle++)
                {
                    var currentVehicle = vehicles[vehicle];
                    var removeVehicle = currentVehicle.Direction == TravelDirection.RIGHT_TO_LEFT && currentVehicle.Position.X < 0 ||
                        currentVehicle.Direction == TravelDirection.LEFT_TO_RIGHT && currentVehicle.Position.X > SceneRenderer.ContextWidth;
                    if (removeVehicle)
                    {
                        // СТАТИСТИКА увеличиваем число покинувших тоннель автомобилей
                        ModelingResult.LeftTunnelVehiclesAmount++;
                        ModelingResult.SavedInCarsPeopleAmount += currentVehicle.PassengersNumber;
                        OnModelingResultChange?.Invoke(ModelingResult);
                        //
                        roarways[roadway].Vehicles.RemoveAt(vehicle);
                    }
                }
            }
        }     

        private void ModelingPeople(int currentTime)
        {
            for (int i = 0; i < scene.Tunnel.People.Count; i++)
            {
                var person = scene.Tunnel.People[i];
                if (!person.IsInSmokeCover && person.State != PersonState.DIED)
                    person.MakeDecision(scene.Tunnel, currentTime, Configuration.DetlaModelingTime, Configuration.ConsiderTemperament);
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
                    if (currentVehicle.PassengersNumber != 0 && currentVehicle.State != VehicleState.BURNED)
                    {
                        int distance = Configuration.DetlaModelingTime * currentVehicle.Speed;
                        // the first vehicle need not orient at ahead transports
                        if (vehicle != 0)
                        {
                            int aheadVehicleX = vehicles[vehicle - 1].Position.X;
                            int aheadVehicleDistance = Configuration.DetlaModelingTime * vehicles[vehicle - 1].Speed;
                            int aheadVehicleLength = vehicles[vehicle - 1].Length;
                            int enabledDistance = Math.Abs(aheadVehicleX - currentVehicle.Position.X) - vehicles[vehicle - 1].Icon.Width - 10;
                            var leaveVehicleCondition = Configuration.LeaveVehiclesImmediately ? scene.Tunnel.IsFire() : 
                                IsFireAhead(currentVehicle) || IsSmokeAhead(currentVehicle) || IsPersonAhead(currentVehicle) || vehicles[vehicle - 1].Speed == 0;
                            if (leaveVehicleCondition)
                            {
                                currentVehicle.Speed = 0;
                                currentVehicle.State = VehicleState.STOPPED;
                                PeopeleLeaveTheVehicle(currentVehicle, currentTime, vehicle, roadway);
                            }
                            else
                            {
                                if (enabledDistance >= distance)
                                    currentVehicle.Position.X = currentVehicle.Direction == TravelDirection.LEFT_TO_RIGHT ? currentVehicle.Position.X + distance : currentVehicle.Position.X - distance;
                                else
                                    currentVehicle.Speed = vehicles[vehicle - 1].Speed;
                            }
                        }
                        else
                        {
                            var leaveVehicleCondition = Configuration.LeaveVehiclesImmediately ? scene.Tunnel.IsFire() :
                                IsFireAhead(currentVehicle) || IsSmokeAhead(currentVehicle) || IsPersonAhead(currentVehicle);
                            if (leaveVehicleCondition)
                            {
                                currentVehicle.Speed = 0;
                                currentVehicle.State = VehicleState.STOPPED;
                                PeopeleLeaveTheVehicle(currentVehicle, currentTime, vehicle, roadway);
                            }
                            else
                                currentVehicle.Position.X = currentVehicle.Direction == TravelDirection.LEFT_TO_RIGHT ? currentVehicle.Position.X + distance : currentVehicle.Position.X - distance;
                        }
                    }
                    else if (currentVehicle.State != VehicleState.BURNED)
                    {
                        var isBurned = CheckIfVihicleBurned(currentVehicle);
                        if (isBurned)
                        {
                            ModelingResult.BurnedVehiclesAmount++;
                            currentVehicle.State = VehicleState.BURNED;
                        }
                    }
                }
            }
        }

        private bool CheckIfVihicleBurned(Vehicle vehicle)
        {
            var dist = Vector.Distance(scene.Tunnel.Fire.Position, vehicle.Position);
            return dist < scene.Tunnel.Fire.Icon.Width;
        }

        private bool IsPersonAhead(Vehicle vehicle)
        {
            int i = 0;
            bool personFound = false;
            while (i < scene.Tunnel.People.Count && !personFound)
            {
                var person = scene.Tunnel.People[i];
                var dist = Vector.Distance(person.Position, vehicle.Position);
                if (dist <= vehicle.Icon.Width)
                    personFound = true;
                i++;
            }
            return personFound;
        }

        private bool IsSmokeAhead(Vehicle vehicle)
        {
            var result = false;
            if (scene.Tunnel.IsSmoke())
            {
                var dist = Vector.Distance(vehicle.Position, scene.Tunnel.Smoke.Position);
                result = dist < vehicle.Icon.Width / 2 + scene.Tunnel.Smoke.Icon.Width / 2;
            }
            return result;
        }

        private bool IsFireAhead(Vehicle vehicle)
        {
            var result = false;
            if (scene.Tunnel.IsFire())
            {
                var dist = Vector.Distance(vehicle.Position, scene.Tunnel.Fire.Position);
                result = dist < vehicle.Icon.Width / 2 + scene.Tunnel.Fire.Icon.Width / 2;
            }
            return result;
        }

        private Cell GetNearestCellToInitialPersonPlacement(int x, int y, out int indexI, out int indexJ)
        {
            var matrixData = scene.Tunnel.RoadwayMatrix.Data;
            int rows = scene.Tunnel.RoadwayMatrix.Rows;
            int columns = scene.Tunnel.RoadwayMatrix.Columns;

            indexI = 0;
            indexJ = 0;

            Cell result = null;
            var minDist = Double.MaxValue;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    var cell = matrixData[i, j];
                    if (!cell.BusyWithPerson)
                    {
                        var dist = Math.Sqrt((x - cell.Center.X) * (x - cell.Center.X) + (y - cell.Center.Y) * (y - cell.Center.Y));
                        if (dist < minDist)
                        {
                            minDist = dist;
                            result = cell;
                            indexI = i;
                            indexJ = j;
                        }
                    }
                }
            return result;
        }

        private bool IsVehicleWhereFireStarted(int vehicleIndex, int roadwayIndex)
        {
            return generateFireVehicleIndex == vehicleIndex && generateFireRoadwayIndex == roadwayIndex;
        }

        private void PeopeleLeaveTheVehicle(Vehicle vehicle, int currentTime, int vehicleIndex, int roadwayIndex)
        {
            if (vehicle.PassengersNumber != 0)
            {
                var isFireStartedThisVehicle = IsVehicleWhereFireStarted(vehicleIndex, roadwayIndex);
                // СТАТИСТИКА
                if (isFireStartedThisVehicle)
                {
                    ModelingResult.BurnedVehiclesAmount += 1;
                    ModelingResult.DamagedPeople += vehicle.PassengersNumber - 1;
                    ModelingResult.DiedPeople += 1;
                   
                }
                var passengersNumber = isFireStartedThisVehicle ? vehicle.PassengersNumber - 1 : vehicle.PassengersNumber;
                var halfPeopleNumber = passengersNumber / 2;
                for(int i = 0, delta = 10; i < passengersNumber; i++, delta += 10)
                {
                    var person = vehicle.Passengers[i];
                    
                    PersonState state = isFireStartedThisVehicle ? PersonState.DAMAGED : PersonState.ALIVE;
                    person.State = state;
                    var y = i < halfPeopleNumber ? vehicle.Position.Y - vehicle.Icon.Height / 2 - 5 : vehicle.Position.Y + vehicle.Icon.Height / 2 + 5;
                    var indexI = 0;
                    var indexJ = 0;
                    var nearestCell = GetNearestCellToInitialPersonPlacement(vehicle.Position.X - vehicle.Icon.Width / 2 + delta, y, out indexI, out indexJ);

                    if (nearestCell != null)
                    {
                        person.Position.X = nearestCell.Center.X;
                        person.Position.Y = nearestCell.Center.Y;

                        person.CellIndexI = indexI;
                        person.CellIndexJ = indexJ;

                        person.LastDecisionTime = currentTime;
                        person.LastDisorientationTime = currentTime;
                        scene.Tunnel.People.Add(person);
                    }
                }               
                vehicle.PassengersNumber = 0;
                vehicle.Passengers.Clear();
            }
        }

        private Vehicle GenerateVehicleWithPassengers(int roadwayNumber)
        {
            var direction = roadwayNumber < Configuration.TunnelConfiguration.RoadwaysConfiguration.Count / 2
                ? TravelDirection.RIGHT_TO_LEFT : TravelDirection.LEFT_TO_RIGHT;

            var vehicleConfiguration = GetVehicleConfiguration();
            var vehicle = VehicleConfigurationToVehicleConverter.Convert(vehicleConfiguration, direction);
            GeneratePeopleForVehicle(vehicle, Configuration.ConsiderTemperament);
       //     vehicle.Driver = PersonConfigurationToPersonConverter.Convert(GetPersonConfiguration(), Configuration.ConsiderPsychotype);

            vehicle.Position.X = direction == TravelDirection.LEFT_TO_RIGHT ? -vehicle.Icon.Width : SceneRenderer.ContextWidth;
            vehicle.Position.Y = roadwayWidth * roadwayNumber + halfRoadwayWidth;// - vehicle.Icon.Height / 2;
            return vehicle;
        }

        private void GeneratePeopleForVehicle(Vehicle vehicle, bool considerPsychotype = false)
        {
            for (int i = 0; i < vehicle.PassengersNumber; i++)
            {
                var person = PersonConfigurationToPersonConverter.Convert(GetPersonConfiguration(), considerPsychotype);
                // СТАТИСТИКА
                ModelingResult.AddPersonByPersonalityPsychotype(person.PersonalityTemperament);
                ModelingResult.AddPersonBySex(person.Sex);
                ModelingResult.AddPersonAgeAmount(person.Age);
                //
                vehicle.Passengers.Add(person);
            }
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
