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
    /// Additional data for company prefabs.
    /// </summary>
    public class Company : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.Company;

        public Token CompanyName { get; set; }

        public Token CityName { get; set; }

        /// <summary>
        /// List of easy difficulty (15 XP) parking spots.
        /// </summary>
        public List<Node> UnloadPointsEasy { get; set; } = new List<Node>();

        /// <summary>
        /// List of medium difficulty (40 XP) parking spots.
        /// </summary>
        public List<Node> UnloadPointsMedium { get; set; } = new List<Node>();

        /// <summary>
        /// List of hard difficulty (90 XP) parking spots.
        /// </summary>
        public List<Node> UnloadPointsHard { get; set; } = new List<Node>();

        /// <summary>
        /// List of trailer spawn points.
        /// </summary>
        public List<Node> TrailerSpawnPoints { get; set; } = new List<Node>();

        public List<Node> Unknown1 { get; set; } = new List<Node>();
        public List<Node> Unknown2 { get; set; } = new List<Node>();

        public static Company Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            var company = Add<Company>(map, position);
            company.PrefabLink = parent;
            parent.SlaveItems.Add(company);
            return company;
        }

        internal override void MoveRel(Vector3 translation)
        {
            base.MoveRel(translation);

            var allNodes = UnloadPointsEasy.Concat(UnloadPointsMedium)
                .Concat(UnloadPointsHard).Concat(TrailerSpawnPoints)
                .Concat(Unknown1).Concat(Unknown2);

            foreach (var node in allNodes)
            {
                node.Move(node.Position + translation);
            }
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            CompanyName = r.ReadToken();
            CityName = r.ReadToken();

            PrefabLink = new UnresolvedItem(r.ReadUInt64());

            // company node
            Node = new UnresolvedNode(r.ReadUInt64());

            UnloadPointsEasy = ReadNodeRefList(r);
            UnloadPointsMedium = ReadNodeRefList(r);
            UnloadPointsHard = ReadNodeRefList(r);
            TrailerSpawnPoints = ReadNodeRefList(r);

            // unknown
            Unknown1 = ReadNodeRefList(r);
            Unknown2 = ReadNodeRefList(r);
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(CompanyName);
            w.Write(CityName);

            w.Write(PrefabLink.Uid);

            w.Write(Node.Uid);

            WriteNodeRefList(w, UnloadPointsEasy);
            WriteNodeRefList(w, UnloadPointsMedium);
            WriteNodeRefList(w, UnloadPointsHard);
            WriteNodeRefList(w, TrailerSpawnPoints);

            WriteNodeRefList(w, Unknown1);
            WriteNodeRefList(w, Unknown2);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            base.UpdateNodeReferences(allNodes);

            UpdateNodeList(UnloadPointsEasy, allNodes);
            UpdateNodeList(UnloadPointsMedium, allNodes);
            UpdateNodeList(UnloadPointsHard, allNodes);
            UpdateNodeList(TrailerSpawnPoints, allNodes);
            UpdateNodeList(Unknown1, allNodes);
            UpdateNodeList(Unknown2, allNodes);
        }

        private static void UpdateNodeList(List<Node> nodeList, Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i] is UnresolvedNode && allNodes.ContainsKey(nodeList[i].Uid))
                {
                    nodeList[i] = allNodes[nodeList[i].Uid];
                }
            }
        }
    }
}
