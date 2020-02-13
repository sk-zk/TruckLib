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
    public class Road : PolylineItem, IDataPart
    {
        // TODO: Use the new KdopItem system.
        // not sure how to implement it while keeping the flexibility
        // of Left/Right objects, RoadTerrain class, Clone() methods etc.

        public override ItemType ItemType => ItemType.Road;

        public override ItemFile DefaultItemFile => ItemFile.Base;

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
        public RoadSide Left { get; set; } = new RoadSide();

        /// <summary>
        /// Terrain, models, railings, sidewalk on the right side of this road.
        /// </summary>
        public RoadSide Right { get; set; } = new RoadSide();

        /// <summary>
        /// The terrain material used in the center of the road.
        /// </summary>
        public Token CenterMaterial { get; set; } = "0";

        /// <summary>
        /// The vegetation used in the center of the road.
        /// </summary>
        public CenterVegetation CenterVegetation { get; set; } = new CenterVegetation();

        public Color CenterMaterialColor { get; set; } = Color.FromArgb(0xffffff);

        /// <summary>
        /// The seed which determines the placement of vegetation models.
        /// </summary>
        public uint RandomSeed { get; set; } = 0;

        public Token OverlayScheme { get; set; }

        /// <summary>
        /// The vegetation spheres on this road.
        /// </summary>
        public List<VegetationSphere> VegetationSpheres { get; set; } 
            = new List<VegetationSphere>();

        /// <summary>
        /// The segment step size.
        /// </summary>
        // set to HighPoly by default since you probably aren't creating
        // non-template roads, so the road is HighPoly no matter what, 
        // but this setting is needed to make the terrain match the road.
        public RoadResolution Resolution { get; set; } = RoadResolution.HighPoly;

        /// <summary>
        /// [Legacy roads only] 
        /// Determines if this road is a city road and can therefore have a sidewalk.
        /// </summary>
        public bool IsCityRoad = false;

        public byte DlcGuard = Lookup.DlcGuard.None;

        /// <summary>
        /// Causes the navigation to apply a huge penalty to paths using this road.
        /// </summary>
        public bool GpsAvoid = false;

        /// <summary>
        /// Determines if this road is displayed in the UI map.
        /// </summary>
        public bool HideInUiMap = false;

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes = false;

        /// <summary>
        /// Determines if the item uses left hand traffic.
        /// </summary>
        public bool LeftHandTraffic = false;

        /// <summary>
        /// Determines if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation = false;

        /// <summary>
        /// Determines if AI traffic can use this road.
        /// <para>If not, AI vehicles will choose a different route.
        /// If there isn't one, they will despawn instead.</para>
        /// </summary>
        public bool NoAiVehicles = false;

        /// <summary>
        /// Determines if the road has invisible walls on both sides of it.
        /// </summary>
        public bool NoBoundary = false;

        /// <summary>
        /// Determines if the player can collide with this item.
        /// </summary>
        public bool NoCollision = false;

        public bool NoTerrainShadows = false;

        public bool SmoothDetailVegetation = true;

        public bool StretchTerrain = false;

        /// <summary>
        /// Determines if the item is reflected in water.
        /// </summary>
        public bool WaterReflection = false;

        public Road() { }
        
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
            r.CenterVegetation = CenterVegetation;
            r.Material = Material;
            r.OverlayScheme = OverlayScheme;
            r.RandomSeed = RandomSeed;
            r.Resolution = Resolution;
            r.VegetationSpheres = new List<VegetationSphere>(VegetationSpheres);
            r.ViewDistance = ViewDistance;

            r.GpsAvoid = GpsAvoid;
            r.HideInUiMap = HideInUiMap;
            r.IgnoreCutPlanes = IgnoreCutPlanes;
            r.IsCityRoad = IsCityRoad;
            r.LeftHandTraffic = LeftHandTraffic;
            r.LowPolyVegetation = LowPolyVegetation;
            r.NoAiVehicles = NoAiVehicles;
            r.NoBoundary = NoBoundary;
            r.NoCollision = NoCollision;
            r.NoTerrainShadows = NoTerrainShadows;
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
                side.Terrain.QuadData.BrushMaterials = new List<Token> { "0" };
                side.Terrain.CalculateQuadGrid(Resolution, Length);                
            }
        }

        private const float railingOffsetFactor = 100f;
        private const float modelOffsetFactor = 100f;
        private const float modelDistanceFactor = 100f;
        private const float heightOffsetFactor = 100f;
        private const float terrainSizeFactor = 10f;
        private const float centerVegDensityFactor = 10f;
        private const float centerVegOffsetFactor = 10f;
        private const float vegFromToFactor = 10f;
        private const int viewDistFactor = 10;

        /// <summary>
        /// Reads the .base component of this road.
        /// </summary>
        /// <param name="r">The reader.</param>
        public override void ReadFromStream(BinaryReader r)
        {
            Uid = r.ReadUInt64();
            BoundingBox.ReadFromStream(r);

            // === kdop flags ===
            var kflag1 = r.ReadByte();
            Left.Terrain.Noise = (TerrainNoise)(kflag1 & 0b11);
            Right.Terrain.Noise = (TerrainNoise)((kflag1 >> 2) & 0b11);
            Left.Terrain.Transition = (TerrainTransition)((kflag1 >> 4) & 0b11);
            Right.Terrain.Transition = (TerrainTransition)((kflag1 >> 6) & 0b11);

            var kflag2 = r.ReadByte();
            var karr2 = new BitArray(new[] { kflag2 });
            Left.Sidewalk.Size = (SidewalkSize)(kflag2 & 0b11);
            Right.Sidewalk.Size = (SidewalkSize)((kflag2 >> 2) & 0b11);
            Right.VegetationCollision = karr2[4];
            Left.VegetationCollision = karr2[5];
            WaterReflection = karr2[6];
            LeftHandTraffic = karr2[7];

            var kflag3 = r.ReadByte();
            var karr3 = new BitArray(new[] { kflag3 });
            Right.Railings.InvertRailing = karr3[1];
            Left.Railings.InvertRailing = karr3[2];
            IsCityRoad = karr3[3];
            Right.Models[0].Shift = karr3[4];
            Left.Models[0].Shift = karr3[5];
            NoAiVehicles = karr3[6];

            var kflag4 = r.ReadByte();
            var karr4 = new BitArray(new[] { kflag4 });
            var highPolyRoad = karr4[0];
            Resolution = highPolyRoad ? RoadResolution.HighPoly : RoadResolution.Normal;
            HideInUiMap = karr4[1];
            NoCollision = karr4[2];
            NoBoundary = karr4[3];
            Right.NoDetailVegetation = karr4[4];
            Left.NoDetailVegetation = karr4[5];
            StretchTerrain = karr4[6];

            // View distance
            ViewDistance = (ushort)(r.ReadByte() * viewDistFactor);

            // === road_flags ===
            var rflag1 = r.ReadByte();
            var rarr1 = new BitArray(new[] { rflag1 });
            LowPolyVegetation = rarr1[0];
            var superfine = rarr1[1];
            if (superfine) Resolution = RoadResolution.Superfine;
            IgnoreCutPlanes = rarr1[2];
            Right.Models[1].Shift = rarr1[5];
            Left.Models[1].Shift = rarr1[6];
            NoTerrainShadows = rarr1[7];

            // DLC guard
            DlcGuard = r.ReadByte();

            var rflag3 = r.ReadByte();
            var rarr3 = new BitArray(new[] { rflag3 });
            SmoothDetailVegetation = rarr3[1];
            Right.Models[0].Flip = rarr3[2];
            Right.Models[1].Flip = rarr3[3];
            Left.Models[0].Flip = rarr3[4];
            Left.Models[1].Flip = rarr3[5];
            Left.ShoulderBlocked = rarr3[6];
            Right.ShoulderBlocked = rarr3[7];

            var rflag4 = r.ReadByte();
            var rarr4 = new BitArray(new[] { rflag4 });
            Left.Railings.DoubleSided = rarr4[0];
            Right.Railings.DoubleSided = rarr4[1];
            Left.Railings.CenterPartOnly = rarr4[2];
            Right.Railings.CenterPartOnly = rarr4[3];
            GpsAvoid = rarr4[4];

            RoadType = r.ReadToken();

            Right.Variant = r.ReadToken();
            Left.Variant = r.ReadToken();

            Right.RightEdge = r.ReadToken();
            Right.LeftEdge = r.ReadToken();
            Left.RightEdge = r.ReadToken();
            Left.LeftEdge = r.ReadToken();

            Right.Terrain.Profile = r.ReadToken();
            Right.Terrain.Coefficient = r.ReadSingle();
            Left.Terrain.Profile = r.ReadToken();
            Left.Terrain.Coefficient = r.ReadSingle();

            Right.Look = r.ReadToken();
            Left.Look = r.ReadToken();

            Material = r.ReadToken();

            // 3 railings per side
            for (int i = 0; i < 3; i++)
            {
                Right.Railings.Models[i].Model = r.ReadToken();
                Right.Railings.Models[i].Offset = r.ReadInt16() / railingOffsetFactor;
                Left.Railings.Models[i].Model = r.ReadToken();
                Left.Railings.Models[i].Offset = r.ReadInt16() / railingOffsetFactor;
            }

            Right.RoadHeightOffset = r.ReadInt32() / heightOffsetFactor;
            Left.RoadHeightOffset = r.ReadInt32() / heightOffsetFactor;

            Node = new UnresolvedNode(r.ReadUInt64());
            ForwardNode = new UnresolvedNode(r.ReadUInt64());

            Length = r.ReadSingle();
        }

        /// <summary>
        /// Reads the .data component of this road.
        /// </summary>
        /// <param name="r">The reader.</param>
        public void ReadDataPart(BinaryReader r)
        {
            // Overlay Scheme
            // TODO: What is this?
            OverlayScheme = r.ReadToken();

            foreach (var side in new[] { Right, Left }) // repeated for both sides of the road
            {
                // Model data
                foreach (var model in side.Models)
                {
                    model.Name = r.ReadToken();
                    model.Offset = r.ReadInt16() / modelOffsetFactor;
                    model.Distance = r.ReadUInt16() / modelDistanceFactor;
                }

                side.Terrain.Size = r.ReadUInt16() / terrainSizeFactor;

                // Vegetation
                foreach (var veg in side.Vegetation)
                {
                    veg.ReadFromStream(r);
                }

                side.Sidewalk.Material = r.ReadToken();
                side.Terrain.QuadData.ReadFromStream(r);
            }

            CenterMaterial = r.ReadToken();       
            CenterMaterialColor = r.ReadColor();

            RandomSeed = r.ReadUInt32();
            CenterVegetation.Name = r.ReadToken();
            CenterVegetation.Density = r.ReadUInt16() / centerVegDensityFactor;
            CenterVegetation.Scale = (VegetationScale)r.ReadByte();
            CenterVegetation.Offset = r.ReadByte() / centerVegOffsetFactor;

            Left.NoDetailVegetationFrom = r.ReadUInt16() / vegFromToFactor;
            Right.NoDetailVegetationFrom = r.ReadUInt16() / vegFromToFactor;
            Left.NoDetailVegetationTo = r.ReadUInt16() / vegFromToFactor;
            Right.NoDetailVegetationTo = r.ReadUInt16() / vegFromToFactor;

            // veg. spheres
            var vegSphereCount = r.ReadUInt32();
            for (int i = 0; i < vegSphereCount; i++)
            {
                var sphere = new VegetationSphere();
                sphere.ReadFromStream(r);
                VegetationSpheres.Add(sphere);
            }

            // Additional parts
            var leftAdditionalPartsCount = r.ReadUInt32();
            for (int i = 0; i < leftAdditionalPartsCount; i++)
            {
                Left.AdditionalParts.Add(r.ReadToken());
            }

            var rightAdditionalPartsCount = r.ReadUInt32();
            for (int i = 0; i < rightAdditionalPartsCount; i++)
            {
                Right.AdditionalParts.Add(r.ReadToken());
            }

            Right.UVRotation = r.ReadSingle();
            Left.UVRotation = r.ReadSingle();
        }

        /// <summary>
        /// Writes the .base part of this road.
        /// </summary>
        /// <param name="w"></param>
        public override void WriteToStream(BinaryWriter w)
        {
            w.Write(Uid);
            BoundingBox.WriteToStream(w);

            // === kdop flags ===
            byte kflag1 = 0;
            kflag1 |= (byte)Left.Terrain.Noise;
            kflag1 |= (byte)((byte)Right.Terrain.Noise << 2);
            kflag1 |= (byte)((byte)Left.Terrain.Transition << 4);
            kflag1 |= (byte)((byte)Right.Terrain.Transition << 6);
            w.Write(kflag1);

            byte kflag2 = 0;
            kflag2 |= (byte)Left.Sidewalk.Size;
            kflag2 |= (byte)((byte)Right.Sidewalk.Size << 2);
            kflag2 |= (byte)(Right.VegetationCollision.ToByte() << 4);
            kflag2 |= (byte)(Left.VegetationCollision.ToByte() << 5);
            kflag2 |= (byte)(WaterReflection.ToByte() << 6);
            kflag2 |= (byte)(LeftHandTraffic.ToByte() << 7);
            w.Write(kflag2);

            byte kflag3 = 0;
            kflag3 |= (byte)(Right.Railings.InvertRailing.ToByte() << 1);
            kflag3 |= (byte)(Left.Railings.InvertRailing.ToByte() << 2);
            kflag3 |= (byte)(IsCityRoad.ToByte() << 3);
            kflag3 |= (byte)(Right.Models[0].Shift.ToByte() << 4);
            kflag3 |= (byte)(Left.Models[0].Shift.ToByte() << 5);
            kflag3 |= (byte)(NoAiVehicles.ToByte() << 6);
            w.Write(kflag3);

            byte kflag4 = 0;
            kflag4 |= (Resolution == RoadResolution.HighPoly).ToByte();
            kflag4 |= (byte)(HideInUiMap.ToByte() << 1);
            kflag4 |= (byte)(NoCollision.ToByte() << 2);
            kflag4 |= (byte)(NoBoundary.ToByte() << 3);
            kflag4 |= (byte)(Right.NoDetailVegetation.ToByte() << 4);
            kflag4 |= (byte)(Left.NoDetailVegetation.ToByte() << 5);
            kflag4 |= (byte)(StretchTerrain.ToByte() << 6);
            w.Write(kflag4);

            // View distance
            w.Write((byte)(ViewDistance / viewDistFactor));

            // === road_flags ===
            byte rflag1 = 0;
            rflag1 |= LowPolyVegetation.ToByte();
            rflag1 |= (byte)((Resolution == RoadResolution.Superfine).ToByte() << 1);
            rflag1 |= (byte)(IgnoreCutPlanes.ToByte() << 2);
            rflag1 |= (byte)(Right.Models[1].Shift.ToByte() << 5);
            rflag1 |= (byte)(Left.Models[1].Shift.ToByte() << 6);
            rflag1 |= (byte)(NoTerrainShadows.ToByte() << 7);
            w.Write(rflag1);

            // DLC guard
            w.Write(DlcGuard);

            byte rflag3 = 0;
            rflag3 |= (byte)(SmoothDetailVegetation.ToByte() << 1);
            rflag3 |= (byte)(Right.Models[0].Flip.ToByte() << 2);
            rflag3 |= (byte)(Right.Models[1].Flip.ToByte() << 3);
            rflag3 |= (byte)(Left.Models[0].Flip.ToByte() << 4);
            rflag3 |= (byte)(Left.Models[1].Flip.ToByte() << 5);
            rflag3 |= (byte)(Left.ShoulderBlocked.ToByte() << 6);
            rflag3 |= (byte)(Right.ShoulderBlocked.ToByte() << 7);
            w.Write(rflag3);

            byte rflag4 = 0;
            rflag4 |= Left.Railings.DoubleSided.ToByte();
            rflag4 |= (byte)(Right.Railings.DoubleSided.ToByte() << 1);
            rflag4 |= (byte)(Left.Railings.CenterPartOnly.ToByte() << 2);
            rflag4 |= (byte)(Right.Railings.CenterPartOnly.ToByte() << 3);
            rflag4 |= (byte)(GpsAvoid.ToByte() << 4);
            w.Write(rflag4);

            w.Write(RoadType);

            w.Write(Right.Variant);
            w.Write(Left.Variant);

            w.Write(Right.RightEdge);
            w.Write(Right.LeftEdge);
            w.Write(Left.RightEdge);
            w.Write(Left.LeftEdge);

            w.Write(Right.Terrain.Profile);
            w.Write(Right.Terrain.Coefficient);
            w.Write(Left.Terrain.Profile);
            w.Write(Left.Terrain.Coefficient);

            w.Write(Right.Look);
            w.Write(Left.Look);

            w.Write(Material);

            // 3 railings per side
            for (int i = 0; i < 3; i++)
            {
                w.Write(Right.Railings.Models[i].Model);
                w.Write((short)(Right.Railings.Models[i].Offset * railingOffsetFactor));
                w.Write(Left.Railings.Models[i].Model);
                w.Write((short)(Left.Railings.Models[i].Offset * railingOffsetFactor));
            }

            w.Write((int)(Right.RoadHeightOffset * heightOffsetFactor));
            w.Write((int)(Left.RoadHeightOffset * heightOffsetFactor));

            w.Write(Node?.Uid ?? 0UL);
            w.Write(ForwardNode?.Uid ?? 0UL);

            w.Write(Length);
        }

        /// <summary>
        /// Writes the .data part of this road.
        /// </summary>
        /// <param name="w"></param>
        public void WriteDataPart(BinaryWriter w)
        {
            // overlay scheme
            w.Write(OverlayScheme);

            foreach (var side in new[] { Right, Left }) // repeated for both sides of the road
            {
                // Model data
                foreach (var model in side.Models)
                {
                    w.Write(model.Name);
                    w.Write((short)(model.Offset * modelOffsetFactor));
                    w.Write((short)(model.Distance * modelDistanceFactor));
                }

                w.Write((ushort)(side.Terrain.Size * terrainSizeFactor));

                // Vegetation
                foreach (var veg in side.Vegetation)
                {
                    veg.WriteToStream(w);
                }

                w.Write(side.Sidewalk.Material);
                side.Terrain.QuadData.WriteToStream(w);
            }

            w.Write(CenterMaterial);
            w.Write(CenterMaterialColor);
            w.Write(RandomSeed);
            w.Write(CenterVegetation.Name);
            w.Write((ushort)(CenterVegetation.Density * centerVegDensityFactor));
            w.Write((byte)CenterVegetation.Scale);
            w.Write((byte)(CenterVegetation.Offset * centerVegOffsetFactor));

            w.Write((ushort)(Left.NoDetailVegetationFrom * vegFromToFactor));
            w.Write((ushort)(Right.NoDetailVegetationFrom * vegFromToFactor));
            w.Write((ushort)(Left.NoDetailVegetationTo * vegFromToFactor));
            w.Write((ushort)(Right.NoDetailVegetationTo * vegFromToFactor));

            // veg. spheres
            w.Write(VegetationSpheres.Count);
            foreach (var sphere in VegetationSpheres)
            {
                sphere.WriteToStream(w);
            }

            // additional parts
            w.Write(Left.AdditionalParts.Count);
            foreach (var part in Left.AdditionalParts)
            {
                w.Write(part);
            }
            w.Write(Right.AdditionalParts.Count);
            foreach (var part in Right.AdditionalParts)
            {
                w.Write(part);
            }

            w.Write(Right.UVRotation);
            w.Write(Left.UVRotation);
        }

    }
}
