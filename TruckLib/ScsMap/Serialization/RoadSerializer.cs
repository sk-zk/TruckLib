using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class RoadSerializer : MapItemSerializer, IDataPayload
    {
        private const float railingOffsetFactor = 100f;
        private const float modelOffsetFactor = 100f;
        private const float modelDistanceFactor = 100f;
        private const float heightOffsetFactor = 100f;
        private const float terrainSizeFactor = 10f;
        private const float centerVegDensityFactor = 10f;
        private const float centerVegOffsetFactor = 10f;
        private const float vegFromToFactor = 10f;
        private const int viewDistFactor = 10;

        public override MapItem Deserialize(BinaryReader r)
        {
            var road = new Road();

            road.Uid = r.ReadUInt64();
            road.BoundingBox.Deserialize(r);

            // === kdop flags ===
            var kflag1 = r.ReadByte();
            road.Left.Terrain.Noise = (TerrainNoise)(kflag1 & 0b11);
            road.Right.Terrain.Noise = (TerrainNoise)((kflag1 >> 2) & 0b11);
            road.Left.Terrain.Transition = (TerrainTransition)((kflag1 >> 4) & 0b11);
            road.Right.Terrain.Transition = (TerrainTransition)((kflag1 >> 6) & 0b11);

            var kflag2 = r.ReadByte();
            var karr2 = new BitArray(new[] { kflag2 });
            road.Left.Sidewalk.Size = (SidewalkSize)(kflag2 & 0b11);
            road.Right.Sidewalk.Size = (SidewalkSize)((kflag2 >> 2) & 0b11);
            road.Right.VegetationCollision = karr2[4];
            road.Left.VegetationCollision = karr2[5];
            road.WaterReflection = karr2[6];
            road.LeftHandTraffic = karr2[7];

            var kflag3 = r.ReadByte();
            var karr3 = new BitArray(new[] { kflag3 });
            road.Right.Railings.InvertRailing = karr3[1];
            road.Left.Railings.InvertRailing = karr3[2];
            road.IsCityRoad = karr3[3];
            road.Right.Models[0].Shift = karr3[4];
            road.Left.Models[0].Shift = karr3[5];
            road.AiVehicles = !karr3[6];

            var kflag4 = r.ReadByte();
            var karr4 = new BitArray(new[] { kflag4 });
            var highPolyRoad = karr4[0];
            road.Resolution = highPolyRoad ? RoadResolution.HighPoly : RoadResolution.Normal;
            road.ShowInUiMap = !karr4[1];
            road.Collision = !karr4[2];
            road.Boundary = !karr4[3];
            road.Right.DetailVegetation = !karr4[4];
            road.Left.DetailVegetation = !karr4[5];
            road.StretchTerrain = karr4[6];

            road.ViewDistance = (ushort)(r.ReadByte() * viewDistFactor);

            // === road_flags ===
            var rflag1 = r.ReadByte();
            var rarr1 = new BitArray(new[] { rflag1 });
            road.LowPolyVegetation = rarr1[0];
            var superfine = rarr1[1];
            if (superfine) road.Resolution = RoadResolution.Superfine;
            road.IgnoreCutPlanes = rarr1[2];
            road.Right.Models[1].Shift = rarr1[5];
            road.Left.Models[1].Shift = rarr1[6];
            road.TerrainShadows = !rarr1[7];

            road.DlcGuard = r.ReadByte();

            var rflag3 = r.ReadByte();
            var rarr3 = new BitArray(new[] { rflag3 });
            road.SmoothDetailVegetation = rarr3[1];
            road.Right.Models[0].Flip = rarr3[2];
            road.Right.Models[1].Flip = rarr3[3];
            road.Left.Models[0].Flip = rarr3[4];
            road.Left.Models[1].Flip = rarr3[5];
            road.Left.ShoulderBlocked = rarr3[6];
            road.Right.ShoulderBlocked = rarr3[7];

            var rflag4 = r.ReadByte();
            var rarr4 = new BitArray(new[] { rflag4 });
            road.Left.Railings.DoubleSided = rarr4[0];
            road.Right.Railings.DoubleSided = rarr4[1];
            road.Left.Railings.CenterPartOnly = rarr4[2];
            road.Right.Railings.CenterPartOnly = rarr4[3];
            road.GpsAvoid = rarr4[4];

            road.RoadType = r.ReadToken();

            road.Right.Variant = r.ReadToken();
            road.Left.Variant = r.ReadToken();

            road.Right.RightEdge = r.ReadToken();
            road.Right.LeftEdge = r.ReadToken();
            road.Left.RightEdge = r.ReadToken();
            road.Left.LeftEdge = r.ReadToken();
             
            road.Right.Terrain.Profile = r.ReadToken();
            road.Right.Terrain.Coefficient = r.ReadSingle();
            road.Left.Terrain.Profile = r.ReadToken();
            road.Left.Terrain.Coefficient = r.ReadSingle();
            
            road.Right.Look = r.ReadToken();
            road.Left.Look = r.ReadToken();

            road.Material = r.ReadToken();

            // 3 railings per side
            for (int i = 0; i < 3; i++)
            {
                road.Right.Railings.Models[i].Model = r.ReadToken();
                road.Right.Railings.Models[i].Offset = r.ReadInt16() / railingOffsetFactor;
                road.Left.Railings.Models[i].Model = r.ReadToken();
                road.Left.Railings.Models[i].Offset = r.ReadInt16() / railingOffsetFactor;
            }

            road.Right.RoadHeightOffset = r.ReadInt32() / heightOffsetFactor;
            road.Left.RoadHeightOffset = r.ReadInt32() / heightOffsetFactor;

            road.Node = new UnresolvedNode(r.ReadUInt64());
            road.ForwardNode = new UnresolvedNode(r.ReadUInt64());

            road.Length = r.ReadSingle();

            return road;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var road = item as Road;
            w.Write(road.Uid);
            road.BoundingBox.Serialize(w);

            // === kdop flags ===
            byte kflag1 = 0;
            kflag1 |= (byte)road.Left.Terrain.Noise;
            kflag1 |= (byte)((byte)road.Right.Terrain.Noise << 2);
            kflag1 |= (byte)((byte)road.Left.Terrain.Transition << 4);
            kflag1 |= (byte)((byte)road.Right.Terrain.Transition << 6);
            w.Write(kflag1);

            byte kflag2 = 0;
            kflag2 |= (byte)road.Left.Sidewalk.Size;
            kflag2 |= (byte)((byte)road.Right.Sidewalk.Size << 2);
            kflag2 |= (byte)(road.Right.VegetationCollision.ToByte() << 4);
            kflag2 |= (byte)(road.Left.VegetationCollision.ToByte() << 5);
            kflag2 |= (byte)(road.WaterReflection.ToByte() << 6);
            kflag2 |= (byte)(road.LeftHandTraffic.ToByte() << 7);
            w.Write(kflag2);

            byte kflag3 = 0;
            // bit 0: obsolete Terrain Only flag
            kflag3 |= (byte)(road.Right.Railings.InvertRailing.ToByte() << 1);
            kflag3 |= (byte)(road.Left.Railings.InvertRailing.ToByte() << 2);
            kflag3 |= (byte)(road.IsCityRoad.ToByte() << 3);
            kflag3 |= (byte)(road.Right.Models[0].Shift.ToByte() << 4);
            kflag3 |= (byte)(road.Left.Models[0].Shift.ToByte() << 5);
            kflag3 |= (byte)((!road.AiVehicles).ToByte() << 6);
            w.Write(kflag3);

            byte kflag4 = 0;
            kflag4 |= (road.Resolution == RoadResolution.HighPoly).ToByte();
            kflag4 |= (byte)((!road.ShowInUiMap).ToByte() << 1);
            kflag4 |= (byte)((!road.Collision).ToByte() << 2);
            kflag4 |= (byte)((!road.Boundary).ToByte() << 3);
            kflag4 |= (byte)((!road.Right.DetailVegetation).ToByte() << 4);
            kflag4 |= (byte)((!road.Left.DetailVegetation).ToByte() << 5);
            kflag4 |= (byte)(road.StretchTerrain.ToByte() << 6);
            w.Write(kflag4);

            w.Write((byte)(road.ViewDistance / viewDistFactor));

            // === road_flags ===
            byte rflag1 = 0;
            rflag1 |= road.LowPolyVegetation.ToByte();
            rflag1 |= (byte)((road.Resolution == RoadResolution.Superfine).ToByte() << 1);
            rflag1 |= (byte)(road.IgnoreCutPlanes.ToByte() << 2);
            rflag1 |= (byte)(road.Right.Models[1].Shift.ToByte() << 5);
            rflag1 |= (byte)(road.Left.Models[1].Shift.ToByte() << 6);
            rflag1 |= (byte)((!road.TerrainShadows).ToByte() << 7);
            w.Write(rflag1);

            w.Write(road.DlcGuard);

            byte rflag3 = 0;
            rflag3 |= (byte)(road.SmoothDetailVegetation.ToByte() << 1);
            rflag3 |= (byte)(road.Right.Models[0].Flip.ToByte() << 2);
            rflag3 |= (byte)(road.Right.Models[1].Flip.ToByte() << 3);
            rflag3 |= (byte)(road.Left.Models[0].Flip.ToByte() << 4);
            rflag3 |= (byte)(road.Left.Models[1].Flip.ToByte() << 5);
            rflag3 |= (byte)(road.Left.ShoulderBlocked.ToByte() << 6);
            rflag3 |= (byte)(road.Right.ShoulderBlocked.ToByte() << 7);
            w.Write(rflag3);

            byte rflag4 = 0;
            rflag4 |= road.Left.Railings.DoubleSided.ToByte();
            rflag4 |= (byte)(road.Right.Railings.DoubleSided.ToByte() << 1);
            rflag4 |= (byte)(road.Left.Railings.CenterPartOnly.ToByte() << 2);
            rflag4 |= (byte)(road.Right.Railings.CenterPartOnly.ToByte() << 3);
            rflag4 |= (byte)(road.GpsAvoid.ToByte() << 4);
            w.Write(rflag4);

            w.Write(road.RoadType);

            w.Write(road.Right.Variant);
            w.Write(road.Left.Variant);

            w.Write(road.Right.RightEdge);
            w.Write(road.Right.LeftEdge);
            w.Write(road.Left.RightEdge);
            w.Write(road.Left.LeftEdge);

            w.Write(road.Right.Terrain.Profile);
            w.Write(road.Right.Terrain.Coefficient);
            w.Write(road.Left.Terrain.Profile);
            w.Write(road.Left.Terrain.Coefficient);

            w.Write(road.Right.Look);
            w.Write(road.Left.Look);

            w.Write(road.Material);

            // 3 railings per side
            for (int i = 0; i < 3; i++)
            {
                w.Write(road.Right.Railings.Models[i].Model);
                w.Write((short)(road.Right.Railings.Models[i].Offset * railingOffsetFactor));
                w.Write(road.Left.Railings.Models[i].Model);
                w.Write((short)(road.Left.Railings.Models[i].Offset * railingOffsetFactor));
            }

            w.Write((int)(road.Right.RoadHeightOffset * heightOffsetFactor));
            w.Write((int)(road.Left.RoadHeightOffset * heightOffsetFactor));

            w.Write(road.Node?.Uid ?? 0UL);
            w.Write(road.ForwardNode?.Uid ?? 0UL);

            w.Write(road.Length);
        }

        public void DeserializeDataPayload(BinaryReader r, MapItem item)
        {
            var road = item as Road;

            // Overlay Scheme
            // TODO: What is this?
            road.OverlayScheme = r.ReadToken();

            foreach (var side in new[] { road.Right, road.Left }) // repeated for both sides of the road
            {
                foreach (var model in side.Models)
                {
                    model.Name = r.ReadToken();
                    model.Offset = r.ReadInt16() / modelOffsetFactor;
                    model.Distance = r.ReadUInt16() / modelDistanceFactor;
                }

                side.Terrain.Size = r.ReadUInt16() / terrainSizeFactor;

                foreach (var veg in side.Vegetation)
                {
                    veg.Deserialize(r);
                }

                side.Sidewalk.Material = r.ReadToken();
                side.Terrain.QuadData.Deserialize(r);
            }

            road.CenterMaterial = r.ReadToken();
            road.CenterMaterialColor = r.ReadColor();

            road.RandomSeed = r.ReadUInt32();
            road.CenterVegetation.Name = r.ReadToken();
            road.CenterVegetation.Density = r.ReadUInt16() / centerVegDensityFactor;
            road.CenterVegetation.Scale = (VegetationScale)r.ReadByte();
            road.CenterVegetation.Offset = r.ReadByte() / centerVegOffsetFactor;
            road.Left.NoDetailVegetationFrom = r.ReadUInt16() / vegFromToFactor;
            road.Right.NoDetailVegetationFrom = r.ReadUInt16() / vegFromToFactor;
            road.Left.NoDetailVegetationTo = r.ReadUInt16() / vegFromToFactor;
            road.Right.NoDetailVegetationTo = r.ReadUInt16() / vegFromToFactor;

            var vegSphereCount = r.ReadUInt32();
            for (int i = 0; i < vegSphereCount; i++)
            {
                var sphere = new VegetationSphere();
                sphere.Deserialize(r);
                road.VegetationSpheres.Add(sphere);
            }

            var leftAdditionalPartsCount = r.ReadUInt32();
            for (int i = 0; i < leftAdditionalPartsCount; i++)
            {
                road.Left.AdditionalParts.Add(r.ReadToken());
            }

            var rightAdditionalPartsCount = r.ReadUInt32();
            for (int i = 0; i < rightAdditionalPartsCount; i++)
            {
                road.Right.AdditionalParts.Add(r.ReadToken());
            }

            road.Right.UVRotation = r.ReadSingle();
            road.Left.UVRotation = r.ReadSingle();
        }

        public void SerializeDataPayload(BinaryWriter w, MapItem item)
        {
            var road = item as Road;

            // overlay scheme
            w.Write(road.OverlayScheme);

            foreach (var side in new[] { road.Right, road.Left }) // repeated for both sides of the road
            {
                foreach (var model in side.Models)
                {
                    w.Write(model.Name);
                    w.Write((short)(model.Offset * modelOffsetFactor));
                    w.Write((short)(model.Distance * modelDistanceFactor));
                }

                w.Write((ushort)(side.Terrain.Size * terrainSizeFactor));

                foreach (var veg in side.Vegetation)
                {
                    veg.Serialize(w);
                }

                w.Write(side.Sidewalk.Material);

                side.Terrain.QuadData.Serialize(w);
            }

            w.Write(road.CenterMaterial);
            w.Write(road.CenterMaterialColor);
            w.Write(road.RandomSeed);
            w.Write(road.CenterVegetation.Name);
            w.Write((ushort)(road.CenterVegetation.Density * centerVegDensityFactor));
            w.Write((byte)road.CenterVegetation.Scale);
            w.Write((byte)(road.CenterVegetation.Offset * centerVegOffsetFactor));

            w.Write((ushort)(road.Left.NoDetailVegetationFrom * vegFromToFactor));
            w.Write((ushort)(road.Right.NoDetailVegetationFrom * vegFromToFactor));
            w.Write((ushort)(road.Left.NoDetailVegetationTo * vegFromToFactor));
            w.Write((ushort)(road.Right.NoDetailVegetationTo * vegFromToFactor));

            w.Write(road.VegetationSpheres.Count);
            foreach (var sphere in road.VegetationSpheres)
            {
                sphere.Serialize(w);
            }

            w.Write(road.Left.AdditionalParts.Count);
            foreach (var part in road.Left.AdditionalParts)
            {
                w.Write(part);
            }
            w.Write(road.Right.AdditionalParts.Count);
            foreach (var part in road.Right.AdditionalParts)
            {
                w.Write(part);
            }

            w.Write(road.Right.UVRotation);
            w.Write(road.Left.UVRotation);
        }
    }
}
