using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Parent class for map items which only have one node.
    /// </summary>
    public abstract class SingleNodeItem : MapItem
    {
        /// <summary>
        /// The node of the item.
        /// </summary>
        public INode Node { get; set; }

        public SingleNodeItem() : base() { }

        internal SingleNodeItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <summary>
        /// Base method for adding a new SingleNodeItem to the map.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="map"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        internal static T Add<T>(IItemContainer map, Vector3 position) where T : SingleNodeItem, new()
        {
            var node = map.AddNode(position, true);

            var newItem = new T();
            newItem.Node = node;
            newItem.Node.ForwardItem = newItem;
            map.AddItem(newItem);

            return newItem;
        }

        public override void Move(Vector3 newPos)
        {
            Node.Move(newPos);
        }

        public override void Translate(Vector3 translation)
        {
            Node.Move(Node.Position + translation);
        }

        internal override IEnumerable<INode> GetItemNodes() => new[] { Node };

        internal override INode GetMainNode() => Node;

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            Node = ResolveNodeReference(Node, allNodes);
        }
    }
}
