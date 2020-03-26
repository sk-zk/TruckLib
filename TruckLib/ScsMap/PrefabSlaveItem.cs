using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public abstract class PrefabSlaveItem : SingleNodeItem, IItemReferences
    {
        public override ItemFile DefaultItemFile => ItemFile.Base;

        public MapItem PrefabLink { get; set; }

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public PrefabSlaveItem()
        {
            Kdop.Flags[31] = true; // always set to true, no idea what it means though
        }

        public static T Add<T>(IItemContainer map, Prefab parent, Vector3 position)
            where T : PrefabSlaveItem, new()
        {
            var item = Add<T>(map, position);
            item.PrefabLink = parent;
            parent.SlaveItems.Add(item);
            return item;
        }

        internal virtual void MoveRel(Vector3 translation)
        {
            Node.Move(Node.Position + translation);
        }

        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            if (PrefabLink is UnresolvedItem && allItems.ContainsKey(PrefabLink.Uid))
            {
                PrefabLink = allItems[PrefabLink.Uid];
            }
        }
    }
}
