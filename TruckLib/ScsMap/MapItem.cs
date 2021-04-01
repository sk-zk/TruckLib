using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for all map items.
    /// </summary>
    public abstract class MapItem : IMapItem
    {
        /// <summary>
        /// The item_type used as identifier in the map format.
        /// </summary>
        public abstract ItemType ItemType { get; }

        /// <summary>
        /// Which sector file the item is written to.
        /// </summary>
        protected ItemFile itemFile;
        public virtual ItemFile ItemFile
        {
            get => itemFile;

            // Signs can be in base or aux depending on the model.
            // It's the only item that behaves like this, so for all other
            // items, the user of the library can't change the itemFile field.
            private set => itemFile = value;
        }

        /// <summary>
        /// The default location for the item type.
        /// </summary>
        public abstract ItemFile DefaultItemFile { get; }

        internal virtual bool HasDataPayload => false;

        internal KdopItem Kdop { get; set; }

        /// <summary>
        /// The UID of this item. 
        /// </summary>
        public ulong Uid
        {
            get => Kdop.Uid;
            set => Kdop.Uid = value;
        }

        /// <summary>
        /// View distance of the item in meters.
        /// </summary>
        protected ushort ViewDistance
        {
            get => Kdop.ViewDistance;
            set => Kdop.ViewDistance = value;
        }

        /// <summary>
        /// The default view distance of the item.
        /// </summary>
        protected abstract ushort DefaultViewDistance { get; }

        /// <summary>
        /// Creates a new item and generates a UID for it.
        /// </summary>
        protected MapItem()
        {
            Init();
        }

        internal MapItem(bool initFields)
        {
            if (initFields) Init();
        }

        protected virtual void Init()
        {
            Kdop = new KdopItem(Utils.GenerateUuid());
            Kdop.ViewDistance = DefaultViewDistance;
            itemFile = DefaultItemFile;
        }

        /// <summary>
        /// Moves the item to a different location.
        /// </summary>
        /// <param name="newPos"></param>
        public abstract void Move(Vector3 newPos);

        /// <summary>
        /// Translates the item to a different location.
        /// </summary>
        /// <param name="translation"></param>
        public abstract void Translate(Vector3 translation);

        /// <summary>
        /// Searches a list of all nodes for the nodes referenced by UID in this map item
        /// and adds references to them in the item's Node fields.
        /// </summary>
        /// <param name="allNodes">A dictionary of all nodes in the entire map.</param>
        internal abstract void UpdateNodeReferences(Dictionary<ulong, INode> allNodes);

        /// <summary>
        /// Returns all external nodes which this item references.
        /// </summary>
        /// <returns></returns>
        internal abstract IEnumerable<INode> GetItemNodes();

        /// <summary>
        /// Returns the node which will be used to determine which sector will be the parent of the item.
        /// </summary>
        /// <returns></returns>
        internal abstract INode GetMainNode();

        protected static INode ResolveNodeReference(INode node, Dictionary<ulong, INode> allNodes)
        {
            if (node is UnresolvedNode && allNodes.TryGetValue(node.Uid, out var resolvedNode))
                return resolvedNode;
            else
                return node;
        }

        protected static void ResolveNodeReferences(IList<INode> nodes, Dictionary<ulong, INode> allNodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i] = ResolveNodeReference(nodes[i], allNodes);
            }
        }

    }
}
