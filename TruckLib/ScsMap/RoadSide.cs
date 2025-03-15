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
        private FlagField tmpFlags = new();

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
        /// Unit name of the right edge model, as defined in <c>/def/world/road_edge.sii</c>.
        /// </summary>
        public Token RightEdge { get; set; }

        /// <summary>
        /// Unit name of the left edge model, as defined in <c>/def/world/road_edge.sii</c>.
        /// </summary>
        public Token LeftEdge { get; set; }

        /// <summary>
        /// Height offset of this side in meters.
        /// </summary>
        public float HeightOffset { get; set; }

        /// <summary>
        /// Gets or sets if visual details in shoulders cannot spawn on this side.
        /// </summary>
        public bool ShoulderBlocked
        {
            get => tmpFlags[0];
            set => tmpFlags[0] = value;
        }

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
        public bool DetailVegetation
        {
            get => tmpFlags[1];
            set => tmpFlags[1] = value;
        }

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
        public bool VegetationCollision
        {
            get => tmpFlags[2];
            set => tmpFlags[2] = value;
        }

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
        public List<Token> AdditionalParts { get; set; } = [];

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
            EdgeOverrides = [];
            VariantOverrides = [];
            DetailVegetation = true;
        }

        /// <summary>
        /// Copies the properties of this RoadSide object to another.
        /// </summary>
        /// <param name="other">The object to copy properties to.</param>
        public void CopyTo(RoadSide other)
        {
            other.Variant = Variant;
            other.Look = Look;
            other.TrafficRule = TrafficRule;
            other.RightEdge = RightEdge;
            other.LeftEdge = LeftEdge;
            other.HeightOffset = HeightOffset;
            other.NoDetailVegetationFrom = NoDetailVegetationFrom;
            other.NoDetailVegetationTo = NoDetailVegetationTo;

            Terrain.CopyTo(other.Terrain);

            other.Sidewalk = Sidewalk.Clone();
            for(int i = 0; i < Vegetation.Length; i++)
            {
                other.Vegetation[i] = Vegetation[i].Clone();
            }
            for (int i = 0; i < Models.Length; i++)
            {
                other.Models[i] = Models[i].Clone();
            }
            other.Railings = Railings.Clone();
            other.AdditionalParts = new List<Token>(AdditionalParts);
            other.EdgeOverrides = new List<EdgeOverride>(EdgeOverrides);
            other.VariantOverrides = new List<VariantOverride>(VariantOverrides);
            other.tmpFlags = tmpFlags;
        }
    }
}
