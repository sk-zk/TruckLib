using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Holds terrain and vegetation data for one side of a standalone terrain segment.
    /// </summary>
    public class TerrainSide
    {
        public Token Edge { get; set; }

        public Token EdgeLook { get; set; }

        /// <summary>
        /// The terrain on this side.
        /// </summary>
        public RoadTerrain Terrain { get; set; } = new RoadTerrain();

        /// <summary>
        /// The vegetation on this side.
        /// </summary>
        public RoadVegetation[] Vegetation { get; set; }

        /// <summary>
        /// Determines if the player can collide with vegetation.
        /// </summary>
        public bool VegetationCollision = false;

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is drawn.
        /// </summary>
        public bool NoDetailVegetation = false;

        public float NoDetailVegetationFrom { get; set; }

        public float NoDetailVegetationTo { get; set; }

        public TerrainSide()
        {
            const int vegetationCount = 3;
            Vegetation = (new RoadVegetation[vegetationCount]).Select(h => new RoadVegetation()).ToArray();
        }
    }
}
