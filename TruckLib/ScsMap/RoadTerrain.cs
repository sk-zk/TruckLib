using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of the procedurally generated terrain of one side of a <see cref="Road"/>
    /// or <see cref="Terrain"/>.
    /// </summary>
    public class RoadTerrain : EdgeTerrain
    {
        /// <summary>
        /// Gets or sets the strength of random noise applied to the vertices of the terrain.
        /// </summary>
        public TerrainNoise Noise { get; set; }

        /// <summary>
        /// Length of terrain transition to neighboring segment.
        /// </summary>
        public TerrainTransition Transition { get; set; }

        /// <summary>
        /// Instantiates a RoadTerrain with its default values.
        /// </summary>
        public RoadTerrain()
        {
            Init();
        }

        internal RoadTerrain(bool initFields)
        {
            if (initFields) Init();
        }

        /// <summary>
        /// Sets the RoadTerrain's properties to its default values.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            Noise = TerrainNoise.Percent100;
            Transition = TerrainTransition._16;
        }

        /// <summary>
        /// Updates the amount of quad columns and rows of this terrain.
        /// </summary>
        /// <param name="resolution">The resolution of the road.</param>
        /// <param name="length">The length of the road.</param>
        public void CalculateQuadGrid(RoadResolution resolution, float length)
        {
            QuadData.Cols = (ushort)CalculateQuadCols(resolution, length);
            QuadData.Rows = (ushort)CalculateQuadRows(Size);
            UpdateQuadAmount();
        }

        /// <summary>
        /// Calculates the amount of quad columns in this terrain.
        /// </summary>
        /// <param name="resolution">The resolution of the road.</param>
        /// <param name="length">The length of the road.</param>
        /// <returns>The amount of quad columns.</returns>
        private int CalculateQuadCols(RoadResolution resolution, float length)
        {
            int interval;
            int colsPerInterval;
            switch (resolution)
            {
                case RoadResolution.Superfine:
                    interval = 1;
                    colsPerInterval = 1;
                    break;
                case RoadResolution.HighPoly:
                    interval = 15;
                    colsPerInterval = 3;
                    break;
                case RoadResolution.Normal:
                default:
                    interval = 15;
                    colsPerInterval = 1;
                    break;
            }
            var cols = ((int)(length / interval) + 1) * colsPerInterval; 
            return cols;
        }

        /// <summary>
        /// Updates the amount of quad columns and rows of this terrain.
        /// </summary>
        /// <param name="stepSize">The step size of the standalone terrain.</param>
        /// <param name="length">The length of the standalone terrain.</param>
        public void CalculateQuadGrid(StepSize stepSize, float length)
        {
            QuadData.Cols = (ushort)CalculateQuadCols(stepSize, length);
            QuadData.Rows = (ushort)CalculateQuadRows(Size);
            UpdateQuadAmount();
        }

        /// <summary>
        /// Calculates the amount of quad columns in this terrain.
        /// </summary>
        /// <param name="stepSize">The step size of the standalone terrain.</param>
        /// <param name="length">The length of the standalone terrain.</param>
        /// <returns>The amount of quad columns.</returns>
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

            int cols = (int)(length / terrainSteps);
            return cols;
        }

        /// <summary>
        /// Makes a deep copy of this object.
        /// </summary>
        /// <returns>A deep copy of this object.</returns>
        public RoadTerrain Clone()
        {
            var rt = (RoadTerrain)MemberwiseClone();
            rt.QuadData = QuadData.Clone();
            return rt;
        }
    }
}
