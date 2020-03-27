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

        public List<Node> LongTrailerSpawnPoints { get; set; } = new List<Node>();

        public static Company Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<Company>(map, parent, position);
        }

        internal override void MoveRel(Vector3 translation)
        {
            base.MoveRel(translation);

            var allNodes = UnloadPointsEasy.Concat(UnloadPointsMedium)
                .Concat(UnloadPointsHard).Concat(TrailerSpawnPoints)
                .Concat(Unknown1).Concat(LongTrailerSpawnPoints);

            foreach (var node in allNodes)
            {
                node.Move(node.Position + translation);
            }
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            base.UpdateNodeReferences(allNodes);

            UpdateNodeList(UnloadPointsEasy, allNodes);
            UpdateNodeList(UnloadPointsMedium, allNodes);
            UpdateNodeList(UnloadPointsHard, allNodes);
            UpdateNodeList(TrailerSpawnPoints, allNodes);
            UpdateNodeList(Unknown1, allNodes);
            UpdateNodeList(LongTrailerSpawnPoints, allNodes);
        }

        private static void UpdateNodeList(List<Node> nodeList, Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i] is UnresolvedNode && 
                    allNodes.TryGetValue(nodeList[i].Uid, out var resolvedNode))
                {
                    nodeList[i] = resolvedNode;
                }
            }
        }
    }
}
