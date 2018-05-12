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
        public Person()
        {
            CellIndexI = UNDEFINED_INDEX;
            CellIndexJ = UNDEFINED_INDEX;
            NearestSmokeCover = new Vector(UNDEFINED_INDEX, UNDEFINED_INDEX);
            Position = new Vector(0, 0);
            PreviousPosition = new Vector(UNDEFINED_INDEX, UNDEFINED_INDEX);
            IsInSmokeCover = false;
            EnterSmokeTime = UNDEFINED_TIME;
            VisitedSmokeCoversIndexes = new List<int>();
        }
        public const int DEFAULT_DELTA_MOVE_TIME = 3;
        public const int MIN_COVER_DISTANCE = 10;
        public const int UNDEFINED_INDEX = -1;
        public const int UNDEFINED_TIME = -1;
        public const int ELDERLY_PERSON_AGE = 50;

        public bool IsInVehicle { get; set; }
        public bool IsInSmokeCover { get; set; }

        public override Vector Position { get; set; }
        private Vector PreviousPosition { get; set; }

        public Sex Sex { get; set; }
        public PersonalityTemperament PersonalityTemperament { get; set; }
        public double MaxSpeed { get; set; }
        public double ComfortableSpeed { get; set; }
        public int Age { get; set; }

        public int MakeDecisionDeltaTime { get; set; }
        public int DisorientationDeltaTime { get; set; }

        public int LastDisorientationTime { get; set; }
        public int LastDecisionTime { get; set; }
        public int LastMoveTime { get; set; }
        public int StartEvacuationTime { get; set; }
        public int EndEvacuationTime { get; set; }

        /// <summary>
        ///  время пребывания в дыму
        /// </summary>
        public int EnterSmokeTime { get; set; }
        public int InSmokeDeltaTime { get; set; }

        public PersonState State { get; set; }

        public Vector NearestSmokeCover { get; set; }
        public double DistanceToNearestSmokeCover { get; set; }
        public List<int> VisitedSmokeCoversIndexes { get; set; }

        public int CellIndexI { get; set; }
        public int CellIndexJ { get; set; }

        public bool NearestSmokeCoverDetected()
        {
            return NearestSmokeCover.X != UNDEFINED_INDEX && NearestSmokeCover.Y != UNDEFINED_INDEX;
        }

        public bool IsInTheCell()
        {
            return CellIndexI != UNDEFINED_INDEX && CellIndexJ != UNDEFINED_INDEX;
        }

        public void DetectNearestSmokeCover(Tunnel tunnel)
        {
            double distance = Double.MaxValue;
            var safeSmokeCovers = tunnel.SmokeCovers.Where(s => 
                !IsSmokeCoverInFire(tunnel.Fire, s) && !IsSmokeCoverInSmoke(tunnel.Smoke, s)).ToList();

            var nearestSmokeCover = GetNearestSmokeCover(safeSmokeCovers, out distance);
            if (nearestSmokeCover == null)
                nearestSmokeCover = GetNearestSmokeCover(tunnel.SmokeCovers, out distance);

            NearestSmokeCover.X = nearestSmokeCover.Position.X;
            NearestSmokeCover.Y = nearestSmokeCover.Position.Y;
            DistanceToNearestSmokeCover = distance;
            /* var minDist = Double.MaxValue;
             for (int i = 0; i < smokeCovers.Count; i++)
             {
                 var cover = smokeCovers[i];
                 var dist = Vector.Distance(Position, cover.Position);         
                 if (!VisitedSmokeCoversIndexes.Contains(i) && dist < minDist)
                 {
                     minDist = dist;
                     NearestSmokeCover.X = cover.Position.X;
                     NearestSmokeCover.Y = cover.Position.Y;
                     DistanceToNearestSmokeCover = minDist;
                 }
             }      */
        }

        private bool IsPreviousPosition(Vector position)
        {
            return PreviousPosition.X == position.X && PreviousPosition.Y == position.Y;
        }

        private SmokeCover GetNearestSmokeCover(List<SmokeCover> smokeCovers, out double distance)
        {           
            SmokeCover result = null;
            distance = Double.MaxValue;
            for (int i = 0; i < smokeCovers.Count; i++)
            {
                var cover = smokeCovers[i];
                var dist = Vector.Distance(Position, cover.Position);
                if (!VisitedSmokeCoversIndexes.Contains(i) && dist < distance)
                {
                    result = cover;
                    distance = dist;
                  //  NearestSmokeCover.X = cover.Position.X;
                   // NearestSmokeCover.Y = cover.Position.Y;
                   // DistanceToNearestSmokeCover = minDist;
                }
            }
            return result;
        }
        

      //  private Cell Get
        /// <summary>
        /// Поиск ближайшей свободной клетки, которая ближе всего расположена к противодымному укрытию
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>

        private Cell GetNeigbourFreeCellNearestToCover(Tunnel tunnel, out double distance, out int indexI, out int indexJ)
        {
            var cells = tunnel.RoadwayMatrix.Data;
            int rows = tunnel.RoadwayMatrix.Rows;
            int columns = tunnel.RoadwayMatrix.Columns;

            var vehicles = tunnel.GetVehicles();

            //   var minDist = Double.MaxValue;
            distance = Double.MaxValue;
            indexI = UNDEFINED_INDEX;
            indexJ = UNDEFINED_INDEX;

            Cell result = null;

            Cell currentResult = null;
            var currentDist = Double.MaxValue;

            // вверху
            if (CellIndexI - 1 >= 0)
            {
                var cell = cells[CellIndexI - 1, CellIndexJ];
                ProcessCellNearestToSmokeCover(cell, vehicles, tunnel.Fire, ref currentDist, ref currentResult);               
            } 
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI - 1;
                indexJ = CellIndexJ;
            }
            // вверху слева
            if (CellIndexI - 1 >= 0 && CellIndexJ - 1 >= 0)
            {
                var cell = cells[CellIndexI - 1, CellIndexJ - 1];
                ProcessCellNearestToSmokeCover(cell, vehicles, tunnel.Fire, ref currentDist, ref currentResult);
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI - 1;
                indexJ = CellIndexJ - 1;
            }
            // ввеху справа
            if (CellIndexI - 1 >= 0 && CellIndexJ + 1 < columns)
            {
                var cell = cells[CellIndexI - 1, CellIndexJ + 1];
                ProcessCellNearestToSmokeCover(cell, vehicles, tunnel.Fire, ref currentDist, ref currentResult);
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI - 1;
                indexJ = CellIndexJ + 1;
            }
            // слева
            if (CellIndexJ - 1 >= 0)
            {
                var cell = cells[CellIndexI, CellIndexJ - 1];
                ProcessCellNearestToSmokeCover(cell, vehicles, tunnel.Fire, ref currentDist, ref currentResult);
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI;
                indexJ = CellIndexJ - 1;
            }
            // справа
            if (CellIndexJ + 1 < columns)
            {
                var cell = cells[CellIndexI, CellIndexJ + 1];
                ProcessCellNearestToSmokeCover(cell, vehicles, tunnel.Fire, ref currentDist, ref currentResult);
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI;
                indexJ = CellIndexJ + 1;
            }
            // снизу
            if (CellIndexI + 1 < rows)
            {
                var cell = cells[CellIndexI + 1, CellIndexJ];
                ProcessCellNearestToSmokeCover(cell, vehicles, tunnel.Fire, ref currentDist, ref currentResult);
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI + 1;
                indexJ = CellIndexJ;
            }
            // внизу слева
            if (CellIndexI + 1 < rows && CellIndexJ - 1 >= 0)
            {
                var cell = cells[CellIndexI + 1, CellIndexJ - 1];
                ProcessCellNearestToSmokeCover(cell, vehicles, tunnel.Fire, ref currentDist, ref currentResult);
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI + 1;
                indexJ = CellIndexJ - 1;
            }
            // внизу справа
            if (CellIndexI + 1 < rows && CellIndexJ + 1 < columns)
            {
                var cell = cells[CellIndexI + 1, CellIndexJ + 1];
                ProcessCellNearestToSmokeCover(cell, vehicles, tunnel.Fire, ref currentDist, ref currentResult);
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI + 1;
                indexJ = CellIndexJ + 1;
            }           
            return result;
        }

        private Cell GetNeigbourFreeCellNearestToTargetCell(Tunnel tunnel, Cell targetCell, out double distance, out int indexI, out int indexJ)
        {
            var cells = tunnel.RoadwayMatrix.Data;
            int rows = tunnel.RoadwayMatrix.Rows;
            int columns = tunnel.RoadwayMatrix.Columns;

            var vehicles = tunnel.GetVehicles();

            //   var minDist = Double.MaxValue;
            distance = Double.MaxValue;
            indexI = UNDEFINED_INDEX;
            indexJ = UNDEFINED_INDEX;

            Cell result = null;

            Cell currentResult = null;
            var currentDist = Double.MaxValue;

            // вверху
            if (CellIndexI - 1 >= 0)
            {
                var cell = cells[CellIndexI - 1, CellIndexJ];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    currentResult = cell;
                    currentDist = Vector.Distance(cell.Center, targetCell.Center);
                }
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI - 1;
                indexJ = CellIndexJ;
            }
            // вверху слева
            if (CellIndexI - 1 >= 0 && CellIndexJ - 1 >= 0)
            {
                var cell = cells[CellIndexI - 1, CellIndexJ - 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    currentResult = cell;
                    currentDist = Vector.Distance(cell.Center, targetCell.Center);
                }
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI - 1;
                indexJ = CellIndexJ - 1;
            }
            // ввеху справа
            if (CellIndexI - 1 >= 0 && CellIndexJ + 1 < columns)
            {
                var cell = cells[CellIndexI - 1, CellIndexJ + 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    currentResult = cell;
                    currentDist = Vector.Distance(cell.Center, targetCell.Center);
                }
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI - 1;
                indexJ = CellIndexJ + 1;
            }
            // слева
            if (CellIndexJ - 1 >= 0)
            {
                var cell = cells[CellIndexI, CellIndexJ - 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    currentResult = cell;
                    currentDist = Vector.Distance(cell.Center, targetCell.Center);
                }
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI;
                indexJ = CellIndexJ - 1;
            }
            // справа
            if (CellIndexJ + 1 < columns)
            {
                var cell = cells[CellIndexI, CellIndexJ + 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    currentResult = cell;
                    currentDist = Vector.Distance(cell.Center, targetCell.Center);
                }
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI;
                indexJ = CellIndexJ + 1;
            }
            // снизу
            if (CellIndexI + 1 < rows)
            {
                var cell = cells[CellIndexI + 1, CellIndexJ];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    currentResult = cell;
                    currentDist = Vector.Distance(cell.Center, targetCell.Center);
                }
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI + 1;
                indexJ = CellIndexJ;
            }
            // внизу слева
            if (CellIndexI + 1 < rows && CellIndexJ - 1 >= 0)
            {
                var cell = cells[CellIndexI + 1, CellIndexJ - 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    currentResult = cell;
                    currentDist = Vector.Distance(cell.Center, targetCell.Center);
                }
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI + 1;
                indexJ = CellIndexJ - 1;
            }
            // внизу справа
            if (CellIndexI + 1 < rows && CellIndexJ + 1 < columns)
            {
                var cell = cells[CellIndexI + 1, CellIndexJ + 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    currentResult = cell;
                    currentDist = Vector.Distance(cell.Center, targetCell.Center);
                }
            }
            if (currentResult != null && currentDist < distance)
            {
                result = currentResult;
                distance = currentDist;
                indexI = CellIndexI + 1;
                indexJ = CellIndexJ + 1;
            }
            return result;
        }


        private void ProcessCellNearestToSmokeCover(Cell cell, List<Vehicle> vehicles, Fire fire, ref double minDist, ref Cell resultCell)
        {
            if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
            {
                var dist = Vector.Distance(cell.Center, NearestSmokeCover); // GetDistanceBetweenNearestSmokeCoverAndCell(cell);
                if (dist < DistanceToNearestSmokeCover)
                {
                    minDist = dist;
                    resultCell = cell;
                }
            }
        }

        private Cell GetNeigbourFreeCell(Tunnel tunnel,  out int indexI, out int indexJ)
        {
            var cells = tunnel.RoadwayMatrix.Data;
            int rows = tunnel.RoadwayMatrix.Rows;
            int columns = tunnel.RoadwayMatrix.Columns;
            
            indexI = UNDEFINED_INDEX;
            indexJ = UNDEFINED_INDEX;

            var vehicles = tunnel.GetVehicles();
           
            Cell result = null;
            double dist = 0;

            // вверху
            if (CellIndexI - 1 >= 0)
            {
                var cell = cells[CellIndexI - 1, CellIndexJ];
                if (/*!cell.BusyWithPerson &&*/ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    // dist = Vector.Distance(cell.Center, NearestSmokeCover);
                    indexI = CellIndexI - 1;
                    indexJ = CellIndexJ;
                    result = cell;
                }
            }
            // вверху слева
            if (CellIndexI - 1 >= 0 && CellIndexJ - 1 >= 0 && result == null)
            {
                var cell = cells[CellIndexI - 1, CellIndexJ - 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    //  dist = Vector.Distance(cell.Center, NearestSmokeCover);
                    indexI = CellIndexI - 1;
                    indexJ = CellIndexJ - 1;
                    result = cell;
                }
            }
            // ввеху справа
            if (CellIndexI - 1 >= 0 && CellIndexJ + 1 < columns && result == null)
            {
                var cell = cells[CellIndexI - 1, CellIndexJ + 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    //  dist = Vector.Distance(cell.Center, NearestSmokeCover);
                    indexI = CellIndexI - 1;
                    indexJ = CellIndexJ + 1;
                    result = cell;
                }
            }
            // слева
            if (CellIndexJ - 1 >= 0 && result == null)
            {
                var cell = cells[CellIndexI, CellIndexJ - 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    //  dist = Vector.Distance(cell.Center, NearestSmokeCover);
                    indexI = CellIndexI;
                    indexJ = CellIndexJ - 1;
                    result = cell;
                }
            }
            // справа
            if (CellIndexJ + 1 < columns && result == null)
            {
                var cell = cells[CellIndexI, CellIndexJ + 1];
                if (/*!cell.BusyWithPerson &&*/ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    //  dist = Vector.Distance(cell.Center, NearestSmokeCover);
                    indexI = CellIndexI;
                    indexJ = CellIndexJ + 1;
                    result = cell;
                }
            }
            // снизу
            if (CellIndexI + 1 < rows && result == null)
            {
                var cell = cells[CellIndexI + 1, CellIndexJ];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    //  dist = Vector.Distance(cell.Center, NearestSmokeCover);
                    indexI = CellIndexI + 1;
                    indexJ = CellIndexJ;
                    result = cell;
                }
            }
            // внизу слева
            if (CellIndexI + 1 < rows && CellIndexJ - 1 >= 0 && result == null)
            {
                var cell = cells[CellIndexI + 1, CellIndexJ - 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    //  dist = Vector.Distance(cell.Center, NearestSmokeCover);
                    indexI = CellIndexI + 1;
                    indexJ = CellIndexJ - 1;
                    result = cell;
                }
            }
            // внизу справа
            if (CellIndexI + 1 < rows && CellIndexJ + 1 < columns && result == null)
            {
                var cell = cells[CellIndexI + 1, CellIndexJ + 1];
                if (/*!cell.BusyWithPerson && */ !IsBusyWithFire(cell, tunnel.Fire) && !IsBusyWithVechicle(cell, vehicles) && !IsPreviousPosition(cell.Center))
                {
                    //  dist = Vector.Distance(cell.Center, NearestSmokeCover);
                    indexI = CellIndexI + 1;
                    indexJ = CellIndexJ + 1;
                    result = cell;
                }
            } 
            return result;
        }

        private bool IsBusyWithFire(Cell cell, Fire fire)
        {
            var dist = Vector.Distance(cell.Center, fire.Position);
            return dist < cell.Radius + fire.Icon.Width / 2;
        }

        private bool IsBusyWithVechicle(Cell cell, List<Vehicle> vehicles)
        {
            int i = 0;
            bool vehicleFound = false;
            while (i < vehicles.Count && !vehicleFound)
            {
                var vehicle = vehicles[i];
                var dist = Vector.Distance(vehicle.Position, cell.Center); 
                if (dist < vehicle.Icon.Width / 2 + cell.Radius) // both icon width and height are equal, so we can write this way                
                    vehicleFound = true;                
                i++;
            }
            return vehicleFound;
        }

        // Если человек попал в область тумана
        private bool IsInSmoke(Smoke smoke)
        {
            var dist = Vector.Distance(Position, smoke.Position);
            return dist <= smoke.Icon.Width + Icon.Width;
        }

        private bool IsSmokeCoverInSmoke(Smoke smoke, SmokeCover smokeCover)
        {
            var dist = Vector.Distance(smokeCover.Position, smoke.Position);
            return dist <= smoke.Icon.Width;
        }

        private bool IsSmokeCoverInFire(Fire fire, SmokeCover smokeCover)
        {
            var dist = Vector.Distance(smokeCover.Position, fire.Position);
            return dist <= fire.Icon.Width;
        }

        private bool IsInFire(Fire fire)
        {
            var dist = Vector.Distance(Position, fire.Position);
            return dist <= fire.Icon.Width + Icon.Width;
        }

        private SmokeCover FindVisibleSmokeCover(Tunnel tunnel)
        {
            var visibleDist = IsInSmoke(tunnel.Smoke) ? 100 : 200;
            var found = false;
            SmokeCover result = null;
            int i = 0;
            while (i < tunnel.SmokeCovers.Count && !found)
            {
                double dist = Vector.Distance(tunnel.SmokeCovers[i].Position, Position);
                if (dist <= visibleDist)
                {
                    found = true;
                    result = tunnel.SmokeCovers[i];
                }
                i++;
            }
            return result;
        }

        private int GetDeltaMoveTime(Tunnel tunnel)
        {
            var count = 0;
            if (IsInSmoke(tunnel.Smoke))
                count++;
            if (State == PersonState.DAMAGED)
                count++;
            if (Age >= ELDERLY_PERSON_AGE)
                count++;
            return DEFAULT_DELTA_MOVE_TIME + count;
        }

   /*     public void MakeDecision(Tunnel tunnel, int currentTime, int deltaTime, bool considerPsychotype = false)
        {
            if (!considerPsychotype)
            {
                while (!NearestSmokeCoverDetected())
                {
                    var smokeCover = FindVisibleSmokeCover(tunnel);
                    if (smokeCover != null)
                    {
                        NearestSmokeCover.X = smokeCover.Position.X;
                        NearestSmokeCover.Y = smokeCover.Position.Y;
                        DistanceToNearestSmokeCover = Vector.Distance(Position, smokeCover.Position);
                    }
                    else
                    {
                        double dist = Double.MaxValue;
                        var freeCell = GetNeigbourFreeCell(tunnel, out dist, out int cellI, out int cellJ);
                        if (freeCell != null)
                        {
                            Position.X = freeCell.Center.X;
                            Position.Y = freeCell.Center.Y;

                            CellIndexI = cellI;
                            CellIndexJ = cellJ;
                        }
                    }                    
                }
                double distanse = Double.MaxValue;
                var cell = GetNeigbourFreeCellNearestToCover(tunnel, out distanse, out int indexI, out int indexJ);
                if (cell != null)
                {
                    Position.X = cell.Center.X;
                    Position.Y = cell.Center.Y;

                    CellIndexI = indexI;
                    CellIndexJ = indexJ;
                    DistanceToNearestSmokeCover = distanse;
                }

            }
        } */

        private void AssignCell(Cell cell, int indexI, int indexJ)
        {
            PreviousPosition.X = Position.X;
            PreviousPosition.Y = Position.Y;

            Position.X = cell.Center.X;
            Position.Y = cell.Center.Y;

            CellIndexI = indexI;
            CellIndexJ = indexJ;
        }

        private Person FindNearestPersonInViewPoint(Tunnel tunnel)
        {
            var dist = IsInSmoke(tunnel.Smoke) ? 100 : 50;
            int i = 0;
            bool personFound = false;
            Person result = null;

            while (i < tunnel.People.Count && !personFound)
            {
                if (Vector.Distance(Position, tunnel.People[i].Position) <= dist)
                {
                    personFound = true;
                    result = tunnel.People[i];
                }
                i++;
            }

            return result;
        }

        private void Move(Tunnel tunnel)
        {
            double distanse = Double.MaxValue;
            var cell = GetNeigbourFreeCellNearestToCover(tunnel, out distanse, out int indexI, out int indexJ);
            if (cell != null)
            {
                AssignCell(cell, indexI, indexJ);
                DistanceToNearestSmokeCover = distanse;
            }
            else
            {
                var freeCell = GetNeigbourFreeCell(tunnel, out int cellI, out int cellJ);
                if (freeCell != null)
                {
                    AssignCell(freeCell, cellI, cellJ);
                    DetectNearestSmokeCover(tunnel);
                }
            }
        }

        private void ResetNearestSmokeCover()
        {
            NearestSmokeCover.X = UNDEFINED_INDEX;
            NearestSmokeCover.Y = UNDEFINED_INDEX;
        }

        private void MoveWithPsyhotype(Tunnel tunnel)
        {
            if (PersonalityTemperament == PersonalityTemperament.CHOLERIC)
            {
                var person = FindNearestPersonInViewPoint(tunnel);
                if (person != null)
                {
                    NearestSmokeCover.X = person.NearestSmokeCover.X;
                    NearestSmokeCover.Y = person.NearestSmokeCover.Y;
                    DistanceToNearestSmokeCover = Vector.Distance(Position, NearestSmokeCover);
                }
                Move(tunnel);
            }
            else if (PersonalityTemperament == PersonalityTemperament.SANGUINE || PersonalityTemperament == PersonalityTemperament.PHLEGMATIC)
            {
                Move(tunnel);
            }
            else if (PersonalityTemperament == PersonalityTemperament.MELANCHOLIC)
            {
                var person = FindNearestPersonInViewPoint(tunnel);
                if (person != null)
                {
                    double distToPerson = 0;
                    var nearestToPersonCell = GetNeigbourFreeCellNearestToTargetCell(tunnel, new Cell(person.Position.X, person.Position.Y, 10), out distToPerson, out int cellI, out int cellJ);
                    if (nearestToPersonCell != null)
                    {
                        AssignCell(nearestToPersonCell, cellI, cellJ);
                    }
                }
                else
                {
                    var freeCell = GetNeigbourFreeCell(tunnel, out int cellI, out int cellJ);
                    if (freeCell != null)
                    {
                        AssignCell(freeCell, cellI, cellJ);
                    }
                }
            }
        }

        private bool IsCloseToNearestSmokeCover()
        {
            return Vector.Distance(Position, NearestSmokeCover) <= MIN_COVER_DISTANCE;
        }

        private SmokeCover FindCloseSmokeCover(List<SmokeCover> smokeCovers)
        {
            bool found = false;
            SmokeCover result = null;
            int i = 0;
            while (!found && i < smokeCovers.Count)
            {
                if (Vector.Distance(Position, smokeCovers[i].Position) <= MIN_COVER_DISTANCE)
                {
                    result = smokeCovers[i];
                    found = true;
                }
                i++;
            }
            return result;
        }

        private void EnterSmokeCover(SmokeCover smokeCover, int currentTime)
        {
            IsInSmokeCover = true;
            EndEvacuationTime = currentTime;
            smokeCover.PeopleAmount += 1;
        }

        // основной алгоритм моделирования поведения человека
        public void MakeDecision(Tunnel tunnel, int currentTime, int deltaTime, bool considerTemperament = false)
        {
            if (StartEvacuationTime == 0)
                StartEvacuationTime = currentTime;

            if (VisitedSmokeCoversIndexes.Count == tunnel.SmokeCovers.Count)
                State = PersonState.DIED;

            if (State != PersonState.DIED && IsInSmoke(tunnel.Smoke))
            {
                if (EnterSmokeTime == UNDEFINED_TIME)
                    EnterSmokeTime = currentTime;
                if (currentTime - EnterSmokeTime >= InSmokeDeltaTime)
                    State = PersonState.DIED;
            }
            else
                EnterSmokeTime = UNDEFINED_TIME;

            if (!considerTemperament && State != PersonState.DIED)
            {
                if (!NearestSmokeCoverDetected())
                    DetectNearestSmokeCover(tunnel);
                if (IsCloseToNearestSmokeCover())
                {
                    var cover = tunnel.FindSmokeCover(NearestSmokeCover);
                    // добавляем убежище в число посещенных и находим новое
                    if (cover.PeopleAmount + 1 >= cover.MaxPeopleAmount)
                    {
                        var currentSmokeCoverIndex = tunnel.FindSmokeCoverIndex(NearestSmokeCover);
                        if (currentSmokeCoverIndex != UNDEFINED_INDEX)
                            VisitedSmokeCoversIndexes.Add(currentSmokeCoverIndex);
                        DetectNearestSmokeCover(tunnel);
                    }
                    else   // иначе человек заходит в противодымное убежище                    
                        EnterSmokeCover(cover, currentTime);                
                }
                if (!IsInSmokeCover && currentTime - LastDecisionTime >= MakeDecisionDeltaTime)
                {
                    int deltaMoveTime = GetDeltaMoveTime(tunnel);
                    if (LastMoveTime == 0 || currentTime - LastMoveTime == deltaMoveTime)
                    {
                        Move(tunnel);
                        LastMoveTime = currentTime;
                    }
                }
            }
            else if (State != PersonState.DIED) // с учетом темперамента личности
            {
                if (PersonalityTemperament != PersonalityTemperament.MELANCHOLIC && !NearestSmokeCoverDetected())
                    DetectNearestSmokeCover(tunnel);
                if (PersonalityTemperament != PersonalityTemperament.MELANCHOLIC && NearestSmokeCoverDetected() && IsCloseToNearestSmokeCover())
                {
                    var cover = tunnel.FindSmokeCover(NearestSmokeCover);
                    // добавляем убежище в число посещенных и находим новое
                    if (cover.PeopleAmount + 1 >= cover.MaxPeopleAmount)
                    {
                        var currentSmokeCoverIndex = tunnel.FindSmokeCoverIndex(NearestSmokeCover);
                        if (currentSmokeCoverIndex != UNDEFINED_INDEX)
                            VisitedSmokeCoversIndexes.Add(currentSmokeCoverIndex);
                        DetectNearestSmokeCover(tunnel);
                    }
                    else   // иначе человек заходит в противодымное убежище              
                        EnterSmokeCover(cover, currentTime);                    
                }
                else if (PersonalityTemperament == PersonalityTemperament.MELANCHOLIC)
                {
                    var closestSmokeCover = FindCloseSmokeCover(tunnel.SmokeCovers);
                    if (closestSmokeCover != null && closestSmokeCover.PeopleAmount + 1 < closestSmokeCover.MaxPeopleAmount)
                    {
                        EnterSmokeCover(closestSmokeCover, currentTime);
                        /* if (closestSmokeCover.PeopleAmount + 1 >= closestSmokeCover.MaxPeopleAmount)
                         {
                             LastDecisionTime = currentTime;
                             //   ResetNearestSmokeCover();
                         }
                         else   // иначе человек заходит в противодымное убежище
                         {
                             EnterSmokeCover(closestSmokeCover, currentTime);
                         } */

                    }
                }

                if (!IsInSmokeCover && currentTime - LastDecisionTime >= MakeDecisionDeltaTime)
                {
                    int deltaMoveTime = GetDeltaMoveTime(tunnel);
                    if (LastMoveTime == 0 || currentTime - LastMoveTime == deltaMoveTime)
                    {
                        MoveWithPsyhotype(tunnel);
                        LastMoveTime = currentTime;
                    }
                }
            }

          /*  if (StartEvacuationTime == 0)
                StartEvacuationTime = currentTime;

            if (!NearestSmokeCoverDetected())            
                DetectNearestSmokeCover(tunnel.SmokeCovers);  
            if (Vector.Distance(Position, NearestSmokeCover) <= MIN_COVER_DISTANCE)
            {
                var cover = tunnel.FindSmokeCover(NearestSmokeCover); // FindSmokeCoverByKoordinates(tunnel.SmokeCovers, NearestSmokeCover);
                // если в противодымном убежище уже много народу
                if (cover.PeopleAmount + 1 >= cover.MaxPeopleAmount)          
                    DetectNearestSmokeCover(tunnel.SmokeCovers);                
                else   // иначе человек заходит в противодымное убежище
                {
                    IsInSmokeCover = true;
                    EndEvacuationTime = currentTime;
                    cover.PeopleAmount += 1;
                }
            }
            if (!IsInSmokeCover && currentTime - LastDecisionTime >= MakeDecisionDeltaTime)
            {
                int deltaMoveTime = GetDeltaMoveTime(tunnel);
                if (LastMoveTime == 0 || currentTime - LastMoveTime == deltaMoveTime)
                {
                    if (!considerPsychotype)
                    {
                        Move(tunnel);                   
                    }
                    else  // с учетом психотипа личности
                    {
                        if (PersonalityPsychotype == PersonalityPsychotype.CHOLERIC)
                        {
                            var person = FindNearestPersonInViewPoint(tunnel);
                            if (person != null)
                            {
                                NearestSmokeCover.X = person.NearestSmokeCover.X;
                                NearestSmokeCover.Y = person.NearestSmokeCover.Y;
                                DistanceToNearestSmokeCover = Vector.Distance(Position, NearestSmokeCover);
                            }
                            Move(tunnel);
                        }
                        else if (PersonalityPsychotype == PersonalityPsychotype.SANGUINE || PersonalityPsychotype == PersonalityPsychotype.PHLEGMATIC)
                        {
                            Move(tunnel);
                        }
                        else if (PersonalityPsychotype == PersonalityPsychotype.MELANCHOLIC)
                        {
                            var person = FindNearestPersonInViewPoint(tunnel);
                            if (person != null)
                            {
                                double distToPerson = 0;
                                var nearestToPersonCell = GetNeigbourFreeCellNearestToTargetCell(tunnel, new Cell(person.Position.X, person.Position.Y, 10), out distToPerson, out int cellI, out int cellJ);
                                if (nearestToPersonCell != null)
                                {
                                    AssignCell(nearestToPersonCell, cellI, cellJ);                
                                }
                            }
                            else
                            {
                                var freeCell = GetNeigbourFreeCell(tunnel, out int cellI, out int cellJ);
                                if (freeCell != null)
                                {
                                    AssignCell(freeCell, cellI, cellJ);
                                }
                            }
                        }           
                    }
                    LastMoveTime = currentTime;
                }
            }*/
        } 
    }
}
