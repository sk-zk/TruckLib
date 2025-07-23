﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class BezierPatchSerializer : MapItemSerializer
    {
        private const float vegDensityFactor = 10f;

        public override MapItem Deserialize(BinaryReader r)
        {
            var bp = new BezierPatch(false);
            ReadKdopItem(r, bp);

            bp.ControlPoints = new Vector3[BezierPatch.ControlPointCols,
                BezierPatch.ControlPointRows];

            for (int x = 0; x < BezierPatch.ControlPointCols; x++)
            {
                for (int z = 0; z < BezierPatch.ControlPointRows; z++)
                {
                    bp.ControlPoints[x, z] = r.ReadVector3();
                }
            }
            bp.ControlPoints = Utils.MirrorX(bp.ControlPoints);

            // Read into field to not trigger recalculation
            bp.tesselation = new(r.ReadUInt16(), r.ReadUInt16());

            bp.Node = new UnresolvedNode(r.ReadUInt64());

            bp.RandomSeed = r.ReadUInt32();

            for (int i = 0; i < bp.Vegetation.Length; i++)
            {
                bp.Vegetation[i].Name = r.ReadToken();
                bp.Vegetation[i].Density = r.ReadUInt16() / vegDensityFactor;
                bp.Vegetation[i].Scale = (VegetationScale)r.ReadByte();
            }
            bp.VegetationSpheres = ReadObjectList<VegetationSphere>(r);

            bp.QuadData = new TerrainQuadData();
            bp.QuadData.Deserialize(r);

            return bp;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var bp = item as BezierPatch;
            WriteKdopItem(w, bp);

            var points = Utils.MirrorX(bp.ControlPoints);
            for (int x = 0; x < BezierPatch.ControlPointCols; x++)
            {
                for (int z = 0; z < BezierPatch.ControlPointRows; z++)
                {
                    w.Write(points[x, z]);
                }
            }

            w.Write(bp.Tesselation.X);
            w.Write(bp.Tesselation.Z);

            w.Write(bp.Node.Uid);

            w.Write(bp.RandomSeed);

            foreach (var veg in bp.Vegetation)
            {
                w.Write(veg.Name);
                w.Write((ushort)(veg.Density * vegDensityFactor));
                w.Write((byte)veg.Scale);
            }

            WriteObjectList(w, bp.VegetationSpheres);

            bp.QuadData.Serialize(w);
        }
    }
}
