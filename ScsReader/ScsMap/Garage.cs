using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    public class Garage : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.Garage;

        /// <summary>
        /// The city the garage is in.
        /// </summary>
        public Token CityName;

        /// <summary>
        /// TODO: What is this?
        /// </summary>
        public uint BuyMode;

        public List<Node> Nodes = new List<Node>();

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            CityName = r.ReadToken();
            BuyMode = r.ReadUInt32();

            Node = new UnresolvedNode(r.ReadUInt64());
            PrefabLink = new UnresolvedItem(r.ReadUInt64());

            Nodes = ReadNodeRefList(r);
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(CityName);
            w.Write(BuyMode);

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
