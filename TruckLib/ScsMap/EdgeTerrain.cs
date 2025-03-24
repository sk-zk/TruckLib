using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for <see cref="RoadTerrain"/> and <see cref="PrefabTerrain"/>.
    /// </summary>
    public abstract class EdgeTerrain
    {
        /// <summary>
        /// Lengths of terrain rows in meters. 
        /// <para>After row 15, every row is 100 meters in size.</para>
        /// </summary>
        public static readonly int[] RowWidthSequence = 
            [1, 1, 1, 2, 6, 6, 10, 10, 10, 20, 40, 50, 50, 50, 100];

        private float size;
        /// <summary>
        /// Terrain size in meters. Must be between 0 and 6500.
        /// </summary>
        public float Size
        {
            get => size;
            set 
            {
                size = Utils.SetIfInRange(value, 0, 6500);
                if (QuadData is not null)
                {
                    QuadData.Rows = (ushort)CalculateQuadRows(size);
                    UpdateQuadList();
                }
            }
        }
        
        /// <summary>
        /// Unit name of the terrain profile.
        /// </summary>
        public Token Profile { get; set; }

        /// <summary>
        /// Vertical scale coefficient of the terrain profile.
        /// </summary>
        public float Coefficient { get; set; }

        /// <summary>
        /// Properties of the terrain quads.
        /// </summary>
        public TerrainQuadData QuadData { get; set; }

        /// <summary>
        /// Sets the RoadTerrain's properties to its default values.
        /// </summary>
        protected virtual void Init()
        {
            Profile = "profile0";
            Coefficient = 1f;
            QuadData = new TerrainQuadData();
        }

        /// <summary>
        /// Returns the width of a terrain quad row at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The width of a terrain quad row at that index.</returns>
        public static int GetRowWidthAt(int index)
        {
            return index < RowWidthSequence.Length 
                ? RowWidthSequence[index] 
                : RowWidthSequence[^1];
        }

        /// <summary>
        /// Calculates the amount of quad rows in this terrain.
        /// </summary>
        /// <param name="terrainSize">The terrain size.</param>
        /// <returns>The amount of quad rows.</returns>
        protected static int CalculateQuadRows(float terrainSize)
        {
            // get the amt of rows by subtracting the sequence of widths
            // from the terrain size until it is 0 or negative.
            // TODO: Check if the game ever uses a width > 100.
            var rowIdx = 0;
            var remainder = terrainSize;
            while (remainder > 0)
            {
                if (rowIdx < RowWidthSequence.Length)
                {
                    remainder -= RowWidthSequence[rowIdx];
                }
                else
                {
                    remainder -= RowWidthSequence[^1];
                }
                rowIdx++;
            }
            return rowIdx;
        }

        protected void UpdateQuadList()
        {
            var amount = QuadData.Cols * QuadData.Rows;

            var quads = QuadData.Quads;
            if (amount == 0)
            {
                quads.Clear();
                return;
            }

            if (amount == quads.Count)
                return;

            if (quads.Count < amount)
            {
                var missing = amount - quads.Count;
                quads.Capacity += missing;
                for (int i = 0; i < missing; i++)
                {
                    quads.Add(new());
                }
            }
            else
            {
                quads.RemoveRange(amount, quads.Count - amount);
            }
        }
    }
}
