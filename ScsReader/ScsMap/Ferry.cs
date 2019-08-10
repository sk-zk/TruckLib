using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
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

        public override void ReadFromStream(BinaryReader r)
        {
           base.ReadFromStream(r);

            Port = r.ReadToken();
            PrefabLink = new UnresolvedItem(r.ReadUInt64());
            Node = new UnresolvedNode(r.ReadUInt64());
            UnloadOffset = r.ReadVector3();
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(Port);
            w.Write(PrefabLink.Uid);
            w.Write(Node.Uid);
            w.Write(UnloadOffset);
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
