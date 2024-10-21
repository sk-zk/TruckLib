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
    /// A standalone terrain item which acts like a terrain-only road.
    /// </summary>
    public class Terrain : PolylineItem
    {
        // TODO: Use the KdopItem system.
        // not sure how to implement it while keeping the flexibility
        // of Left/Right objects, RoadTerrain class, Clone() methods etc.

        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Terrain;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Returns the minimum length of a terrain item with the current step size setting.
        /// </summary>
        public float MinLength => StepSize switch
        {
            // step size / 2
            StepSize.Meters2 => 1,
            StepSize.Meters4 => 2,
            StepSize.Meters12 => 6,
            StepSize.Meters16 => 8,
            _ => 8,
        };

        /// <summary>
        /// The maximum length of a terrain item.
        /// </summary>
        public float MaxLength => 99999f;

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// Terrain and vegetation on the left side.
        /// </summary>
        public TerrainSide Left { get; set; }

        /// <summary>
        /// Terrain and vegetation on the right side.
        /// </summary>
        public TerrainSide Right { get; set; }

        /// <summary>
        /// The terrain quad density.
        /// </summary>
        public StepSize StepSize { get; set; }

        /// <summary>
        /// The railings of this terrain.
        /// </summary>
        public Railings Railings { get; set; }

        /// <summary>
        /// The seed for the RNG which determines which vegetation models
        /// to place. The position of the models does not appear to be
        /// affected by this.
        /// </summary>
        public uint RandomSeed { get; set; }

        /// <summary>
        /// The vegetation spheres on this terrain.
        /// </summary>
        public List<VegetationSphere> VegetationSpheres { get; set; }

        public Vector3 NodeOffset { get; set; }

        public Vector3 ForwardNodeOffset { get; set; }

        /// <summary>
        /// Gets or sets if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection { get; set; } = false;

        /// <summary>
        /// Gets or sets if this item will render behind cut planes.
        /// </summary>
        public bool IgnoreCutPlanes { get; set; } = false;

        /// <summary>
        /// Gets or sets if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation { get; set; } = false;

        [Obsolete]
        public bool StretchTerrain { get; set; } = false;

        /// <summary>
        /// Gets or sets if the terrain has invisible walls on both sides of it.
        /// </summary>
        public bool Boundary { get; set; } = true;

        /// <summary>
        /// Gets or sets if collision is enabled.
        /// </summary>
        public bool Collision { get; set; } = true;

        public bool TerrainShadows { get; set; } = true;

        /// <summary>
        /// Gets or sets if detail vegetation transitions smoothly in places
        /// where it is affected by brushes.
        /// </summary>
        public bool SmoothDetailVegetation { get; set; } = false;

        public bool Unknown { get; set; } = false;
        public bool Unknown2 { get; set; } = false;

        public Terrain() : base() { }

        internal Terrain(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Left = new TerrainSide();
            Right = new TerrainSide();
            Railings = new Railings();
            StepSize = StepSize.Meters4;
            VegetationSpheres = [];
        }

        /// <summary>
        /// Adds a single terrain item to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="backwardPos">The position of the backward (red) node.</param>
        /// <param name="forwardPos">The position of the forward (green) node.</param>
        /// <param name="material">The unit name of the terrain material.</param>
        /// <param name="leftSize">The terrain size on the left side.</param>        
        /// <param name="rightSize">The terrain size on the right side.</param>
        /// <returns>The newly created terrain item.</returns>
        public static Terrain Add(IItemContainer map, Vector3 backwardPos, Vector3 forwardPos, Token material,
            float leftSize, float rightSize)
        {
            var terrain = Add<Terrain>(map, backwardPos, forwardPos);
            terrain.InitFromAddOrAppend(backwardPos, forwardPos, material, leftSize, rightSize);
            return terrain;
        }

        /// <summary>
        /// Appends a terrain item to this terrain.
        /// </summary>
        /// <param name="position">The position of the forward node of the new terrain item.</param>
        /// <param name="material">The unit name of the terrain material.</param>
        /// <param name="leftSize">The terrain size on the left side.</param>        
        /// <param name="rightSize">The terrain size on the right side.</param>
        /// <returns>The newly created terrain item.</returns>
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
                side.Terrain.QuadData.BrushMaterials = [new(material)];
                side.Terrain.CalculateQuadGrid(StepSize, Length);
            }
        }

        /// <inheritdoc/>
        public override void Recalculate()
        {
            base.Recalculate();
            Left.Terrain.CalculateQuadGrid(StepSize, Length);
            Right.Terrain.CalculateQuadGrid(StepSize, Length);
        }
    }
}
