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

        public Vector3 UnloadOffset { get; set; }

        public IMapItem PrefabLink { get; set; }

        /// <summary>
        /// Determines if the item is actually a train transport.
        /// </summary>
        public bool TrainTransport
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public Ferry() : base() { }

        internal Ferry(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
        }

        public static Ferry Add(IItemContainer map, Vector3 position, Token port)
        {
            var ferry = Add<Ferry>(map, position);
            ferry.Port = port;
            return ferry;
        }

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
