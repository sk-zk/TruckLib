using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Holds vegetation, terrain and model data for one corner of a prefab.
    /// </summary>
    public class PrefabCorner
    {
        /// <summary>
        /// The terrain of this corner.
        /// </summary>
        public RoadTerrain Terrain { get; set; }

        /// <summary>
        /// The vegetation of this corner.
        /// </summary>
        public RoadVegetation[] Vegetation { get; set; }

        public float DetailVegetationFrom { get; set; }

        public float DetailVegetationTo { get; set; }

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
            Vegetation = new RoadVegetation[vegetationAmnt]
                 .Select(h => new RoadVegetation()).ToArray();

            Init();
        }

        internal PrefabCorner(bool initFields)
        {
            const int vegetationAmnt = 2;
            Vegetation = new RoadVegetation[vegetationAmnt]
                 .Select(h => new RoadVegetation()).ToArray();

            if (initFields) Init();
        }

        protected void Init()
        {
            Terrain = new RoadTerrain();
            DetailVegetationFrom = 5;
            DetailVegetationTo = 100;
        }
    }
}
