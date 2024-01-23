using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Holds vegetation, terrain, and corner model properties for one corner of a prefab.
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

        /// <summary>
        /// Distance from the edge of the prefab, in meters, where the band in which detail
        /// vegetation will be placed begins.
        /// </summary>
        public float DetailVegetationFrom { get; set; }

        /// <summary>
        /// Distance from the edge of the prefab, in meters, where the band in which detail
        /// vegetation will be placed ends.
        /// </summary>
        public float DetailVegetationTo { get; set; }

        /// <summary>
        /// The unit name of the corner model.
        /// </summary>
        public Token Model { get; set; }

        /// <summary>
        /// The look of the corner model.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// The variant of the corner model.
        /// </summary>
        public Token Variant { get; set; }

        /// <summary>
        /// Instantiates a PrefabCorner with its default values.
        /// </summary>
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

        /// <summary>
        /// Sets the PrefabCorner's properties to its default values.
        /// </summary>
        protected void Init()
        {
            Terrain = new RoadTerrain();
            DetailVegetationFrom = 5;
            DetailVegetationTo = 100;
        }
    }
}
