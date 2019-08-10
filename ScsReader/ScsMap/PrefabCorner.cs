using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Holds vegetation, terrain and model data for one corner of a prefab.
    /// </summary>
    public class PrefabCorner
    {
        /// <summary>
        /// The terrain of this corner.
        /// </summary>
        public RoadTerrain Terrain { get; set; } = new RoadTerrain();

        /// <summary>
        /// The vegetation of this corner.
        /// </summary>
        public RoadVegetation[] Vegetation { get; set; }

        public float DetailVegetationFrom { get; set; } = 5;

        public float DetailVegetationTo { get; set; } = 100;

        /// <summary>
        /// The unit name of the corner model.
        /// </summary>
        public Token Model { get; set; }

        /// <summary>
        /// The corner look.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// The corner variant.
        /// </summary>
        public Token Variant { get; set; }

        public PrefabCorner()
        {
           const int vegetationAmnt = 2;
           Vegetation = new RoadVegetation[vegetationAmnt].Select(h => new RoadVegetation()).ToArray();
        }
    }
}
