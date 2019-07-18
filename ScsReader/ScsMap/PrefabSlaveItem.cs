using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    public abstract class PrefabSlaveItem : SingleNodeItem, IItemReferences
    {
        // TODO: Figure out the kdop flags for these items

        public override ItemFile DefaultItemFile => ItemFile.Base;

        public MapItem PrefabLink;

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
