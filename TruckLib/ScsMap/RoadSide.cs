using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Holds everything that exists on both sides of a road, like vegetation and railings.
    /// </summary>
    public class RoadSide
    {
        public Token Variant { get; set; }

        public Token Look { get; set; }

        public Token LanesVariant { get; set; }

        public Token RightEdge { get; set; }

        public Token LeftEdge { get; set; }

        public float RoadHeightOffset { get; set; }

        /// <summary>
        /// Gets or sets if visual details in shoulders cannot spawn on this side.
        /// </summary>
        public bool ShoulderBlocked = false;

        /// <summary>
        /// Sidewalk for legacy city roads.
        /// </summary>
        public Sidewalk Sidewalk { get; set; } = new Sidewalk();

        /// <summary>
        /// Terrain for this side of the road.
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
        public bool DetailVegetation = true;
        public float NoDetailVegetationFrom { get; set; }
        public float NoDetailVegetationTo { get; set; }

        /// <summary>
        /// Gets or sets if vegetation has collision.
        /// </summary>
        public bool VegetationCollision = false;

        /// <summary>
        /// Models on the side of a road.
        /// </summary>
        public RoadModel[] Models { get; set; }

        /// <summary>
        /// Railings on the side of road.
        /// </summary>
        public RoadRailings Railings { get; set; } = new RoadRailings();

        public List<Token> AdditionalParts { get; set; } = new List<Token>();

        public List<EdgeOverride> EdgeOverrides { get; set; }

        public List<VariantOverride> VariantOverrides { get; set; }

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

        protected void Init()
        {
            Terrain = new RoadTerrain();
            EdgeOverrides = new List<EdgeOverride>();
            VariantOverrides = new List<VariantOverride>();
        }

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
