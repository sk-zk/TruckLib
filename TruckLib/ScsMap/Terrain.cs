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
            }
        }

        private const float modelOffsetFactor = 100f;
        private const float noDetVegFromToFactor = 10f;
        private const float terrainSizeFactor = noDetVegFromToFactor;
        private const int distFactor = 10;

        public override void ReadFromStream(BinaryReader r)
        {
            // Uid
            Uid = r.ReadUInt64();

            // KDOP
            BoundingBox.ReadFromStream(r);

            var kflag1 = r.ReadByte();
            var karr1 = new BitArray(new[] { kflag1 });
            Right.Terrain.Noise = (TerrainNoise)(kflag1 & 0b11);
            Right.Terrain.Transition = (TerrainTransition)((kflag1 >> 2) & 0b11);
            StepSize = (StepSize)((kflag1 >> 4) & 0b11);
            Right.VegetationCollision = karr1[6];
            WaterReflection = karr1[7];

            var kflag2 = r.ReadByte();
            var karr2 = new BitArray(new[] { kflag2 });
            Railings.InvertRailing = karr2[0];
            NoCollision = karr2[1];
            NoBoundary = karr2[2];
            Right.NoDetailVegetation = karr2[3];
            StretchTerrain = karr2[4];
            LowPolyVegetation = karr2[5];
            IgnoreCutPlanes = karr2[6];

            var kflag3 = r.ReadByte();
            var karr3 = new BitArray(new[] { kflag3 });
            NoTerrainShadows = karr3[0];
            SmoothDetailVegetation = karr3[2];

            var kflag4 = r.ReadByte();
            var karr4 = new BitArray(new[] { kflag4 });
            Left.Terrain.Noise = (TerrainNoise)(kflag4 & 0b11);
            Left.Terrain.Transition = (TerrainTransition)((kflag4 >> 2) & 0b11);
            Left.VegetationCollision = karr4[3];
            Left.NoDetailVegetation = karr4[4];

            // View distance
            ViewDistance = (ushort)(r.ReadByte() * distFactor);

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

            Right.UVRotation = r.ReadSingle();
            Left.UVRotation = r.ReadSingle();
        }

        public override void WriteToStream(BinaryWriter w)
        {
            // Uid
            w.Write(Uid);

            // KDOP
            BoundingBox.WriteToStream(w);

            byte kflag1 = 0;
            kflag1 |= (byte)(WaterReflection.ToByte() << 7);
            kflag1 |= (byte)(Right.VegetationCollision.ToByte() << 6);
            kflag1 |= (byte)((byte)StepSize << 4);
            kflag1 |= (byte)((byte)Right.Terrain.Transition << 2);
            kflag1 |= (byte)Right.Terrain.Noise;
            w.Write(kflag1);

            byte kflag2 = 0;
            kflag2 |= (byte)(IgnoreCutPlanes.ToByte() << 6);
            kflag2 |= (byte)(LowPolyVegetation.ToByte() << 5);
            kflag2 |= (byte)(StretchTerrain.ToByte() << 4);
            kflag2 |= (byte)(Right.NoDetailVegetation.ToByte() << 3);
            kflag2 |= (byte)(NoBoundary.ToByte() << 2);
            kflag2 |= (byte)(NoCollision.ToByte() << 1);
            kflag2 |= Railings.InvertRailing.ToByte();
            w.Write(kflag2);

            byte kflag3 = 0;
            kflag3 |= (byte)(SmoothDetailVegetation.ToByte() << 2);
            kflag3 |= NoTerrainShadows.ToByte();
            w.Write(kflag3);

            byte kflag4 = 0;
            kflag4 |= (byte)(Left.VegetationCollision.ToByte() << 5);
            kflag4 |= (byte)(Left.NoDetailVegetation.ToByte() << 4);
            kflag4 |= (byte)((byte)Left.Terrain.Transition << 2);
            kflag4 |= (byte)Left.Terrain.Noise;
            w.Write(kflag4);

            // View distance
            w.Write((byte)(ViewDistance / distFactor));

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

            w.Write(Right.UVRotation);
            w.Write(Left.UVRotation);
        }

    }
}
