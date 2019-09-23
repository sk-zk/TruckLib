using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class RoadTerrain
    {
        private float size;
        /// <summary>
        /// Terrain size in meters.
        /// </summary>
        public float Size
        {
            get => size;
            set => size = Utils.SetIfInRange(value, 0, 6500);
        }

        /// <summary>
        /// The terrain profile.
        /// </summary>
        public Token Profile { get; set; } = "profile0";

        /// <summary>
        /// Vertical scale of the terrain profile.
        /// </summary>
        public float Coefficient { get; set; } = 1f;

        public TerrainNoise Noise { get; set; } = TerrainNoise.Percent100;
        public TerrainTransition Transition { get; set; } = TerrainTransition._16;

        public TerrainQuadData QuadData { get; set; } = new TerrainQuadData();

        /// <summary>
        /// Updates the amount of quad columns and rows of this terrain.
        /// </summary>
        /// <param name="stepSize">The step size of the standalone terrain.</param>
        /// <param name="length">The length of the standalone terrain.</param>
        public void CalculateQuadGrid(StepSize stepSize, float length)
        {
            QuadData.Cols = (ushort)CalculateQuadCols(stepSize, length);
            QuadData.Rows = (ushort)CalculateQuadRows(Size);
        }

        /// <summary>
        /// Calculates the amount of quad columns in this terrain.
        /// </summary>
        /// <param name="stepSize">The step size of the standalone terrain.</param>
        /// <param name="length">The length of the standalone terrain.</param>
        /// <returns>The amount of quad columns</returns>
        private int CalculateQuadCols(StepSize stepSize, float length)
        {
            int terrainSteps;
            switch (stepSize)
            {
                case StepSize.Meters16:
                    terrainSteps = 16;
                    break;
                case StepSize.Meters12:
                    terrainSteps = 12;
                    break;
                case StepSize.Meters2:
                    terrainSteps = 2;
                    break;
                case StepSize.Meters4:
                default:
                    terrainSteps = 4;
                    break;
            }

            int cols = (int)(length / terrainSteps) + 1;
            return cols;
        }

        /// <summary>
        /// Updates the amount of quad columns and rows of this terrain.
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="length"></param>
        public void CalculateQuadGrid(RoadResolution resolution, float length)
        {
            QuadData.Cols = (ushort)CalculateQuadCols(resolution, length);
            QuadData.Rows = (ushort)CalculateQuadRows(Size);
        }

        /// <summary>
        /// Calculates the amount of quad columns in this terrain.
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static int CalculateQuadCols(RoadResolution resolution, float length)
        {
            int roadSteps;
            switch (resolution)
            {
                case RoadResolution.Superfine:
                    roadSteps = 1;
                    break;
                case RoadResolution.HighPoly:
                    roadSteps = 5;
                    break;
                case RoadResolution.Normal:
                default:
                    roadSteps = 15;
                    break;
            }

            int cols = (int)(length / roadSteps) + 1;
            return cols;
        }

        /// <summary>
        /// Calculates the amount of quad rows in this terrain.
        /// </summary>
        /// <param name="terrainSize">The terrain size.</param>
        /// <returns>The amount of quad rows.</returns>
        private int CalculateQuadRows(float terrainSize)
        {
            // rows
            // width of each row in meters:
            var rowWidthSequence = new int[] { 1, 1, 1, 2, 6, 6, 10, 10, 10,
                20, 40, 50, 50, 50, 100 };

            // get the amt of rows by subtracting the sequence of widths
            // from the terrain size until it is 0 or negative.
            // TODO: Check if the game ever uses a width > 100.
            var rows = 0;
            var remainder = terrainSize;
            while (remainder > 0)
            {
                if (rows < rowWidthSequence.Length)
                {
                    remainder -= rowWidthSequence[rows];
                }
                else
                {
                    remainder -= rowWidthSequence[rowWidthSequence.Last()];
                }
                rows++;
            }

            return rows;
        }

        public RoadTerrain Clone()
        {
            var rt = (RoadTerrain)MemberwiseClone();
            rt.QuadData = QuadData.Clone();
            return rt;
        }
    }
}
