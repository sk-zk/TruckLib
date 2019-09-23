using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class FuelPump : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.FuelPump;

        public List<Node> Nodes { get; set; } = new List<Node>();

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Node = new UnresolvedNode(r.ReadUInt64());
            PrefabLink = new UnresolvedItem(r.ReadUInt64());

            Nodes = ReadNodeRefList(r);
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(Node.Uid);
            w.Write(PrefabLink.Uid);

            WriteNodeRefList(w, Nodes);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            base.UpdateNodeReferences(allNodes);

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode && allNodes.ContainsKey(Nodes[i].Uid))
                {
                    Nodes[i] = allNodes[Nodes[i].Uid];
                }
            }
        }
    }
}
