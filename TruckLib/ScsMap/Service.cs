using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Additional data for service prefabs (gas stations, weigh stations etc).
    /// </summary>
    public class Service : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.Service;

        public List<Node> Nodes { get; set; } = new List<Node>();

        public ServiceType ServiceType
        {
            get => (ServiceType)Flags.GetByte(0);
            set => Flags.SetByte(0, (byte)value);
        }

        public static Service Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<Service>(map, parent, position);
        }

        internal override void MoveRel(Vector3 translation)
        {
            base.MoveRel(translation);

            foreach (var node in Nodes)
            {
                node.Move(node.Position + translation);
            }
        }

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
