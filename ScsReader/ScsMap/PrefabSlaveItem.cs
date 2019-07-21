using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    public abstract class PrefabSlaveItem : SingleNodeItem, IItemReferences
    {
        public override ItemFile DefaultItemFile => ItemFile.Base;

        public MapItem PrefabLink;

        /// <summary>
        /// TEMP FIELD
        /// <para>TODO: Figure out the kdop flags for these items</para>
        /// </summary>
        public new BitArray Flags
        {
            get => base.Flags;
        }

        public PrefabSlaveItem()
        {
            Flags[31] = true; // always set to true, no idea what it means though
        }

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            if (PrefabLink is UnresolvedItem && allItems.ContainsKey(PrefabLink.Uid))
            {
                PrefabLink = allItems[PrefabLink.Uid];
            }
        }
    }
}
