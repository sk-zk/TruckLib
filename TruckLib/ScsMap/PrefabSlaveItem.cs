using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for prefab slave items, which is a type of item that is placed
    /// for certain spawn point types of a prefab.
    /// </summary>
    public abstract class PrefabSlaveItem : SingleNodeItem, IItemReferences
    {
        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// The prefab this item is linked to.
        /// </summary>
        public IMapItem PrefabLink { get; set; }

        public PrefabSlaveItem() : base() { }

        internal PrefabSlaveItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Kdop.Flags[31] = true; // always set to true, no idea what it means though
        }

        /// <summary>
        /// Adds a new prefab slave item of type T to the map.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="map">The map.</param>
        /// <param name="parent">The prefab this item is linked to.</param>
        /// <param name="position">The (global) position of the node.</param>
        /// <returns>The newly created prefab slave item.</returns>
        public static T Add<T>(IItemContainer map, Prefab parent, Vector3 position)
            where T : PrefabSlaveItem, new()
        {
            var item = Add<T>(map, position);
            item.PrefabLink = parent;
            parent.SlaveItems.Add(item);
            return item;
        }

        /// <inheritdoc/>
        public override void Move(Vector3 newPos)
        {
            Node.Move(newPos);
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            Node.Move(Node.Position + translation);
        }

        /// <inheritdoc/>
        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            if (PrefabLink is UnresolvedItem 
                && allItems.TryGetValue(PrefabLink.Uid, out var resolvedPrefab))
            {
                PrefabLink = resolvedPrefab;
            }
        }
    }
}
