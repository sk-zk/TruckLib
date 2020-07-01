using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A standalone terrain segment which acts like a terrain-only road.
    /// </summary>
    public class Terrain : PolylineItem
    {
        // TODO: Use the new KdopItem system.
        // not sure how to implement it while keeping the flexibility
        // of Left/Right objects, RoadTerrain class, Clone() methods etc.
        public override ItemType ItemType => ItemType.Terrain;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public float MinLength => StepSize switch
        {
            // step size / 2
            StepSize.Meters2 => 1,
            StepSize.Meters4 => 2,
            StepSize.Meters12 => 6,
            StepSize.Meters16 => 8,
            _ => 8,
        };

        public float MaxLength => 99999f;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// Terrain and vegetation on the left side.
        /// </summary>
        public TerrainSide Left { get; set; } = new TerrainSide();

        /// <summary>
        /// Terrain and vegetation on the right side.
        /// </summary>
        public TerrainSide Right { get; set; } = new TerrainSide();

        /// <summary>
        /// The terrain quad density. Roads would do this with hi-poly and superfine flags.
        /// </summary>
        public StepSize StepSize { get; set; }

        public Railings Railings { get; set; } = new Railings();

        /// <summary>
        /// The random seed which determines the placement of vegetation models.
        /// </summary>
        public uint RandomSeed { get; set; }

        /// <summary>
        /// The vegetation spheres on this terrain.
        /// </summary>
        public List<VegetationSphere> VegetationSpheres { get; set; }

        public Vector3 NodeOffset { get; set; }

        public Vector3 ForwardNodeOffset { get; set; }

        /// <summary>
        /// Determines if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection = false;

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes = false;

        /// <summary>
        /// Determines if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation = false;

        [Obsolete]
        public bool StretchTerrain = false;

        /// <summary>
        /// Determines if the terrain has invisible walls on both sides of it.
        /// </summary>
        public bool Boundary = true;

        public bool Collision = true;

        public bool TerrainShadows = true;

        public bool SmoothDetailVegetation = false;

        public Terrain() : base() { }

        internal Terrain(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            StepSize = StepSize.Meters4;
            VegetationSpheres = new List<VegetationSphere>();
        }

        /// <summary>
        /// Adds a single terrain segment to the map.
        /// </summary>
        /// <param name="map">The map the terrain will be added to.</param>
        /// <param name="backwardPos">The position of the backward (red) node.</param>
        /// <param name="forwardPos">The position of the forward (green) node.</param>
        /// <param name="material">The terrain material.</param>
        /// <param name="leftSize">The terrain size on the left side.</param>        
        /// <param name="rightSize">The terrain size on the right side.</param>
        /// <returns>A new terrain.</returns>
        public static Terrain Add(IItemContainer map, Vector3 backwardPos, Vector3 forwardPos, Token material,
            float leftSize, float rightSize)
        {
            var terrain = Add<Terrain>(map, backwardPos, forwardPos);
            terrain.InitFromAddOrAppend(backwardPos, forwardPos, material, leftSize, rightSize);
            return terrain;
        }

        /// <summary>
        /// Appends a terrain segment to this terrain.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="material"></param>
        /// <param name="leftSize"></param>
        /// <param name="rightSize"></param>
        /// <returns></returns>
        public Terrain Append(Vector3 position, Token material, float leftSize, float rightSize)
        {
            var terrain = Append<Terrain>(position);
            terrain.InitFromAddOrAppend(ForwardNode.Position, position, material, leftSize, rightSize);
            return terrain;
        }

        /// <summary>
        /// Initializes a terrain which has been created via Add or Append.
        /// </summary>
        internal void InitFromAddOrAppend(Vector3 backwardPos, Vector3 forwardPos, Token material,
            float leftSize, float rightSize)
        {
            Length = Vector3.Distance(backwardPos, forwardPos);
            Left.Terrain.Size = leftSize;
            Right.Terrain.Size = rightSize;
            foreach (var side in new[] { Left, Right })
            {
                side.Terrain.QuadData.BrushMaterials = new List<Token> { material };
                side.Terrain.CalculateQuadGrid(StepSize, Length);
            }
        }

        public override void Recalculate()
        {
            base.Recalculate();
            Left.Terrain.CalculateQuadGrid(StepSize, Length);
            Right.Terrain.CalculateQuadGrid(StepSize, Length);
        }
    }
}
