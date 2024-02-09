using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A road segment.
    /// </summary>
    public class Road : PolylineItem
    {
        // TODO: Use the KdopItem system.
        // not sure how to implement it while keeping the flexibility
        // of Left/Right objects, RoadTerrain class, Clone() methods etc.

        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Road;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
        internal override bool HasDataPayload => true;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Returns the minimum length of a road segment with the current resolution setting.
        /// </summary>
        public float MinLength => Resolution switch
        {
            RoadResolution.Superfine => 0.40000001f,
            RoadResolution.HighPoly => 1.25f,
            RoadResolution.Normal => 3.75f,
            _ => 3.75f,
        };

        /// <summary>
        /// The maximum length of a road segment.
        /// </summary>
        public static readonly float MaxLength = 1000f;

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The unit name of the road type.
        /// </summary>
        public Token RoadType { get; set; }

        /// <summary>
        /// The unit name of the road material for legacy, pre-template roads.
        /// </summary>
        public Token Material { get; set; }

        /// <summary>
        /// Terrain, models, railings, sidewalk on the left side of this road.
        /// </summary>
        public RoadSide Left { get; set; }

        /// <summary>
        /// Terrain, models, railings, sidewalk on the right side of this road.
        /// </summary>
        public RoadSide Right { get; set; }

        /// <summary>
        /// Unit name of the terrain material used in the center of dual carriageways.
        /// </summary>
        public Token CenterMaterial { get; set; }

        /// <summary>
        /// Vegetation in the center of dual carriageways.
        /// </summary>
        public CenterVegetation CenterVegetation { get; set; } 

        /// <summary>
        /// Color tint of the material used in the center of dual carriageways.
        /// </summary>
        public Color CenterMaterialColor { get; set; }

        /// <summary>
        /// UV rotation of the material in the center of dual carriageways.
        /// </summary>
        public ushort CenterMaterialRotation { get; set; }

        /// <summary>
        /// Gets or sets if detail vegetation is used in the center of dual carriageways.
        /// </summary>
        public bool CenterDetailVegetation { get; set; }

        /// <summary>
        /// The seed for the RNG which determines which vegetation models
        /// to place. The position of the models does not appear to be
        /// affected by this.
        /// </summary>
        public uint RandomSeed { get; set; }

        /// <summary>
        /// Unit name of the overlay texture.
        /// </summary>
        public Token Overlay { get; set; }

        /// <summary>
        /// Vegetation spheres on this road.
        /// </summary>
        public List<VegetationSphere> VegetationSpheres { get; set; }

        /// <summary>
        /// For pre-template roads, this sets the quad step size of the road itself.
        /// For template roads, only <see cref="RoadResolution.Normal">Normal</see> and
        /// <see cref="RoadResolution.HighPoly">HighPoly</see> are supported, and it only affects
        /// the step size of terrain quads.
        /// </summary>
        public RoadResolution Resolution { get; set; }

        /// <summary>
        /// Legacy only. Gets or sets whether a pre-template road is a city road and
        /// can therefore have a sidewalk.
        /// </summary>
        [Obsolete]
        public bool IsCityRoad { get; set; } = false;

        public byte DlcGuard { get; set; }

        /// <summary>
        /// Gets or sets if the satnav should avoid this road segment.
        /// </summary>
        public bool GpsAvoid { get; set; } = false;

        /// <summary>
        /// Gets or sets if this road is displayed in the UI map.
        /// </summary>
        public bool ShowInUiMap { get; set; } = true;

        /// <summary>
        /// Gets or sets if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes { get; set; } = false;

        /// <summary>
        /// Gets or sets if the item uses left hand traffic.
        /// </summary>
        public bool LeftHandTraffic { get; set; } = false;

        /// <summary>
        /// Gets or sets if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation { get; set; } = false;

        /// <summary>
        /// Gets or sets if AI traffic can use this road.
        /// If not, AI vehicles will choose a different route.
        /// If there isn't one, they will despawn instead.
        /// </summary>
        public bool AiVehicles { get; set; } = true;

        /// <summary>
        /// Gets or sets if the road has invisible walls on both sides of it.
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
        public bool SmoothDetailVegetation { get; set; } = true;

        [Obsolete]
        public bool StretchTerrain { get; set; } = false;

        /// <summary>
        /// Gets or sets if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection { get; set; } = false;

        /// <summary>
        /// Gets or sets if this road is only visible in the UI map once discovered.
        /// </summary>
        public bool Secret { get; set; } = false;

        public bool Unknown { get; set; } = false;
        public bool Unknown2 { get; set; } = false;
        public bool Unknown3 { get; set; } = false;

        public Road() : base() { }

        internal Road(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Left = new RoadSide();
            Right = new RoadSide();
            Resolution = RoadResolution.HighPoly;
            CenterMaterial = "0";
            CenterMaterialColor = Color.FromArgb(0xffffff);
            CenterVegetation = new CenterVegetation();
            CenterDetailVegetation = true;
            VegetationSpheres = new List<VegetationSphere>();
            // set to HighPoly by default since you probably aren't creating
            // non-template roads, so the road is HighPoly no matter what, 
            // but this setting is needed to make the terrain match the road.
        }

        /// <summary>
        /// Adds a road segment to the map.
        /// </summary>
        /// <param name="map">The map .</param>
        /// <param name="backwardPos">The position of the backward (red) node.</param>
        /// <param name="forwardPos">The position of the forward (green) node.</param>
        /// <param name="type">The unit name of the road.</param>
        /// <param name="leftTerrainSize">The terrain size on the left side.</param>
        /// <param name="rightTerrainSize">The terrain size on the right side.</param>
        /// <returns>The newly created road segment.</returns>
        public static Road Add(IItemContainer map, Vector3 backwardPos, Vector3 forwardPos,
            Token type, float leftTerrainSize = 0f, float rightTerrainSize = 0f)
        {
            var road = Add<Road>(map, backwardPos, forwardPos);
            road.InitFromAddOrAppend(backwardPos, forwardPos, type, leftTerrainSize, rightTerrainSize);
            return road;
        }

        /// <summary>
        /// Appends a road segment to this road. 
        /// </summary>
        /// <param name="position">The position of the ForwardNode of the new road.</param>
        /// <param name="cloneSettings">Whether the new segment should have the same settings as this one.</param>
        /// <returns>The newly created road segment.</returns>
        public Road Append(Vector3 position, bool cloneSettings = true)
        {
            if (!cloneSettings)
            {
                return Append(position, RoadType);
            }

            var r = Append(position, RoadType, Left.Terrain.Size, Right.Terrain.Size);
            CopySettingsTo(r);
            return r;
        }

        private void CopySettingsTo(Road r)
        {
            // don't forget to remove all the flags after the kdopitem migration

            r.Left = Left.Clone();
            r.Right = Right.Clone();
            r.CenterMaterial = CenterMaterial;
            r.CenterMaterialColor = CenterMaterialColor;
            r.CenterVegetation = CenterVegetation.Clone();
            r.Material = Material;
            r.Overlay = Overlay;
            r.RandomSeed = RandomSeed;
            r.Resolution = Resolution;
            r.VegetationSpheres = new List<VegetationSphere>(VegetationSpheres);
            r.ViewDistance = ViewDistance;

            r.GpsAvoid = GpsAvoid;
            r.ShowInUiMap = ShowInUiMap;
            r.IgnoreCutPlanes = IgnoreCutPlanes;
            r.IsCityRoad = IsCityRoad;
            r.LeftHandTraffic = LeftHandTraffic;
            r.LowPolyVegetation = LowPolyVegetation;
            r.AiVehicles = AiVehicles;
            r.Boundary = Boundary;
            r.Collision = Collision;
            r.TerrainShadows = TerrainShadows;
            r.SmoothDetailVegetation = SmoothDetailVegetation;
            r.StretchTerrain = StretchTerrain;
            r.WaterReflection = WaterReflection;
        }

        /// <summary>
        /// Appends a road segment to this road. 
        /// </summary>
        /// <param name="position">The position of the ForwardNode of the new road.</param>
        /// <param name="type">The unit name of the road.</param>
        /// <param name="leftTerrainSize">The terrain size on the left side.</param>
        /// <param name="rightTerrainSize">The terrain size on the right side.</param>
        /// <returns>The newly created road.</returns>
        public Road Append(Vector3 position, Token type, float leftTerrainSize = 0f, 
            float rightTerrainSize = 0f)
        {
            var road = Append<Road>(position);
            road.InitFromAddOrAppend(ForwardNode.Position, position, type, leftTerrainSize, rightTerrainSize);
            road.Material = Material;
            return road;
        }

        /// <summary>
        /// Prepends a road segment to this road.
        /// </summary>
        /// <param name="position">The position of the backward node of the new road.</param>
        /// <returns>The newly created road.</returns>
        public Road Prepend(Vector3 position)
        {
            return Prepend(position, RoadType);
        }

        /// <summary>
        /// Prepends a road segment to this road.
        /// </summary>
        /// <param name="position">The position of the backward node of the new road.</param>
        /// <param name="type">The unit name of the road.</param>
        /// <param name="leftTerrainSize">The terrain size on the left side.</param>
        /// <param name="rightTerrainSize">The terrain size on the right side.</param>
        /// <returns>The newly created road.</returns>
        public Road Prepend(Vector3 position, Token type, float leftTerrainSize = 0f,
            float rightTerrainSize = 0f)
        {
            var road = Prepend<Road>(position);
            road.InitFromAddOrAppend(position, Node.Position, type, leftTerrainSize, rightTerrainSize);
            road.Material = Material;
            return road;
        }

        /// <summary>
        /// Initializes a road which has been created via Add or Append.
        /// </summary>
        internal void InitFromAddOrAppend(Vector3 backwardPos, Vector3 forwardPos,
            Token type, float leftTerrainSize = 0f, float rightTerrainSize = 0f)
        {
            RoadType = type;
            Length = Vector3.Distance(backwardPos, forwardPos);

            Left.Terrain.Size = leftTerrainSize;
            Right.Terrain.Size = rightTerrainSize;
            foreach (var side in new[] { Left, Right })
            {
                side.Terrain.QuadData.BrushMaterials = new List<Material> { new Material("0") };
                side.Terrain.CalculateQuadGrid(Resolution, Length);
            }
        }

        /// <inheritdoc/>
        public override void Recalculate()
        {
            base.Recalculate();
            Left.Terrain.CalculateQuadGrid(Resolution, Length);
            Right.Terrain.CalculateQuadGrid(Resolution, Length);
        }      
    }
}
