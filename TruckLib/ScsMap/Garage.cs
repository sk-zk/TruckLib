using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class Garage : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.Garage;

        /// <summary>
        /// The city the garage is in.
        /// </summary>
        public Token CityName { get; set; }

        /// <summary>
        /// Sets if the point is the buy point of the garage (1) or not (0).
        /// </summary>
        public uint BuyMode { get; set; }

        public List<Node> TrailerSpawnPoints { get; set; }

        public Garage() : base() { }

        internal Garage(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            TrailerSpawnPoints = new List<Node>();
        }

        public static Garage Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<Garage>(map, parent, position);
        }

        public override void Move(Vector3 newPos)
        {
            throw new NotImplementedException();
        }

        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);

            foreach (var node in TrailerSpawnPoints)
            {
                node.Move(node.Position + translation);
            }
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            base.UpdateNodeReferences(allNodes);

            for (int i = 0; i < TrailerSpawnPoints.Count; i++)
            {
                if (TrailerSpawnPoints[i] is UnresolvedNode 
                    && allNodes.TryGetValue(TrailerSpawnPoints[i].Uid, out var resolvedNode))
                {
                    TrailerSpawnPoints[i] = resolvedNode;
                }
            }
        }
    }
}
