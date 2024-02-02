using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class CurveSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var curve = new Curve(false);
            ReadKdopItem(r, curve);

            curve.Model = r.ReadToken();
 
            curve.Node = new UnresolvedNode(r.ReadUInt64());
            curve.ForwardNode = new UnresolvedNode(r.ReadUInt64());

            curve.Locators = new CurveLocatorList(curve);
            var locatorUid = r.ReadUInt64();
            if (locatorUid != 0)
                curve.Locators.Add(new UnresolvedNode(locatorUid));
            locatorUid = r.ReadUInt64();
            if (locatorUid != 0)
                curve.Locators.Add(new UnresolvedNode(locatorUid));

            curve.Length = r.ReadSingle();
 
            curve.RandomSeed = r.ReadUInt32();
 
            curve.Stretch = r.ReadSingle();
            curve.Scale = r.ReadSingle();
            curve.FixedStep = r.ReadSingle();
 
            curve.TerrainMaterial = r.ReadToken();
            curve.TerrainColor = r.ReadColor();
            curve.TerrainRotation = r.ReadSingle();
 
            curve.FirstPart = r.ReadToken();
            curve.LastPart = r.ReadToken();
            curve.CenterPartVariation = r.ReadToken();
 
            curve.Look = r.ReadToken();
 
            curve.HeightOffsets = ReadObjectList<float>(r);

            return curve;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var curve = item as Curve;
            WriteKdopItem(w, curve);

            w.Write(curve.Model);
 
            w.Write(curve.Node.Uid);
            w.Write(curve.ForwardNode.Uid);

            var listSize = curve.Locators.Count;
            for (int i = 0; i < listSize; i++)
            {
                w.Write(curve.Locators[i].Uid);
            }
            for (int i = listSize; i < CurveLocatorList.MaxSize; i++)
            {
                w.Write(0UL);
            }

            w.Write(curve.Length);
 
            w.Write(curve.RandomSeed);
 
            w.Write(curve.Stretch);
            w.Write(curve.Scale);
            w.Write(curve.FixedStep);
 
            w.Write(curve.TerrainMaterial);
            w.Write(curve.TerrainColor);
            w.Write(curve.TerrainRotation);
 
            w.Write(curve.FirstPart);
            w.Write(curve.LastPart);
            w.Write(curve.CenterPartVariation);
 
            w.Write(curve.Look);

            WriteObjectList(w, curve.HeightOffsets);
        }
    }
}
