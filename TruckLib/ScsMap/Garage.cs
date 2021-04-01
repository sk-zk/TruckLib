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
        /// Determines if the point is the buy point of the garage (1) or not (0).
        /// </summary>
        public uint BuyMode { get; set; }

        public List<INode> TrailerSpawnPoints { get; set; }

        public Garage() : base() { }

        internal Garage(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            TrailerSpawnPoints = new List<INode>();
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

        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);
            ResolveNodeReferences(TrailerSpawnPoints, allNodes);
        }
    }
}
