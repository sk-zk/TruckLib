using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class TerrainSerializer : MapItemSerializer
    {
        private const float modelOffsetFactor = 100f;
        private const float noDetVegFromToFactor = 10f;
        private const float terrainSizeFactor = noDetVegFromToFactor;
        private const int distFactor = 10;

        public override MapItem Deserialize(BinaryReader r)
        {
            var t = new Terrain(false);
            t.Kdop = new KdopItem(false);
            t.Uid = r.ReadUInt64();

            ReadKdopBounds(r, t);

            t.Left = new TerrainSide(false);
            t.Left.Terrain = new RoadTerrain(false);
            t.Right = new TerrainSide(false);
            t.Right.Terrain = new RoadTerrain(false);
            t.Railings = new Railings();

            var kflag1 = r.ReadByte();
            var karr1 = new BitArray(new[] { kflag1 });
            t.Right.Terrain.Noise = (TerrainNoise)(kflag1 & 0b11);
            t.Right.Terrain.Transition = (TerrainTransition)((kflag1 >> 2) & 0b11);
            t.StepSize = (StepSize)((kflag1 >> 4) & 0b11);
            t.Right.VegetationCollision = karr1[6];
            t.WaterReflection = karr1[7];

            var kflag2 = r.ReadByte();
            var karr2 = new BitArray(new[] { kflag2 });
            t.Railings.InvertRailing = karr2[0];
            t.Collision = !karr2[1];
            t.Boundary = !karr2[2];
            t.Right.DetailVegetation = !karr2[3];
            t.StretchTerrain = karr2[4];
            t.LowPolyVegetation = karr2[5];
            t.IgnoreCutPlanes = karr2[6];
            t.UnknownB7 = karr2[7];

            var kflag3 = r.ReadByte();
            var karr3 = new BitArray(new[] { kflag3 });
            t.TerrainShadows = !karr3[0];
            t.UnknownC1 = karr3[1];
            t.SmoothDetailVegetation = karr3[2];
            t.AdaptiveTessellation = karr3[3];
            t.UnknownC4 = karr3[4];
            t.UnknownC5 = karr3[5];
            t.UnknownC6 = karr3[6];
            t.UnknownC7 = karr3[7];

            var kflag4 = r.ReadByte();
            var karr4 = new BitArray(new[] { kflag4 });
            t.Left.Terrain.Noise = (TerrainNoise)(kflag4 & 0b11);
            t.Left.Terrain.Transition = (TerrainTransition)((kflag4 >> 2) & 0b11);
            t.Left.DetailVegetation = !karr4[4];
            t.Left.VegetationCollision = karr4[5];

            t.ViewDistance = (ushort)(r.ReadByte() * distFactor);

            t.Node = new UnresolvedNode(r.ReadUInt64());
            t.ForwardNode = new UnresolvedNode(r.ReadUInt64());

            // TODO: What is this?
            t.NodeOffset = r.ReadVector3();
            t.ForwardNodeOffset = r.ReadVector3();

            t.Length = r.ReadSingle();
            r.ReadSingle(); // previous_length - we don't need to read this in

            t.RandomSeed = r.ReadUInt32();

            for (int i = 0; i < t.Railings.Models.Length; i++)
            {
                t.Railings.Models[i].Model = r.ReadToken();
                t.Railings.Models[i].Offset = r.ReadInt16() / modelOffsetFactor;
            }   

            // some terrain & veg stuff for each side
            foreach (var side in new[] { t.Right, t.Left })
            {
                side.Terrain.Size = r.ReadUInt16() / terrainSizeFactor;
                side.Terrain.Profile = r.ReadToken();
                side.Terrain.Coefficient = r.ReadSingle();

                // prev_profile
                // prev_profile_coef
                // Ignore these because we'll only use them
                // on serialization, which will do it dynamically
                r.ReadToken();
                r.ReadSingle();

                foreach (var veg in side.Vegetation)
                {
                    veg.Deserialize(r);
                }

                side.NoDetailVegetationFrom = r.ReadUInt16() / noDetVegFromToFactor;
                side.NoDetailVegetationTo = r.ReadUInt16() / noDetVegFromToFactor;
            }

            t.VegetationSpheres = ReadObjectList<VegetationSphere>(r);

            t.Right.Terrain.QuadData = new TerrainQuadData(false);
            t.Right.Terrain.QuadData.Deserialize(r);
            t.Left.Terrain.QuadData = new TerrainQuadData(false);
            t.Left.Terrain.QuadData.Deserialize(r);

            t.Right.Edge = r.ReadToken();
            t.Right.EdgeLook = r.ReadToken();
            t.Left.Edge = r.ReadToken();
            t.Left.EdgeLook = r.ReadToken();

            return t;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var t = item as Terrain;
            w.Write(t.Uid);

            WriteKdopBounds(w, t);

            byte kflag1 = 0;
            kflag1 |= (byte)(t.WaterReflection.ToByte() << 7);
            kflag1 |= (byte)(t.Right.VegetationCollision.ToByte() << 6);
            kflag1 |= (byte)((byte)t.StepSize << 4);
            kflag1 |= (byte)((byte)t.Right.Terrain.Transition << 2);
            kflag1 |= (byte)t.Right.Terrain.Noise;
            w.Write(kflag1);

            byte kflag2 = 0;
            kflag2 |= (byte)(t.UnknownB7.ToByte() << 7);
            kflag2 |= (byte)(t.IgnoreCutPlanes.ToByte() << 6);
            kflag2 |= (byte)(t.LowPolyVegetation.ToByte() << 5);
            kflag2 |= (byte)(t.StretchTerrain.ToByte() << 4);
            kflag2 |= (byte)((!t.Right.DetailVegetation).ToByte() << 3);
            kflag2 |= (byte)((!t.Boundary).ToByte() << 2);
            kflag2 |= (byte)((!t.Collision).ToByte() << 1);
            kflag2 |= t.Railings.InvertRailing.ToByte();
            w.Write(kflag2);

            byte kflag3 = 0;
            kflag3 |= (byte)(t.UnknownC7.ToByte() << 7);
            kflag3 |= (byte)(t.UnknownC6.ToByte() << 6);
            kflag3 |= (byte)(t.UnknownC5.ToByte() << 5);
            kflag3 |= (byte)(t.UnknownC4.ToByte() << 4);
            kflag3 |= (byte)(t.AdaptiveTessellation.ToByte() << 3);
            kflag3 |= (byte)(t.SmoothDetailVegetation.ToByte() << 2);
            kflag3 |= (byte)(t.UnknownC1.ToByte() << 1);
            kflag3 |= (!t.TerrainShadows).ToByte();
            w.Write(kflag3);

            byte kflag4 = 0;
            kflag4 |= (byte)(t.Left.VegetationCollision.ToByte() << 5);
            kflag4 |= (byte)((!t.Left.DetailVegetation).ToByte() << 4);
            kflag4 |= (byte)((byte)t.Left.Terrain.Transition << 2);
            kflag4 |= (byte)t.Left.Terrain.Noise;
            w.Write(kflag4);

            w.Write((byte)(t.ViewDistance / distFactor));

            w.Write(t.Node.Uid);
            w.Write(t.ForwardNode.Uid);

            w.Write(t.NodeOffset);
            w.Write(t.ForwardNodeOffset);

            w.Write(t.Length);
            w.Write(PolylineItem.SumPrecedingLengths(t)); 

            w.Write(t.RandomSeed);

            foreach (var railing in t.Railings.Models)
            {
                w.Write(railing.Model);
                w.Write((short)(railing.Offset * modelOffsetFactor));
            }

            TerrainSide bwRight = null;
            TerrainSide bwLeft = null;
            if (t.BackwardItem is Terrain bwTerrain)
            {
                bwRight = bwTerrain.Right;
                bwLeft = bwTerrain.Left;
            }
            WriteThatTerrainAndVegetationPart(t.Right, bwRight);
            WriteThatTerrainAndVegetationPart(t.Left, bwLeft);

            WriteObjectList(w, t.VegetationSpheres);

            t.Right.Terrain.QuadData.Serialize(w);
            t.Left.Terrain.QuadData.Serialize(w);

            w.Write(t.Right.Edge);
            w.Write(t.Right.EdgeLook);
            w.Write(t.Left.Edge);
            w.Write(t.Left.EdgeLook);

            void WriteThatTerrainAndVegetationPart(TerrainSide side, TerrainSide bwSide)
            {
                w.Write((ushort)(side.Terrain.Size * terrainSizeFactor));
                w.Write(side.Terrain.Profile);
                w.Write(side.Terrain.Coefficient);

                if (bwSide is null)
                {
                    // prev profile
                    w.Write(0UL);
                    // prev coef
                    w.Write(1f);
                }
                else
                {
                    // prev profile
                    w.Write(bwSide.Terrain.Profile);
                    // prev coef
                    w.Write(bwSide.Terrain.Coefficient);
                }

                foreach (var veg in side.Vegetation)
                {
                    veg.Serialize(w);
                }

                w.Write((ushort)(side.NoDetailVegetationFrom * noDetVegFromToFactor));
                w.Write((ushort)(side.NoDetailVegetationTo * noDetVegFromToFactor));
            }
        }
    }
}
