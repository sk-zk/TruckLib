using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Holds everything that exists on both sides of a road, like vegetation and railings.
    /// </summary>
    public class RoadSide
    {
        public Token Variant;

        public Token Look;

        public Token RightEdge;

        public Token LeftEdge;

        public float RoadHeightOffset;

        public bool ShoulderBlocked = false;

        /// <summary>
        /// Sidewalk for legacy city roads.
        /// </summary>
        public Sidewalk Sidewalk = new Sidewalk();

        /// <summary>
        /// Terrain for this side of the road.
        /// </summary>
        public RoadTerrain Terrain = new RoadTerrain();

        /// <summary>
        /// Vegetation on this side of the road.
        /// </summary>
        public RoadVegetation[] Vegetation;

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is drawn.
        /// </summary>
        public bool NoDetailVegetation = false;
        public float NoDetailVegetationFrom;
        public float NoDetailVegetationTo;

        /// <summary>
        /// Determines if the player can collide with vegetation.
        /// </summary>
        public bool VegetationCollision = false;

        /// <summary>
        /// Models on the side of a road.
        /// </summary>
        public RoadModel[] Models;

        /// <summary>
        /// Railings on or on the side of road.
        /// </summary>
        public RoadRailings Railings = new RoadRailings();

        public List<Token> AdditionalParts = new List<Token>();

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
