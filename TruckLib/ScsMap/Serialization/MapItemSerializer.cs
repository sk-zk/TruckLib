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

        public static void ReadKdopItem(BinaryReader r, MapItem item)
        {
            item.Kdop = new KdopItem(false);
            item.Kdop.Uid = r.ReadUInt64();
            ReadKdopBounds(r, item);
            item.Kdop.Flags = new FlagField(r.ReadUInt32());
            item.Kdop.ViewDistance = (ushort)((int)r.ReadByte() * viewDistanceFactor);
        }

        public static void ReadKdopBounds(BinaryReader r, MapItem item)
        {
            ReadKdopBounds(r, item.Kdop);
        }

        public static void ReadKdopBounds(BinaryReader r, KdopBounds kb)
        {
            ReadKbArr(r, kb.Minimums);
            ReadKbArr(r, kb.Maximums);

            static void ReadKbArr(BinaryReader r, float[] arr)
            {
                for (int i = 0; i < arr.Length; i++)
                    arr[i] = r.ReadSingle();
            }
        }

        public static void WriteKdopItem(BinaryWriter w, MapItem item)
        {
            w.Write(item.Kdop.Uid);
            WriteKdopBounds(w, item);
            w.Write(item.Kdop.Flags.Bits);
            w.Write((byte)(item.Kdop.ViewDistance / viewDistanceFactor));
        }

        public static void WriteKdopBounds(BinaryWriter w, MapItem item)
        {
            foreach (var arr in new float[][] {
                item.Kdop.Minimums, item.Kdop.Maximums })
            {
                for (int i = 0; i < 5; i++)
                {
                    w.Write(arr[i]);
                }
            }
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

        public static List<INode> ReadNodeRefList(BinaryReader r)
        {
            var count = r.ReadUInt32();
            var list = new List<INode>((int)count);
            for (int i = 0; i < count; i++)
            {
                list.Add(new UnresolvedNode(r.ReadUInt64()));
            }
            return list;
        }

        public static List<IMapItem> ReadItemRefList(BinaryReader r)
        {
            return ReadObjectList<UnresolvedItem>(r).Cast<IMapItem>().ToList();
        }

        public static void WriteNodeRefList(BinaryWriter w, IList<INode> nodeList)
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

        public static void WriteItemRefList(BinaryWriter w, List<IMapItem> itemList)
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
