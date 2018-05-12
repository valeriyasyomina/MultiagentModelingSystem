using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class RoadwayMatrix
    {
        public static int CELL_SIZE = 10;

        public Cell[,] Data { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public void Initialize(int row, int columns)
        {
            Data = new Cell[row, columns];
            Rows = row;
            Columns = columns;

            int cellRadius = CELL_SIZE / 2;
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    Data[i, j] = new Cell(j * CELL_SIZE + cellRadius, i * CELL_SIZE + cellRadius, cellRadius);
        }
    }
}
