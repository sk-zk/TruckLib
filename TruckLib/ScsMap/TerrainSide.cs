using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of one side of a terrain item.
    /// </summary>
    public class TerrainSide
    {
        /// <summary>
        /// Unit name of the edge model on this side.
        /// </summary>
        public Token Edge { get; set; }

        /// <summary>
        /// The look of the edge model.
        /// </summary>
        public Token EdgeLook { get; set; }

        /// <summary>
        /// The terrain on this side.
        /// </summary>
        public RoadTerrain Terrain { get; set; }

        /// <summary>
        /// The vegetation on this side.
        /// </summary>
        public RoadVegetation[] Vegetation { get; set; }

        /// <summary>
        /// Gets or sets if vegetation has collision.
        /// </summary>
        public bool VegetationCollision;

        /// <summary>
        /// Gets or sets if detail vegetation (small clumps of grass etc.) is rendered
        /// if the selected terrain material supports it.
        /// </summary>
        public bool DetailVegetation;

        /// <summary>
        /// The start of the band, in meters, in which detail vegetation is not rendered.
        /// </summary>
        public float NoDetailVegetationFrom { get; set; }

        /// <summary>
        /// The end of the band, in meters, in which detail vegetation is not rendered.
        /// </summary>
        public float NoDetailVegetationTo { get; set; }

        /// <summary>
        /// Instantiates a new TerrainSide object with default values.
        /// </summary>
        public TerrainSide()
        {
            const int vegetationCount = 3;
            Vegetation = (new RoadVegetation[vegetationCount])
                .Select(h => new RoadVegetation()).ToArray();

            Init();
        }

        internal TerrainSide(bool initFields)
        {
            const int vegetationCount = 3;
            Vegetation = (new RoadVegetation[vegetationCount])
                .Select(h => new RoadVegetation()).ToArray();

            if (initFields) Init();
        }

        /// <summary>
        /// Sets the object to its default values.
        /// </summary>
        protected void Init()
        {
            DetailVegetation = true;
            Terrain = new RoadTerrain();
        }
    }
}
