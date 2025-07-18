using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A prefab slave item which is placed for the <c>CompanyPoint</c> spawn point type of company prefabs.
    /// </summary>
    /// <remarks>Additional nodes are created for parking spots and trailer spawn points.</remarks>
    public class Company : PrefabSlaveItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Company;

        /// <summary>
        /// Unit name of the company.
        /// </summary>
        public Token CompanyName { get; set; }

        /// <summary>
        /// Unit name of the city in which this company is located, as defined in <c>/def/city.sii</c>.
        /// </summary>
        public Token CityName { get; set; }

        /// <summary>
        /// Additional nodes placed for parking spots and trailer spawn points.
        /// </summary>
        public CompanySpawnPointList SpawnPoints { get; set; }

        public Company() : base() { }

        internal Company(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            SpawnPoints = new(this);
        }

        /// <summary>
        /// Adds a new Company item to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="parent">The prefab this item is linked to.</param>
        /// <param name="position">The position of the item's node.</param>
        /// <returns>The newly created Company item.</returns>
        public static Company Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<Company>(map, parent, position);
        }

        /// <inheritdoc/>
        public override void Move(Vector3 newPos)
        {
            var translation = newPos - Node.Position;
            Translate(translation);
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);

            foreach (var spawnPoint in SpawnPoints)
            {
                spawnPoint.Node.Move(spawnPoint.Node.Position + translation);
            }
        }

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(IDictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);

            for (int i = 0; i < SpawnPoints.Count; i++)
            {
                var spawnPoint = SpawnPoints[i];
                spawnPoint.Node = ResolveNodeReference(spawnPoint.Node, allNodes);
                SpawnPoints[i] = spawnPoint;
            }
        }

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes()
        {
            var nodes = new List<INode>(SpawnPoints.Count + 1) { Node };
            foreach (var point in SpawnPoints)
                nodes.Add(point.Node);
            return nodes;
        }
    }
}
