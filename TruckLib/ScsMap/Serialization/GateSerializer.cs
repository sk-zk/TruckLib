using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap.Serialization
{
    internal class GateSerializer : MapItemSerializer
    {
        private const int EmptyActivationPointStructSignifier = -1;
        private const int ActivationPointStructAmount = 2;

        public override MapItem Deserialize(BinaryReader r)
        {
            var gate = new Gate(false);
            ReadKdopItem(r, gate);

            gate.Model = r.ReadToken();

            var nodeCount = r.ReadUInt32();
            gate.Node = new UnresolvedNode(r.ReadUInt64());
            var otherNodes = new UnresolvedNode[nodeCount - 1];
            for (int i = 1; i < nodeCount; i++)
            {
                otherNodes[i - 1] = new UnresolvedNode(r.ReadUInt64());
            }

            for (int i = 0; i < ActivationPointStructAmount; i++)
            {
                var trigger = r.ReadPascalString();
                var nodeIndex = r.ReadInt32();
                if (nodeIndex != EmptyActivationPointStructSignifier)
                {
                    var point = new GateActivationPoint
                    {
                        Trigger = trigger,
                        Node = otherNodes[nodeIndex - 1]
                    };
                    gate.ActivationPoints[i] = point;
                }
            }

            return gate;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var gate = item as Gate;
            WriteKdopItem(w, gate);

            w.Write(gate.Model);

            var activationPointNodeUids = gate.ActivationPoints.Where(x => x is not null).Select(x => x.Node.Uid).ToArray();
            var activationPointCount = activationPointNodeUids.Length;
            w.Write(activationPointCount + 1);
            w.Write(gate.Node.Uid);
            foreach (var uid in activationPointNodeUids)
            {
                w.Write(uid);
            }

            foreach (var point in gate.ActivationPoints)
            {
                if (point is null)
                {
                    w.WritePascalString("");
                    w.Write(-1);
                } 
                else
                {
                    w.WritePascalString(point.Trigger);
                    w.Write(Array.IndexOf(activationPointNodeUids, gate.Uid) + 1);
                }
            }
        }
    }
}
