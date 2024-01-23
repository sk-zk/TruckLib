using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Holds vegetation, terrain, and corner model properties for one node of a prefab.
    /// </summary>
    public class PrefabNode
    {
        /// <summary>
        /// The terrain of this node.
        /// </summary>
        public PrefabTerrain Terrain { get; set; }

        /// <summary>
        /// The vegetation of this node.
        /// </summary>
        public RoadVegetation[] Vegetation { get; set; }

        /// <summary>
        /// Distance from the edge of the node, in meters, where the band in which detail
        /// vegetation will be placed begins.
        /// </summary>
        public float DetailVegetationFrom { get; set; }

        /// <summary>
        /// Distance from the edge of the node, in meters, where the band in which detail
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
        /// Instantiates a PrefabNode with its default values.
        /// </summary>
        public PrefabNode()
        {
            const int vegetationAmnt = 2;
            Vegetation = new RoadVegetation[vegetationAmnt]
                 .Select(h => new RoadVegetation()).ToArray();

            Init();
        }

        internal PrefabNode(bool initFields)
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
            Terrain = new PrefabTerrain();
            DetailVegetationFrom = 5;
            DetailVegetationTo = 100;
        }
    }
}
