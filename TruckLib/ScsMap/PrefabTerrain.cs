using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of procedurally generated terrain of one edge of a <see cref="Prefab"/>. 
    /// </summary>
    public class PrefabTerrain : EdgeTerrain
    {
        internal int TerrainPointCount { get; set; }

        /// <summary>
        /// Instantiates a PrefabTerrain with its default values.
        /// </summary>
        public PrefabTerrain()
        {
            Init();
        }

        internal PrefabTerrain(bool initFields)
        {
            if (initFields) Init();
        }

        /// <summary>
        /// Sets the PrefabTerrain's properties to its default values.
        /// </summary>
        protected override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Updates the amount of quad columns and rows of this terrain.
        /// </summary>
        /// <param name="stepSize">The step size of the standalone terrain.</param>
        /// <param name="length">The length of the standalone terrain.</param>
        public void CalculateQuadGrid()
        {
            QuadData.Cols = TerrainPointCount > 0 ? (ushort)(TerrainPointCount - 1) : (ushort)0;
            QuadData.Rows = (ushort)CalculateQuadRows(Size);
            UpdateQuadList();
        }
    }
}
