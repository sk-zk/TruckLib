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
    /// A prefab slave item which is placed for several spawn point types of a prefab.
    /// </summary>
    public class Service : PrefabSlaveItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Service;

        public List<INode> Nodes { get; set; }

        /// <summary>
        /// The spawn point type.
        /// </summary>
        public ServiceType ServiceType
        {
            get => (ServiceType)Kdop.Flags.GetByte(0);
            set => Kdop.Flags.SetByte(0, (byte)value);
        }

        public Service() : base() { }

        internal Service(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Nodes = [];
        }

        /// <summary>
        /// Adds a Service item to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="parent">The prefab this item is linked to.</param>
        /// <param name="position">The position of the node.</param>
        /// <returns>The newly created Service item.</returns>
        public static Service Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<Service>(map, parent, position);
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

            foreach (var node in Nodes)
            {
                node.Move(node.Position + translation);
            }
        }

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(IDictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);
            ResolveNodeReferences(Nodes, allNodes);
        }

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes()
        {
            var nodes = new List<INode>(Nodes.Count + 1) { Node };
            nodes.AddRange(Nodes);
            return nodes;
        }

    }
}
