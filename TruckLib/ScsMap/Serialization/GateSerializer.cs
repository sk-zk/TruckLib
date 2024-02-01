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
        private const int UnusedActivationPointIndex = -1;

        public override MapItem Deserialize(BinaryReader r)
        {
            var gate = new Gate(false);
            ReadKdopItem(r, gate);

            gate.Model = r.ReadToken();

            var nodeCount = r.ReadUInt32();
            gate.Node = new UnresolvedNode(r.ReadUInt64());
            var activationPointNodes = new UnresolvedNode[nodeCount - 1];
            for (int i = 1; i < nodeCount; i++)
            {
                activationPointNodes[i - 1] = new UnresolvedNode(r.ReadUInt64());
            }

            gate.ActivationPoints = new GateActivationPointList(gate);
            for (int i = 0; i < GateActivationPointList.MaxSize; i++)
            {
                var trigger = r.ReadPascalString();
                var nodeIndex = r.ReadInt32();
                if (nodeIndex != UnusedActivationPointIndex)
                {
                    var point = new GateActivationPoint
                    {
                        Trigger = trigger,
                        Node = activationPointNodes[nodeIndex - 1]
                    };
                    gate.ActivationPoints.Add(point, false);
                }
            }

            return gate;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var gate = item as Gate;
            WriteKdopItem(w, gate);

            w.Write(gate.Model);

            w.Write(gate.ActivationPoints.Count + 1);
            w.Write(gate.Node.Uid);
            foreach (var point in gate.ActivationPoints)
            {
                w.Write(point.Node.Uid);
            }

            var listSize = gate.ActivationPoints.Count;
            for (int i = 0; i < listSize; i++)
            {
                w.WritePascalString(gate.ActivationPoints[i].Trigger);
                w.Write(i + 1);
            }
            for (int i = listSize; i < GateActivationPointList.MaxSize; i++)
            {
                w.WritePascalString("");
                w.Write(UnusedActivationPointIndex);
            }
        }
    }
}
