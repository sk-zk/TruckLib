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

        public Token RightEdge { get; set; }

        public Token LeftEdge { get; set; }

        public float RoadHeightOffset { get; set; }

        public bool ShoulderBlocked = false;

        /// <summary>
        /// Sidewalk for legacy city roads.
        /// </summary>
        public Sidewalk Sidewalk { get; set; } = new Sidewalk();

        /// <summary>
        /// Terrain for this side of the road.
        /// </summary>
        public RoadTerrain Terrain { get; set; } = new RoadTerrain();

        /// <summary>
        /// Vegetation on this side of the road.
        /// </summary>
        public RoadVegetation[] Vegetation { get; set; }

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is drawn.
        /// </summary>
        public bool NoDetailVegetation = false;
        public float NoDetailVegetationFrom { get; set; }
        public float NoDetailVegetationTo { get; set; }

        /// <summary>
        /// Determines if the player can collide with vegetation.
        /// </summary>
        public bool VegetationCollision = false;

        /// <summary>
        /// Models on the side of a road.
        /// </summary>
        public RoadModel[] Models { get; set; }

        /// <summary>
        /// Railings on or on the side of road.
        /// </summary>
        public RoadRailings Railings { get; set; } = new RoadRailings();

        public List<Token> AdditionalParts { get; set; } = new List<Token>();

        public float UVRotation { get; set; }

        public RoadSide()
        {
            const int modelCount = 2;
            Models = (new RoadModel[modelCount]).Select(h => new RoadModel()).ToArray();
            const int vegetationCount = 3;
            Vegetation = (new RoadVegetation[vegetationCount]).Select(h => new RoadVegetation()).ToArray();
        }

        public RoadSide Clone()
        {
            var rs = (RoadSide)MemberwiseClone();
            rs.Terrain = Terrain.Clone();
            for(int i = 0; i < Vegetation.Length; i++)
            {
                rs.Vegetation[i] = Vegetation[i].Clone();
            }
            for (int i = 0; i < Models.Length; i++)
            {
                rs.Models[i] = Models[i].Clone();
            }
            rs.Railings = Railings.Clone();
            rs.AdditionalParts = new List<Token>(AdditionalParts);
            return rs;
        }
    }

}
