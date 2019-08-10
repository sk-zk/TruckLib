using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// A standalone terrain segment which acts like a terrain-only road.
    /// </summary>
    public class Terrain : PolylineItem
    {
        // TODO: Use the new KdopItem system
        // not sure how to implement it while keeping Left/Right items
        public override ItemType ItemType => ItemType.Terrain;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

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
        public StepSize StepSize { get; set; } = StepSize.Meters4;

        public Railings Railings { get; set; } = new Railings();

        /// <summary>
        /// The random seed which determines the placement of vegetation models.
        /// </summary>
        public uint RandomSeed { get; set; }

        /// <summary>
        /// The vegetation spheres on this terrain.
        /// </summary>
        public List<VegetationSphere> VegetationSpheres { get; set; } 
            = new List<VegetationSphere>();

        /// <summary>
        /// Determines if the item is reflected in water.
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

        public bool StretchTerrain = false;

        /// <summary>
        /// Determines if the terrain has invisible walls on both sides of it.
        /// </summary>
        public bool NoBoundary = false;

        /// <summary>
        /// Determines if the player can collide with this item.
        /// </summary>
        public bool NoCollision = false;

        public bool NoTerrainShadows = false;

        public bool SmoothDetailVegetation = false;
  
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
        private void InitFromAddOrAppend(Vector3 backwardPos, Vector3 forwardPos, Token material,
            float leftSize, float rightSize)
        {
            Length = Vector3.Distance(backwardPos, forwardPos);
            Left.Terrain.Size = leftSize;
            Right.Terrain.Size = rightSize;
            foreach(var side in new[]{ Left, Right })
            {
                side.Terrain.QuadData.BrushMaterials = new List<Token> { material };
                side.Terrain.CalculateQuadGrid(StepSize, Length);
                side.Terrain.QuadData.Quads = new TerrainQuad[side.Terrain.QuadData.Cols
                    * side.Terrain.QuadData.Rows].Select(h => new TerrainQuad()).ToList();
            }
        }

        private const float modelOffsetFactor = 100f;
        private const float noDetVegFromToFactor = 10f;
        private const float terrainSizeFactor = noDetVegFromToFactor;

        public override void ReadFromStream(BinaryReader r)
        {
            // Uid
            Uid = r.ReadUInt64();

            // KDOP
            BoundingBox.ReadFromStream(r);

            // 1: Water Reflection; 2: Right vegetation collision;
            // 3+4: Step size;
            // 5+6 Right terrain transition;
            // 7+8: Right terrain noise
            var flags1 = r.ReadByte();
            var bitArr1 = new BitArray(new byte[] { flags1 });
            WaterReflection = bitArr1[8 - 1];
            Right.VegetationCollision = bitArr1[8 - 2];
            StepSize = (StepSize)((flags1 >> 4) & 0b11);
            Right.Terrain.Transition = (TerrainTransition)((flags1 >> 2) & 0b11);
            Right.Terrain.Noise = (TerrainNoise)(flags1 & 0b11);

            // 1: Unknown; 2: Ignore cut planes;
            // 3: Low poly vegetation; 4: Stretch terrain;
            // 5: Right no detail vegetation; 6: No boundary;
            // 7: No collision; 8: Invert railing
            var flags2 = r.ReadByte();
            var bitArr2 = new BitArray(new byte[] { flags2 });
            IgnoreCutPlanes = bitArr2[8 - 2];
            LowPolyVegetation = bitArr2[8 - 3];
            StretchTerrain = bitArr2[8 - 4];
            Right.NoDetailVegetation = bitArr2[8 - 5];
            NoBoundary = bitArr2[8 - 6];
            NoCollision = bitArr2[8 - 7];
            Railings.InvertRailing = bitArr2[8 - 8];

            // 6: Smooth detail veg.; 8: No terrain shadows
            // rest unknown
            var flags3 = r.ReadByte();
            var bitArr3 = new BitArray(new byte[] { flags3 });
            SmoothDetailVegetation = bitArr3[8 - 6];
            NoTerrainShadows = bitArr3[8 - 8];

            // 1: Unknown; 2: Unknown;
            // 3: Left vegetation collision; 4: Left no detail vegetation;
            // 5+6 Left terrain transition;
            // 7+8: Left terrain noise
            var flags4 = r.ReadByte();
            var bitArr4 = new BitArray(new byte[] { flags4 });
            Left.VegetationCollision = bitArr4[8 - 3];
            Left.NoDetailVegetation = bitArr4[8 - 4];
            Left.Terrain.Transition = (TerrainTransition)((flags4 >> 2) & 0b11);
            Left.Terrain.Noise = (TerrainNoise)(flags4 & 0b11);

            // View distance
            ViewDistance = (ushort)((int)r.ReadByte() * 10);

            // nodes
            Node = new UnresolvedNode(r.ReadUInt64());
            ForwardNode = new UnresolvedNode(r.ReadUInt64());

            // float3 node0_offset / float3 node1_offset
            // TODO: What is this?
            r.ReadSingle();
            r.ReadSingle();
            r.ReadSingle();
            r.ReadSingle();
            r.ReadSingle();
            r.ReadSingle();

            // road length
            Length = r.ReadSingle();

            // seed
            RandomSeed = r.ReadUInt32();

            // railings
            for (int i = 0; i < Railings.Models.Length; i++)
            {
                Railings.Models[i].Model = r.ReadToken();
                Railings.Models[i].Offset = r.ReadInt16() / modelOffsetFactor;
            }

            // some terrain & veg stuff for each side
            foreach (var side in new[] { Right, Left })
            {
                // terrain size
                side.Terrain.Size = r.ReadUInt16() / terrainSizeFactor;

                // profile
                side.Terrain.Profile = r.ReadToken();

                // coefficient
                side.Terrain.Coefficient = r.ReadSingle();

                // prev_profile
                // TODO: What is this?
                r.ReadToken();

                // prev_profile_coef
                // TODO: What is this?
                r.ReadSingle();

                // vegetation
                foreach (var veg in side.Vegetation)
                {
                    veg.ReadFromStream(r);
                }

                // no det veg.
                side.NoDetailVegetationFrom = r.ReadUInt16() / noDetVegFromToFactor;
                side.NoDetailVegetationTo = r.ReadUInt16() / noDetVegFromToFactor;
            }

            // veg. spheres
            VegetationSpheres = ReadObjectList<VegetationSphere>(r);

            // terrain quad data
            Right.Terrain.QuadData.ReadFromStream(r);
            Left.Terrain.QuadData.ReadFromStream(r);

            // edges
            Right.Edge = r.ReadToken();
            Right.EdgeLook = r.ReadToken();
            Left.Edge = r.ReadToken();
            Left.EdgeLook = r.ReadToken();
        }

        public override void WriteToStream(BinaryWriter w)
        {
            // Uid
            w.Write(Uid);

            // KDOP
            BoundingBox.WriteToStream(w);

            // 1: Water Reflection; 2: Right vegetation collision;
            // 3+4: Step size;
            // 5+6 Right terrain transition;
            // 7+8: Right terrain noise
            byte flags1 = 0;
            flags1 |= (byte)(WaterReflection.ToByte() << 7);
            flags1 |= (byte)(Right.VegetationCollision.ToByte() << 6);
            flags1 |= (byte)((byte)StepSize << 4);
            flags1 |= (byte)((byte)Right.Terrain.Transition << 2);
            flags1 |= (byte)Right.Terrain.Noise;
            w.Write(flags1);

            // 1: Unknown; 2: Ignore cut planes;
            // 3: Low poly vegetation; 4: Stretch terrain;
            // 5: Right no detail vegetation; 6: No boundary;
            // 7: No collision; 8: Invert railing
            byte flags2 = 0;
            flags2 |= (byte)(IgnoreCutPlanes.ToByte() << 6);
            flags2 |= (byte)(LowPolyVegetation.ToByte() << 5);
            flags2 |= (byte)(StretchTerrain.ToByte() << 4);
            flags2 |= (byte)(Right.NoDetailVegetation.ToByte() << 3);
            flags2 |= (byte)(NoBoundary.ToByte() << 2);
            flags2 |= (byte)(NoCollision.ToByte() << 1);
            flags2 |= Railings.InvertRailing.ToByte();
            w.Write(flags2);

            // 6: Smooth detail veg.; 8: No terrain shadows
            // rest unknown
            byte flags3 = 0;
            flags3 |= (byte)(SmoothDetailVegetation.ToByte() << 2);
            flags3 |= NoTerrainShadows.ToByte();
            w.Write(flags3);

            // 1: Unknown; 2: Unknown;
            // 3: Left vegetation collision; 4: Left no detail vegetation;
            // 5+6 Left terrain transition;
            // 7+8: Left terrain noise
            byte flags4 = 0;
            flags4 |= (byte)(Left.VegetationCollision.ToByte() << 5);
            flags4 |= (byte)(Left.NoDetailVegetation.ToByte() << 4);
            flags4 |= (byte)((byte)Left.Terrain.Transition << 2);
            flags4 |= (byte)Left.Terrain.Noise;
            w.Write(flags4);

            // View distance
            w.Write((byte)(ViewDistance / 10));

            // nodes
            w.Write(Node.Uid);
            w.Write(ForwardNode.Uid);

            // node offsets
            w.Write(0f);
            w.Write(0f);
            w.Write(0f);
            w.Write(0f);
            w.Write(0f);
            w.Write(0f);

            // length
            w.Write(Length);

            // random seed
            w.Write(RandomSeed);

            // railings
            // 3 railings per side
            foreach (var railing in Railings.Models)
            {
                // Model
                w.Write(railing.Model);

                // Offset
                w.Write((short)(railing.Offset * modelOffsetFactor));
            }

            // some terrain & veg stuff for each side
            foreach (var side in new[] { Right, Left })
            {
                // Terrain size
                w.Write((ushort)(side.Terrain.Size * terrainSizeFactor));

                // terrain profile
                w.Write(side.Terrain.Profile);

                // terrain coefficient
                w.Write(side.Terrain.Coefficient);

                // prev_profile
                w.Write(0L);

                // prev_profile_coefficient
                w.Write(1f);

                // vegetation
                foreach (var veg in side.Vegetation)
                {
                    veg.WriteToStream(w);
                }

                // no det. veg.
                w.Write((ushort)(side.NoDetailVegetationFrom * noDetVegFromToFactor));
                w.Write((ushort)(side.NoDetailVegetationTo * noDetVegFromToFactor));
            }

            // veg. spheres
            WriteObjectList(w, VegetationSpheres);

            // terrain quads
            Right.Terrain.QuadData.WriteToStream(w);
            Left.Terrain.QuadData.WriteToStream(w);

            // edges
            w.Write(Right.Edge);
            w.Write(Right.EdgeLook);
            w.Write(Left.Edge);
            w.Write(Left.EdgeLook);
        }

    }
}
