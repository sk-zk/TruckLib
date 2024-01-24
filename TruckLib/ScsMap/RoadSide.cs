using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of one side of a road, such as vegetation and railings.
    /// </summary>
    public class RoadSide
    {
        /// <summary>
        /// Unit name of the road variant.
        /// </summary>
        public Token Variant { get; set; }

        /// <summary>
        /// Unit name of the road look.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// Unit name of the traffic rule variant.
        /// </summary>
        public Token TrafficRule { get; set; }

        /// <summary>
        /// Unit name of the right edge look.
        /// </summary>
        public Token RightEdge { get; set; }

        /// <summary>
        /// Unit name of the left edge look.
        /// </summary>
        public Token LeftEdge { get; set; }

        /// <summary>
        /// Height offset of this side in meters.
        /// </summary>
        public float HeightOffset { get; set; }

        /// <summary>
        /// Gets or sets if visual details in shoulders cannot spawn on this side.
        /// </summary>
        public bool ShoulderBlocked { get; set; } = false;

        /// <summary>
        /// Sidewalk for legacy city roads.
        /// </summary>
        public Sidewalk Sidewalk { get; set; } = new Sidewalk();

        /// <summary>
        /// Terrain on this side of the road.
        /// </summary>
        public RoadTerrain Terrain { get; set; }

        /// <summary>
        /// Vegetation on this side of the road.
        /// </summary>
        public RoadVegetation[] Vegetation { get; set; }

        /// <summary>
        /// Gets or sets if detail vegetation (small clumps of grass etc.) is rendered
        /// if the selected terrain material supports it.
        /// </summary>
        public bool DetailVegetation { get; set; } = true;

        /// <summary>
        /// The start of the band, in meters, in which detail vegetation is not rendered.
        /// </summary>
        public float NoDetailVegetationFrom { get; set; }

        /// <summary>
        /// The end of the band, in meters, in which detail vegetation is not rendered.
        /// </summary>
        public float NoDetailVegetationTo { get; set; }

        /// <summary>
        /// Gets or sets if vegetation has collision.
        /// </summary>
        public bool VegetationCollision = false;

        /// <summary>
        /// Models on this side of the road.
        /// </summary>
        public RoadModel[] Models { get; set; }

        /// <summary>
        /// Railings on this side the road.
        /// </summary>
        public RoadRailings Railings { get; set; } = new RoadRailings();

        /// <summary>
        /// Unit names of enabled additional parts.
        /// </summary>
        public List<Token> AdditionalParts { get; set; } = new List<Token>();

        /// <summary>
        /// Edge model overrides on this side of the road.
        /// </summary>
        public List<EdgeOverride> EdgeOverrides { get; set; }

        /// <summary>
        /// Model variant override on this side of the road.
        /// </summary>
        public List<VariantOverride> VariantOverrides { get; set; }

        /// <summary>
        /// Instantiates a new RoadSide object with default values.
        /// </summary>
        public RoadSide()
        {
            const int modelCount = 2;
            Models = (new RoadModel[modelCount]).Select(h => new RoadModel()).ToArray();
            const int vegetationCount = 3;
            Vegetation = (new RoadVegetation[vegetationCount]).Select(h => new RoadVegetation()).ToArray();

            Init();
        }

        internal RoadSide(bool initFields)
        {
            const int modelCount = 2;
            Models = (new RoadModel[modelCount]).Select(h => new RoadModel()).ToArray();
            const int vegetationCount = 3;
            Vegetation = (new RoadVegetation[vegetationCount]).Select(h => new RoadVegetation()).ToArray();

            if (initFields) Init();
        }

        /// <summary>
        /// Sets the object to its default values.
        /// </summary>
        protected void Init()
        {
            Terrain = new RoadTerrain();
            EdgeOverrides = new List<EdgeOverride>();
            VariantOverrides = new List<VariantOverride>();
        }

        /// <summary>
        /// Makes a deep copy of this object.
        /// </summary>
        /// <returns>A deep copy of this object.</returns>
        public RoadSide Clone()
        {
            var cloned = (RoadSide)MemberwiseClone();
            cloned.Terrain = Terrain.Clone();
            cloned.Sidewalk = Sidewalk.Clone();
            for(int i = 0; i < Vegetation.Length; i++)
            {
                cloned.Vegetation[i] = Vegetation[i].Clone();
            }
            for (int i = 0; i < Models.Length; i++)
            {
                cloned.Models[i] = Models[i].Clone();
            }
            cloned.Railings = Railings.Clone();
            cloned.AdditionalParts = new List<Token>(AdditionalParts);
            cloned.EdgeOverrides = new List<EdgeOverride>(EdgeOverrides);
            cloned.VariantOverrides = new List<VariantOverride>(VariantOverrides);
            return cloned;
        }
    }

}
