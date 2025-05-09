﻿using System;
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
        /// Length of the terain transition from the profile of the backward road item
        /// to this one, given as the number of quads.
        /// </summary>
        /// <remarks>Note that the <see cref="RoadResolution"/> flag affects this.</remarks>
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
            Transition = TerrainTransition.Quads16;
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
            UpdateQuadList();
        }

        /// <summary>
        /// Calculates the amount of quad columns in this terrain.
        /// </summary>
        /// <param name="resolution">The resolution of the road.</param>
        /// <param name="length">The length of the road.</param>
        /// <returns>The amount of quad columns.</returns>
        private static int CalculateQuadCols(RoadResolution resolution, float length)
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
        /// <param name="adaptiveTessellation">Whether adaptive tessellation is enabled.</param>
        public void CalculateQuadGrid(StepSize stepSize, float length, bool adaptiveTessellation)
        {
            QuadData.Cols = (ushort)CalculateQuadCols(stepSize, length, adaptiveTessellation);
            QuadData.Rows = (ushort)CalculateQuadRows(Size);
            UpdateQuadList();
        }

        /// <summary>
        /// Calculates the amount of quad columns in this terrain.
        /// </summary>
        /// <param name="stepSize">The step size of the standalone terrain.</param>
        /// <param name="length">The length of the standalone terrain.</param>
        /// <param name="adaptiveTessellation">Whether adaptive tessellation is enabled.</param>
        /// <returns>The amount of quad columns.</returns>
        private static int CalculateQuadCols(StepSize stepSize, float length, bool adaptiveTessellation)
        {
            float terrainSteps;
            if (adaptiveTessellation && stepSize == StepSize.Meters2 && length < 2)
            {
                terrainSteps = 0.2f;
            }
            else if (adaptiveTessellation && stepSize == StepSize.Meters16 && length > 64)
            {
                terrainSteps = 32f;
            }
            else
            {
                terrainSteps = stepSize switch
                {
                    StepSize.Meters16 => 16,
                    StepSize.Meters12 => 12,
                    StepSize.Meters4 => 4,
                    StepSize.Meters2 => 2,
                    _ => 4,
                };
            }
            int cols = (int)Math.Ceiling(length / terrainSteps);
            return cols;
        }

        /// <summary>
        /// Copies the properties of this RoadTerrain object to another.
        /// </summary>
        /// <param name="other">The object to copy properties to.</param>
        public void CopyTo(RoadTerrain other)
        {
            other.Size = Size;
            other.Profile = Profile;
            other.Coefficient = Coefficient;
            other.Noise = Noise;
            other.Transition = Transition;
            other.QuadData.BrushMaterials = new(QuadData.BrushMaterials);
            other.QuadData.BrushColors = new(QuadData.BrushColors);
        }
    }
}
