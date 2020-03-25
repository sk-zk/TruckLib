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
    public class Ferry : SingleNodeItem, IItemReferences
    {
        public override ItemType ItemType => ItemType.Ferry;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public Token Port { get; set; }

        public Vector3 UnloadOffset { get; set; } = Vector3.Zero;

        public MapItem PrefabLink { get; set; }

        /// <summary>
        /// Determines if the item is actually a train transport.
        /// </summary>
        public bool TrainTransport
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        public static Ferry Add(IItemContainer map, Vector3 position, Token port)
        {
            var ferry = Add<Ferry>(map, position);
            ferry.Port = port;
            return ferry;
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
