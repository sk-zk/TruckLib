using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TruckLib.ScsMap.Serialization
{
    abstract class MapItemSerializer
    {
        public abstract MapItem Deserialize(BinaryReader r);

        public abstract void Serialize(BinaryWriter w, MapItem item);

        private const int viewDistanceFactor = 10;

        public static void ReadKdop(BinaryReader r, MapItem item)
        {
            item.KdopItem.Uid = r.ReadUInt64();
            item.KdopItem.BoundingBox.Deserialize(r);
            item.KdopItem.Flags = new BitArray(r.ReadBytes(4));
            item.KdopItem.ViewDistance = (ushort)((int)r.ReadByte() * viewDistanceFactor);
        }

        public static void WriteKdop(BinaryWriter w, MapItem item)
        {
            w.Write(item.KdopItem.Uid);
            item.KdopItem.BoundingBox.Serialize(w);
            w.Write(item.KdopItem.Flags.ToUInt());
            w.Write((byte)(item.KdopItem.ViewDistance / viewDistanceFactor));
        }

        /// <summary>
        /// Reads a list of IBinarySerializable objects or numerical types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r"></param>
        /// <returns></returns>
        public static List<T> ReadObjectList<T>(BinaryReader r) where T : new()
        {
            var count = r.ReadUInt32();
            return r.ReadObjectList<T>(count);
        }

        /// <summary>
        /// Writes a list of IBinarySerializable objects or numerical primitives.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="w"></param>
        /// <param name="list"></param>
        public static void WriteObjectList<T>(BinaryWriter w, List<T> list)
        {
            if (list is null)
            {
                w.Write(0);
                return;
            }

            w.Write((uint)list.Count);
            w.WriteObjectList<T>(list);
        }

        public static List<Node> ReadNodeRefList(BinaryReader r)
        {
            var list = new List<Node>();
            var count = r.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                list.Add(new UnresolvedNode(r.ReadUInt64()));
            }
            return list;
        }

        public static List<MapItem> ReadItemRefList(BinaryReader r)
        {
            return ReadObjectList<UnresolvedItem>(r).Cast<MapItem>().ToList();
        }

        public static void WriteNodeRefList(BinaryWriter w, List<Node> nodeList)
        {
            if (nodeList is null)
            {
                w.Write(0);
                return;
            }

            w.Write(nodeList.Count);
            foreach (var node in nodeList)
            {
                w.Write(node.Uid);
            }
        }

        public static void WriteItemRefList(BinaryWriter w, List<MapItem> itemList)
        {
            if (itemList is null)
            {
                w.Write(0);
                return;
            }

            w.Write(itemList.Count);
            foreach (var item in itemList)
            {
                w.Write(item.Uid);
            }
        }
    }
}
