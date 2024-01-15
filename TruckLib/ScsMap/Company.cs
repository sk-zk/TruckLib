using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TruckLib.Model.Ppd;

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

        public List<CompanySpawnPoint> SpawnPoints { get; set; }

        public Company() : base() { }

        internal Company(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            SpawnPoints = new();
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

            foreach (var spawnPoint in SpawnPoints)
            {
                spawnPoint.Node.Move(spawnPoint.Node.Position + translation);
            }
        }

        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);

            for (int i = 0; i < SpawnPoints.Count; i++)
            {
                var spawnPoint = SpawnPoints[i];
                spawnPoint.Node = ResolveNodeReference(spawnPoint.Node, allNodes);
                SpawnPoints[i] = spawnPoint;
            }
        }
    }
}
