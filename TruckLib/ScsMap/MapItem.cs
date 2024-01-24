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
    /// The base class for all map items.
    /// </summary>
    public abstract class MapItem : IMapItem
    {
        /// <summary>
        /// The item type ID used as identifier in the map format.
        /// </summary>
        public abstract ItemType ItemType { get; }

        private ItemFile itemFile;
        /// <summary>
        /// Gets which sector file the item is written to.
        /// </summary>
        public virtual ItemFile ItemFile
        {
            get => itemFile;

            // Signs can be in base or aux depending on the model.
            // It's the only item that behaves like this, so for all other
            // items, the user of the library can't change the itemFile field.
            internal set => itemFile = value;
        }

        /// <summary>
        /// Gets the default location for the item type.
        /// </summary>
        public abstract ItemFile DefaultItemFile { get; }

        /// <summary>
        /// Whether the item has a .data payload.
        /// </summary>
        internal virtual bool HasDataPayload => false;

        internal KdopItem Kdop { get; set; }

        /// <summary>
        /// Gets or sets the UID of this item. 
        /// </summary>
        public ulong Uid
        {
            get => Kdop.Uid;
            set => Kdop.Uid = value;
        }

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        protected ushort ViewDistance
        {
            get => Kdop.ViewDistance;
            set => Kdop.ViewDistance = value;
        }

        /// <summary>
        /// Gets the default view distance of the item.
        /// </summary>
        protected abstract ushort DefaultViewDistance { get; }

        /// <summary>
        /// The default editor layer for map items.
        /// </summary>
        public const int DefaultLayer = 0;
        private byte layer = DefaultLayer;
        /// <summary>
        /// Gets or sets the editor layer to which this item belongs.
        /// </summary>
        public byte Layer {
            get => layer;
            set { layer = Utils.SetIfInRange(value, (byte)0, (byte)8); }
        }

        /// <summary>
        /// Instantiates a new item and generates a UID for it.
        /// </summary>
        protected MapItem()
        {
            itemFile = DefaultItemFile;
            Init();
        }

        /// <summary>
        /// Instantiates a new item.
        /// </summary>
        /// <param name="initFields">If true, <see cref="MapItem.Init">Init()</see> is called.
        /// Otherwise, the item's properties are left uninitialized.</param>
        internal MapItem(bool initFields)
        {
            itemFile = DefaultItemFile;
            if (initFields) Init();
        }

        /// <summary>
        /// Sets the item's properties to its default values.
        /// </summary>
        protected virtual void Init()
        {
            Kdop = new KdopItem(Utils.GenerateUuid());
            Kdop.ViewDistance = DefaultViewDistance;
        }

        /// <summary>
        /// Moves the item to a different location.
        /// </summary>
        /// <param name="newPos">The new position.</param>
        public abstract void Move(Vector3 newPos);

        /// <summary>
        /// Translates the item by the given vector.
        /// </summary>
        /// <param name="translation">The translation vector.</param>
        public abstract void Translate(Vector3 translation);

        /// <summary>
        /// Searches a list of all nodes for the nodes referenced by UID in this map item
        /// and updates the respective references. This is used for loading a map and
        /// does not need to be called by the user at any point.
        /// </summary>
        /// <param name="allNodes">A dictionary of all nodes in the entire map.</param>
        internal abstract void UpdateNodeReferences(Dictionary<ulong, INode> allNodes);

        /// <summary>
        /// Returns all external nodes which this item references.
        /// </summary>
        /// <returns>All external nodes which this item references.</returns>
        internal abstract IEnumerable<INode> GetItemNodes();

        /// <summary>
        /// Returns the node which will be used to determine the sector
        /// which will be the parent of the item.
        /// </summary>
        /// <returns>
        /// The node which will be used to determine the sector
        /// which will be the parent of the item.
        /// </returns>
        internal abstract INode GetMainNode();

        /// <summary>
        /// Attempts to resolve the given <see cref="UnresolvedNode"/> and returns
        /// the resolved node.
        /// If the node is not an <see cref="UnresolvedNode"/> or <paramref name="allNodes"/>
        /// does not contain it, <paramref name="node"/> is returned unmodified.
        /// </summary>
        /// <param name="node">The node to resolve.</param>
        /// <param name="allNodes">A dictionary of all nodes in the entire map.</param>
        /// <returns>The resolved node if it could be resolved; returns <paramref name="node"/>
        /// unmodified otherwise.</returns>
        protected static INode ResolveNodeReference(INode node, Dictionary<ulong, INode> allNodes)
        {
            if (node is UnresolvedNode && allNodes.TryGetValue(node.Uid, out var resolvedNode))
                return resolvedNode;
            else
                return node;
        }

        /// <summary>
        /// Attempts to resolve the given list of <see cref="UnresolvedNode"/>s and returns
        /// the resolved nodes.
        /// Any node which is not an <see cref="UnresolvedNode"/> or not contained in
        /// <paramref name="allNodes"/> is not modified.
        /// </summary>
        /// <param name="nodes">The nodes to resolve.</param>
        /// <param name="allNodes">A dictionary of all nodes in the entire map.</param>
        protected static void ResolveNodeReferences(IList<INode> nodes, Dictionary<ulong, INode> allNodes)
        {
            for (int i = 0; i < nodes.Count; i++)
                nodes[i] = ResolveNodeReference(nodes[i], allNodes);
        }
    }
}
