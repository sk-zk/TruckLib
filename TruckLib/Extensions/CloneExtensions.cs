using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap.Serialization;
using TruckLib.ScsMap;

namespace TruckLib
{
    internal static class CloneExtensions
    {
        /// <summary>
        /// Clones an IBinarySerializable object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to clone.</param>
        /// <returns>The clone.</returns>
        public static T Clone<T>(this T obj) where T : IBinarySerializable, new()
        {
            T cloned = new();
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            obj.Serialize(writer);
            stream.Position = 0;

            using var reader = new BinaryReader(stream);
            cloned.Deserialize(reader);

            return cloned;
        }

        /// <summary>
        /// Clones a MapItem.
        /// </summary>
        /// <typeparam name="T">The type of the MapItem.</typeparam>
        /// <param name="item">The MapItem to clone.</param>
        /// <returns>The clone.</returns>
        public static T CloneItem<T>(this T item) where T : MapItem
        {
            T cloned;

            var serializer = MapItemSerializerFactory.Get(item.ItemType);

            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            serializer.Serialize(writer, item);
            if (serializer is IDataPayload)
            {
                (serializer as IDataPayload).SerializeDataPayload(writer, item);
            }
            stream.Position = 0;

            using var reader = new BinaryReader(stream);
            cloned = (T)serializer.Deserialize(reader);
            if (serializer is IDataPayload)
            {
                (serializer as IDataPayload).DeserializeDataPayload(reader, cloned);
            }

            return cloned;
        }
    }
}
