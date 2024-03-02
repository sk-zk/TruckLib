using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.Models.Ppd;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A prefab slave item which is placed for the <c>GaragePoint</c> and <c>BuyPoint</c>
    /// spawn point types of a prefab.
    /// </summary>
    /// <remarks>
    /// <c>GaragePoint</c> items additionally contain one node for each <c>TrailerSpawn</c> spawn point.
    /// </remarks>
    public class Garage : PrefabSlaveItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Garage;

        /// <summary>
        /// The city the garage is in.
        /// </summary>
        public Token CityName { get; set; }

        /// <summary>
        /// Gets or sets if this is the <c>BuyPoint</c> (true) or the <c>GaragePoint</c> (false).
        /// </summary>
        public bool IsBuyPoint { get; set; }

        /// <summary>
        /// Nodes representing the <c>TrailerSpawn</c> spawn point type of the prefab.
        /// </summary>
        public List<INode> TrailerSpawnPoints { get; set; }

        public Garage() : base() { }

        internal Garage(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            TrailerSpawnPoints = new List<INode>();
        }

        /// <summary>
        /// Adds a garage to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="parent">The prefab this item is linked to.</param>
        /// <param name="position">The position of the node.</param>
        /// <returns>The newly created garage.</returns>
        public static Garage Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<Garage>(map, parent, position);
        }

        /// <inheritdoc/>
        public override void Move(Vector3 newPos)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);

            foreach (var node in TrailerSpawnPoints)
            {
                node.Move(node.Position + translation);
            }
        }

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(IDictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);
            ResolveNodeReferences(TrailerSpawnPoints, allNodes);
        }

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes()
        {
            var nodes = new List<INode>(TrailerSpawnPoints.Count + 1) { Node };
            nodes.AddRange(TrailerSpawnPoints);
            return nodes;
        }
    }
}
