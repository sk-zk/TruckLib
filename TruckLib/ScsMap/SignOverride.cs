using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A sign override, which overrides attributes of a sign template object.
    /// </summary>
    public class SignOverride : IBinarySerializable
    {
        /// <summary>
        /// The ID of the sign template object.
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// The name of the sign area the object is placed on, as defined in the sign's model.
        /// </summary>
        public Token AreaName { get; set; }

        /// <summary>
        /// The overridden attributes of this object.
        /// </summary>
        public List<ISignOverrideAttribute> Attributes { get; set; } 
            = new List<ISignOverrideAttribute>();

        /// <inheritdoc/>
        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Id = r.ReadUInt32();
            AreaName = r.ReadToken();

            // attributes
            var attributeCount = r.ReadUInt32();
            for (int i = 0; i < attributeCount; i++)
            {
                var type = (AttributeType)r.ReadUInt16();
                ISignOverrideAttribute attrib;

                switch (type)
                {
                    case AttributeType.SByte:
                        attrib = new SignOverrideAttribute<sbyte>();
                        attrib.Index = r.ReadUInt32();
                        attrib.SetValue(r.ReadSByte());
                        break;
                    case AttributeType.Int32:
                        attrib = new SignOverrideAttribute<int>();
                        attrib.Index = r.ReadUInt32();
                        attrib.SetValue(r.ReadInt32());
                        break;
                    case AttributeType.UInt32:
                        attrib = new SignOverrideAttribute<uint>();
                        attrib.Index = r.ReadUInt32();
                        attrib.SetValue(r.ReadUInt32());
                        break;
                    case AttributeType.Float:
                        attrib = new SignOverrideAttribute<float>();
                        attrib.Index = r.ReadUInt32();
                        attrib.SetValue(r.ReadSingle());
                        break;
                    case AttributeType.String:
                        attrib = new SignOverrideAttribute<string>();
                        attrib.Index = r.ReadUInt32();
                        attrib.SetValue(r.ReadPascalString());
                        break;
                    case AttributeType.UInt64:
                        attrib = new SignOverrideAttribute<ulong>();
                        attrib.Index = r.ReadUInt32();
                        attrib.SetValue(r.ReadUInt64());
                        break;
                    default:
                        throw new NotImplementedException($"Unknown attribute type {type}");
                }

                Attributes.Add(attrib);
            }
        }

        /// <inheritdoc/>
        public void Serialize(BinaryWriter w)
        {
            w.Write(Id);
            w.Write(AreaName);

            w.Write(Attributes.Count);
            foreach (var attrib in Attributes)
            {
                w.Write((ushort)TypeToEnum(attrib.Type));

                w.Write(attrib.Index);

                switch (attrib.GetValue())
                {
                    case sbyte value:
                        w.Write(value);
                        break;
                    case int value:
                        w.Write(value);
                        break;
                    case uint value:
                        w.Write(value);
                        break;
                    case float value:
                        w.Write(value);
                        break;
                    case string value:
                        w.WritePascalString(value);
                        break;
                    case ulong value:
                        w.Write(value);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private AttributeType TypeToEnum(Type type)
        {
            if (type == typeof(sbyte)) return AttributeType.SByte;
            if (type == typeof(int)) return AttributeType.Int32;
            if (type == typeof(uint)) return AttributeType.UInt32;
            if (type == typeof(float)) return AttributeType.Float;
            if (type == typeof(string)) return AttributeType.String;
            if (type == typeof(ulong)) return AttributeType.UInt64;

            throw new NotImplementedException($"No matching enum entry for type {type}");
        }

        private enum AttributeType
        {
            SByte = 1,
            Int32 = 2,
            UInt32 = 3,
            Float = 4,
            String = 5,
            UInt64 = 6
        }

    }
}
