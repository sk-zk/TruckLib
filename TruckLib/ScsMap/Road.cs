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
        // TODO: Use the new KdopItem system.
        // not sure how to implement it while keeping the flexibility
        // of Left/Right objects, RoadTerrain class, Clone() methods etc.

        public override ItemType ItemType => ItemType.Road;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        internal override bool HasDataPayload => true;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public float MinLength => Resolution switch
        {
            RoadResolution.Superfine => 0.40000001f,
            RoadResolution.HighPoly => 1.25f,
            RoadResolution.Normal => 3.75f,
            _ => 3.75f,
        };

        public float MaxLength => 1000f;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The unit name of the road.
        /// </summary>
        public Token RoadType { get; set; }

        /// <summary>
        /// The road material.
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
        /// The terrain material used in the center of the road.
        /// </summary>
        public Token CenterMaterial { get; set; }

        /// <summary>
        /// The vegetation used in the center of the road.
        /// </summary>
        public CenterVegetation CenterVegetation { get; set; } 

        public Color CenterMaterialColor { get; set; }

        /// <summary>
        /// The seed which determines the placement of vegetation models.
        /// </summary>
        public uint RandomSeed { get; set; }

        public Token OverlayScheme { get; set; }

        /// <summary>
        /// The vegetation spheres on this road.
        /// </summary>
        public List<VegetationSphere> VegetationSpheres { get; set; } 

        /// <summary>
        /// The segment step size.
        /// </summary>
        public RoadResolution Resolution { get; set; }

        /// <summary>
        /// [Legacy roads only]<br/>
        /// <para>Makes this road a city road and can therefore have a sidewalk.
        /// </para>
        /// </summary>
        [Obsolete]
        public bool IsCityRoad = false;

        public byte DlcGuard;

        /// <summary>
        /// Gets or sets if the satnav should avoid this road segment.
        /// </summary>
        public bool GpsAvoid = false;

        /// <summary>
        /// Gets or sets if this road is displayed in the UI map.
        /// </summary>
        public bool ShowInUiMap = true;

        /// <summary>
        /// Gets or sets if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes = false;

        /// <summary>
        /// Gets or sets if the item uses left hand traffic.
        /// </summary>
        public bool LeftHandTraffic = false;

        /// <summary>
        /// Gets or sets if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation = false;

        /// <summary>
        /// Gets or sets if AI traffic can use this road.
        /// <para>If not, AI vehicles will choose a different route.
        /// If there isn't one, they will despawn instead.</para>
        /// </summary>
        public bool AiVehicles = true;

        /// <summary>
        /// Gets or sets if the road has invisible walls on both sides of it.
        /// </summary>
        public bool Boundary = true;

        /// <summary>
        /// Gets or sets if collision is enabled.
        /// </summary>
        public bool Collision = true;

        public bool TerrainShadows = true;

        /// <summary>
        /// Gets or sets if detail vegetation transitions smoothly in places
        /// where it is affected by brushes.
        /// </summary>
        public bool SmoothDetailVegetation = true;

        [Obsolete]
        public bool StretchTerrain = false;

        /// <summary>
        /// Gets or sets if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection = false;

        public Road() : base() { }

        internal Road(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Left = new RoadSide();
            Right = new RoadSide();
            Resolution = RoadResolution.HighPoly;
            CenterMaterial = "0";
            CenterMaterialColor = Color.FromArgb(0xffffff);
            CenterVegetation = new CenterVegetation();
            VegetationSpheres = new List<VegetationSphere>();
            // set to HighPoly by default since you probably aren't creating
            // non-template roads, so the road is HighPoly no matter what, 
            // but this setting is needed to make the terrain match the road.
        }

        /// <summary>
        /// Adds a single road segment to the map.
        /// </summary>
        /// <param name="map">The map the road will be added to.</param>
        /// <param name="backwardPos">The position of the backward (red) node.</param>
        /// <param name="forwardPos">The position of the forward (green) node.</param>
        /// <param name="type">Unit name of the road.</param>
        /// <param name="leftTerrainSize">Terrain size on the left side.</param>
        /// <param name="rightTerrainSize">Terrain size on the right side.</param>
        /// <returns>The new road.</returns>
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
        /// <param name="position">The position of the ForwardNode of the new building.</param>
        /// <returns>The new road.</returns>
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
            r.OverlayScheme = OverlayScheme;
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
        /// <param name="type">The unit name.</param>
        /// <param name="leftTerrainSize">Terrain size on the left side.</param>
        /// <param name="rightTerrainSize">Terrain size on the right side.</param>
        /// <returns>The new road.</returns>
        public Road Append(Vector3 position, Token type, float leftTerrainSize = 0f, 
            float rightTerrainSize = 0f)
        {
            var road = Append<Road>(position);
            road.InitFromAddOrAppend(ForwardNode.Position, position, type, leftTerrainSize, rightTerrainSize);
            road.Material = Material;
            return road;
        }

        public Road Prepend(Vector3 position)
        {
            return Prepend(position, RoadType);
        }

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

        public override void Recalculate()
        {
            base.Recalculate();
            Left.Terrain.CalculateQuadGrid(Resolution, Length);
            Right.Terrain.CalculateQuadGrid(Resolution, Length);
        }      
    }
}
