using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for <see cref="CutsceneAction"/> and <see cref="TriggerAction"/>.
    /// </summary>
    public abstract class ActionBase : IBinarySerializable
    {
        public List<float> NumParams { get; set; } = new();

        public List<string> StringParams { get; set; } = new();

        public List<Token> TargetTags { get; set; } = new();

        public float TargetRange { get; set; }

        protected FlagField ActionFlags = new();

        private const uint NoParamsMarker = uint.MaxValue;

        public virtual void Deserialize(BinaryReader r, uint? version = null)
        {
            var numParamCount = r.ReadUInt32();
            // if there are no custom params of any kind,
            // there's just an 0xFFFFFFFF here and the item ends.
            if (numParamCount == NoParamsMarker)
                return;

            for (int i = 0; i < numParamCount; i++)
            {
                NumParams.Add(r.ReadSingle());
            }

            var stringParamCount = r.ReadUInt32();
            for (int i = 0; i < stringParamCount; i++)
            {
                var strLen = (int)r.ReadUInt64();
                var strBytes = r.ReadBytes(strLen);
                var str = Encoding.Default.GetString(strBytes);
                StringParams.Add(str);
            }

            var targetTagsCount = r.ReadUInt32();
            for (int i = 0; i < targetTagsCount; i++)
            {
                TargetTags.Add(r.ReadToken());
            }

            TargetRange = r.ReadSingle();
            ActionFlags = new FlagField(r.ReadUInt32());
        }

        public virtual void Serialize(BinaryWriter w)
        {
            if (HasNoCustomParams())
            {
                w.Write(NoParamsMarker);
                return;
            }

            w.Write(NumParams.Count);
            foreach (var param in NumParams)
            {
                w.Write(param);
            }

            w.Write(StringParams.Count);
            foreach (var param in StringParams)
            {
                w.WritePascalString(param);
            }

            w.Write(TargetTags.Count);
            foreach (var tag in TargetTags)
            {
                w.Write(tag);
            }

            w.Write(TargetRange);
            w.Write(ActionFlags.Bits);
        }

        private bool HasNoCustomParams() => 
            NumParams.Count == 0 
            && StringParams.Count == 0 
            && TargetTags.Count == 0;
    }
}
