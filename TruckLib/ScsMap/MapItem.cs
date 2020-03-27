using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for all map items.
    /// </summary>
    public abstract class MapItem : IMapObject
    {
        /// <summary>
        /// The item_type used as identifier in the map format.
        /// </summary>
        public abstract ItemType ItemType
        {
            get; 
        }

        /// <summary>
        /// Whether the item goes to .base or .aux.
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

        public float[] KdopMimimums
        {
            get => Kdop.Minimums;
            set => Kdop.Minimums = value;
        }

        public float[] KdopMaximums
        {
            get => Kdop.Maximums;
            set => Kdop.Maximums = value;
        }

        /// <summary>
        /// View distance of an item in meters.
        /// </summary>
        protected ushort ViewDistance
        {
            get => Kdop.ViewDistance;
            set => Kdop.ViewDistance = value;
        }

        protected abstract ushort DefaultViewDistance { get; }

        /// <summary>
        /// Creates a new item and generates a UID for it.
        /// </summary>
        public MapItem()
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
        /// Searches a list of all nodes for the nodes referenced by UID in this map item
        /// and adds references to them in the item's Node fields.
        /// </summary>
        /// <param name="allNodes">A dictionary of all nodes in the entire map.</param>
        public abstract void UpdateNodeReferences(Dictionary<ulong, Node> allNodes);

        /// <summary>
        /// Returns all external nodes this item references.
        /// </summary>
        /// <returns></returns>
        internal abstract IEnumerable<Node> GetItemNodes();
    }
}
