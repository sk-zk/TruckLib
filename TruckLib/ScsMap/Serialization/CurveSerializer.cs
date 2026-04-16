using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap.Serialization
{
    class CurveSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var curve = new Curve(false);
            ReadKdopItem(r, curve);
 
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

            var subcurveUseMask = r.ReadUInt32();

            var subcurveCount = BitOperations.PopCount(subcurveUseMask);
            var subcurves = r.ReadObjectList<Subcurve>((uint)subcurveCount);

            curve.Subcurves = new Subcurve[Curve.SubcurveCount];
            int srcIdx = 0;
            for (int i = 0; i < Curve.SubcurveCount; i++)
            {
                var idxMask = 1 << i;
                if ((subcurveUseMask & idxMask) != 0)
                {
                    curve.Subcurves[i] = subcurves[srcIdx++];
                }
            }

            return curve;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var curve = item as Curve;
            WriteKdopItem(w, curve);
 
            w.Write(curve.Node.Uid);
            w.Write(curve.ForwardNode.Uid);

            var locatorCount = curve.Locators.Count;
            for (int i = 0; i < locatorCount; i++)
            {
                w.Write(curve.Locators[i].Uid);
            }
            for (int i = locatorCount; i < CurveLocatorList.MaxCapacity; i++)
            {
                w.Write(0UL);
            }

            w.Write(curve.Length);

            var subcurveUseMask = 0;
            for (int i = 0; i < Curve.SubcurveCount; i++)
            {
                if (curve.Subcurves[i] != null)
                {
                    subcurveUseMask |= 1 << i;
                }
            }
            w.Write(subcurveUseMask);

            for (int i = 0; i < Curve.SubcurveCount; i++)
            {
                curve.Subcurves[i]?.Serialize(w);
            }
        }
    }
}
