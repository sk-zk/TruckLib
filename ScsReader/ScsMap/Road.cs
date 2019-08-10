using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// A road segment.
    /// </summary>
    public class Road : PolylineItem, IDataPart
    {
        // TODO: Use the new KdopItem system
        // not sure how to implement it while keeping Left/Right items

        public override ItemType ItemType => ItemType.Road;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

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
        // set to HighPoly by default since you probably aren't creating non-template roads,
        // so the road is HighPoly no matter what, but this setting is needed regardless to
        // make the terrain match the road.
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

        public Road()
        {
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
                side.Terrain.QuadData.Quads = new TerrainQuad[side.Terrain.QuadData.Cols
                    * side.Terrain.QuadData.Rows].Select(h => new TerrainQuad()).ToList();
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


        /// <summary>
        /// Reads the .base component of this road.
        /// </summary>
        /// <param name="r">The reader.</param>
        public override void ReadFromStream(BinaryReader r)
        {
            Uid = r.ReadUInt64();
            BoundingBox.ReadFromStream(r);

            // 1+2: Right terrain transition; 3+4 Left terrain transition
            // 5+6: Right terrain noise; 7+8: Left terrain noise
            var flags1 = r.ReadByte();
            Right.Terrain.Transition = (TerrainTransition)((flags1 >> 6) & 0b11);
            Left.Terrain.Transition = (TerrainTransition)((flags1 >> 4) & 0b11);
            Right.Terrain.Noise = (TerrainNoise)((flags1 >> 2) & 0b11);
            Left.Terrain.Noise = (TerrainNoise)(flags1 & 0b11);

            // 1: Left hand traffic; 2: Water reflection;
            // 3: Left vegetation collision; 4: Right vegetation collision;
            // 5+6: Right sidewalk size; 7+8: Left sidewalk size
            var flags2 = r.ReadByte();
            var bitArr2 = new BitArray(new byte[] { flags2 });
            LeftHandTraffic = bitArr2[8 - 1];
            WaterReflection = bitArr2[8 - 2];
            Left.VegetationCollision = bitArr2[8 - 3];
            Right.VegetationCollision = bitArr2[8 - 4];
            Right.Sidewalk.Size = (SidewalkSize)((flags2 >> 2) & 0b11);
            Left.Sidewalk.Size = (SidewalkSize)(flags2 & 0b11);

            // 1: Unknown; 2: No AI Vehicles;
            // 3: Left Model1 Shift; 4: Right Model1 Shift;
            // 5: Is City Road; 6: Left Invert Railing;
            // 7: Right Invert Railing; 8: Unknown
            var flags3 = r.ReadByte();
            var bitArr3 = new BitArray(new byte[] { flags3 });
            NoAiVehicles = bitArr3[8 - 2];
            Left.Models[0].Shift = bitArr3[8 - 3];
            Right.Models[0].Shift = bitArr3[8 - 4];
            IsCityRoad = bitArr3[8 - 5];
            Left.Railings.InvertRailing = bitArr3[8 - 6];
            Right.Railings.InvertRailing = bitArr3[8 - 7];

            // 1: Unknown; 2: Stretch terrain; 
            // 3: Left No detail vegetation; 4: Right no detail vegetation;
            // 5: No boundary; 6: No collision;
            // 7: Hide in UI map; 8: High-poly road
            var flags4 = r.ReadByte();
            var bitArr4 = new BitArray(new byte[] { flags4 });
            StretchTerrain = bitArr4[8 - 2];
            Left.NoDetailVegetation = bitArr4[8 - 3];
            Right.NoDetailVegetation = bitArr4[8 - 4];
            NoBoundary = bitArr4[8 - 5];
            NoCollision = bitArr4[8 - 6];
            HideInUiMap = bitArr4[8 - 7];
            var highPolyRoad = bitArr4[8 - 8];
            if (highPolyRoad) Resolution = RoadResolution.HighPoly;

            // View distance
            ViewDistance = (ushort)((int)r.ReadByte() * 10);

            // === road_flags ===
            // 1: No terrain shadows; 2: Left Model2 Shift; 
            // 3: Right Model2 Shift; 4: Unknown;
            // 5: Unknown; 6: Ignore cut planes;
            // 7: Superfine; 8: Low-poly vegetation
            var flags5 = r.ReadByte();
            var bitArr5 = new BitArray(new byte[] { flags5 });
            NoTerrainShadows = bitArr5[8 - 1];
            Left.Models[1].Shift = bitArr5[8 - 2];
            Right.Models[1].Shift = bitArr5[8 - 3];
            IgnoreCutPlanes = bitArr5[8 - 6];
            var superfine = bitArr5[8 - 7];
            if (superfine) Resolution = RoadResolution.Superfine;
            LowPolyVegetation = bitArr5[8 - 8];

            // DLC guard
            DlcGuard = r.ReadByte();

            // 1: Right shoulder blocked; 2: Left shoulder blocked;
            // 3: Left Model2 Flip; 4: Left Model1 Flip;
            // 5: Right Model2 Flip; 6: Right Model1 Flip;
            // 7: Smooth detail vegetation; 8: Unknown
            var flags6 = r.ReadByte();
            var bitArr6 = new BitArray(new byte[] { flags6 });
            Right.ShoulderBlocked = bitArr6[8 - 1];
            Left.ShoulderBlocked = bitArr6[8 - 2];
            Left.Models[1].Flip = bitArr6[8 - 3];
            Left.Models[0].Flip = bitArr6[8 - 4];
            Right.Models[1].Flip = bitArr6[8 - 5];
            Right.Models[0].Flip = bitArr6[8 - 6];
            SmoothDetailVegetation = bitArr6[8 - 7];

            // 1: Unknown; 2: Unknown;
            // 3: Unknown; 4: GPS Avoid;
            // 5: Right railing Center part only; 6: Left railing Center part only;
            // 7: Right railing Double sided; 8: Left railing Double sided
            var flags7 = r.ReadByte();
            var bitArr7 = new BitArray(new byte[] { flags7 });
            GpsAvoid = bitArr7[8 - 4];
            Right.Railings.CenterPartOnly = bitArr7[8 - 5];
            Left.Railings.CenterPartOnly = bitArr7[8 - 6];
            Right.Railings.DoubleSided = bitArr7[8 - 7];
            Left.Railings.DoubleSided = bitArr7[8 - 8];

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
            CenterVegetation.VegetationName = r.ReadToken();
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
        }

        /// <summary>
        /// Writes the .base part of this road.
        /// </summary>
        /// <param name="w"></param>
        public override void WriteToStream(BinaryWriter w)
        {
            w.Write(Uid);
            BoundingBox.WriteToStream(w);

            // 1+2: Right terrain transition; 3+4 Left terrain transition
            // 5+6: Right terrain noise; 7+8: Left terrain noise
            byte flags1 = 0;
            flags1 |= (byte)((byte)Right.Terrain.Transition << 6);
            flags1 |= (byte)((byte)Left.Terrain.Transition << 4);
            flags1 |= (byte)((byte)Right.Terrain.Noise << 2);
            flags1 |= (byte)Left.Terrain.Noise;
            w.Write(flags1);

            // 1: Left hand traffic; 2: Water reflection;
            // 3: Left vegetation collision; 4: Right vegetation collision;
            // 5+6: Right sidewalk size; 7+8: Left sidewalk size
            byte flags2 = 0;
            flags2 |= (byte)(LeftHandTraffic.ToByte() << 7);
            flags2 |= (byte)(WaterReflection.ToByte() << 6);
            flags2 |= (byte)(Left.VegetationCollision.ToByte() << 5);
            flags2 |= (byte)(Right.VegetationCollision.ToByte() << 4);
            flags2 |= (byte)((byte)Right.Sidewalk.Size << 2);
            flags2 |= (byte)Left.Sidewalk.Size;
            w.Write(flags2);

            // 1: Unknown; 2: No AI Vehicles;
            // 3: Left Model1 Shift; 4: Right Model1 Shift;
            // 5: Is City Road; 6: Left Invert Railing;
            // 7: Right Invert Railing; 8: Unknown
            byte flags3 = 0;
            flags3 |= (byte)(NoAiVehicles.ToByte() << 6);
            flags3 |= (byte)(Left.Models[0].Shift.ToByte() << 5);
            flags3 |= (byte)(Right.Models[0].Shift.ToByte() << 4);
            flags3 |= (byte)(IsCityRoad.ToByte() << 3);
            flags3 |= (byte)(Left.Railings.InvertRailing.ToByte() << 2);
            flags3 |= (byte)(Right.Railings.InvertRailing.ToByte() << 1);
            w.Write(flags3);

            // 1: Unknown; 2: Stretch terrain; 
            // 3: Left No detail vegetation; 4: Right no detail vegetation;
            // 5: No boundary; 6: No collision;
            // 7: Hide in UI map; 8: High-poly road
            byte flags4 = 0;
            flags4 |= (byte)(StretchTerrain.ToByte() << 6);
            flags4 |= (byte)(Left.NoDetailVegetation.ToByte() << 5);
            flags4 |= (byte)(Right.NoDetailVegetation.ToByte() << 4);
            flags4 |= (byte)(NoBoundary.ToByte() << 3);
            flags4 |= (byte)(NoCollision.ToByte() << 2);
            flags4 |= (byte)(HideInUiMap.ToByte() << 1);
            flags4 |= (Resolution == RoadResolution.HighPoly).ToByte();
            w.Write(flags4);

            // View distance
            w.Write((byte)(ViewDistance / 10));

            // 1: No terrain shadows; 2: Left Model2 Shift; 
            // 3: Right Model2 Shift; 4: Unknown;
            // 5: Unknown; 6: Ignore cut planes;
            // 7: Superfine; 8: Low-poly vegetation
            byte flags5 = 0;
            flags5 |= (byte)(NoTerrainShadows.ToByte() << 7);
            flags5 |= (byte)(Left.Models[1].Shift.ToByte() << 6);
            flags5 |= (byte)(Right.Models[1].Shift.ToByte() << 5);
            flags5 |= (byte)(IgnoreCutPlanes.ToByte() << 2);
            flags5 |= (byte)((Resolution == RoadResolution.Superfine).ToByte() << 1);
            flags5 |= LowPolyVegetation.ToByte();
            w.Write(flags5);

            // DLC guard
            w.Write((byte)DlcGuard);

            // 1: Right shoulder blocked; 2: Left shoulder blocked;
            // 3: Left Model2 Flip; 4: Left Model1 Flip;
            // 5: Right Model2 Flip; 6: Right Model1 Flip;
            // 7: Smooth detail vegetation; 8: Unknown
            byte flags6 = 0;
            flags6 |= (byte)(Right.ShoulderBlocked.ToByte() << 7);
            flags6 |= (byte)(Left.ShoulderBlocked.ToByte() << 6);
            flags6 |= (byte)(Left.Models[1].Flip.ToByte() << 5);
            flags6 |= (byte)(Left.Models[0].Flip.ToByte() << 4);
            flags6 |= (byte)(Right.Models[1].Flip.ToByte() << 3);
            flags6 |= (byte)(Right.Models[0].Flip.ToByte() << 2);
            flags6 |= (byte)(SmoothDetailVegetation.ToByte() << 1);
            w.Write(flags6);

            // 1: Unknown; 2: Unknown;
            // 3: Unknown but I've seen it in use; 4: GPS Avoid;
            // 5: Right railing Center part only; 6: Left railing Center part only;
            // 7: Right railing Double sided; 8: Left railing Double sided
            byte flags7 = 0;
            flags7 |= (byte)(GpsAvoid.ToByte() << 4);
            flags7 |= (byte)(Right.Railings.CenterPartOnly.ToByte() << 3);
            flags7 |= (byte)(Left.Railings.CenterPartOnly.ToByte() << 2);
            flags7 |= (byte)(Right.Railings.DoubleSided.ToByte() << 1);
            flags7 |= Left.Railings.DoubleSided.ToByte();
            w.Write(flags7);

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

            w.Write(Node.Uid);
            w.Write(ForwardNode.Uid);

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
            w.Write(CenterVegetation.VegetationName);
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
        }

    }

}
