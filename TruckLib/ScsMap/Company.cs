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
        public List<INode> UnloadPointsEasy { get; set; }

        /// <summary>
        /// List of medium difficulty (40 XP) parking spots.
        /// </summary>
        public List<INode> UnloadPointsMedium { get; set; }

        /// <summary>
        /// List of hard difficulty (90 XP) parking spots.
        /// </summary>
        public List<INode> UnloadPointsHard { get; set; }

        /// <summary>
        /// List of trailer spawn points.
        /// </summary>
        public List<INode> TrailerSpawnPoints { get; set; }

        public List<INode> Unknown1 { get; set; }

        public List<INode> LongTrailerSpawnPoints { get; set; } 

        public Company() : base() { }

        internal Company(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            UnloadPointsEasy = new List<INode>();
            UnloadPointsMedium = new List<INode>();
            UnloadPointsHard = new List<INode>();
            TrailerSpawnPoints = new List<INode>();
            Unknown1 = new List<INode>();
            LongTrailerSpawnPoints = new List<INode>();
        }

        public static Company Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<Company>(map, parent, position);
        }

        public override void Move(Vector3 newPos)
        {
            throw new NotImplementedException();
        }

        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);

            var allNodes = UnloadPointsEasy.Concat(UnloadPointsMedium)
                .Concat(UnloadPointsHard).Concat(TrailerSpawnPoints)
                .Concat(Unknown1).Concat(LongTrailerSpawnPoints);

            foreach (var node in allNodes)
            {
                node.Move(node.Position + translation);
            }
        }

        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);

            ResolveNodeReferences(UnloadPointsEasy, allNodes);
            ResolveNodeReferences(UnloadPointsMedium, allNodes);
            ResolveNodeReferences(UnloadPointsHard, allNodes);
            ResolveNodeReferences(TrailerSpawnPoints, allNodes);
            ResolveNodeReferences(Unknown1, allNodes);
            ResolveNodeReferences(LongTrailerSpawnPoints, allNodes);
        }
    }
}
