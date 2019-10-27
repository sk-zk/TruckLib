using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class TriggerAction : IBinarySerializable
    {
        /// <summary>
        /// Unit name of the action.
        /// </summary>
        public Token Name { get; set; }

        public List<float> NumParams { get; set; } = new List<float>();

        public List<string> StringParams { get; set; } = new List<string>();

        public List<Token> TargetTags { get; set; } = new List<Token>();

        public float TargetRange { get; set; }

        private const int typeStart = 0;
        private const int typeLength = 4; // presumably
        public ActionType Type 
        {
            get => (ActionType)actionFlags.GetBitString(typeStart, typeLength);
            set
            {
                actionFlags.SetBitString((uint)value, typeStart, typeLength);
            }
        }

        private BitArray actionFlags = new BitArray(32);

        public void ReadFromStream(BinaryReader r)
        {
            // name
            Name = r.ReadToken();

            // num params
            var numParamCount = r.ReadUInt32(); 
            for (int i = 0; i < numParamCount; i++)
            {
                NumParams.Add(r.ReadSingle());
            }

            // string params
            var strParamCount = r.ReadUInt32();
            for (int i = 0; i < strParamCount; i++)
            {
                var strLen = (int)r.ReadUInt64();
                var strBytes = r.ReadBytes(strLen);
                var str = Encoding.Default.GetString(strBytes);
                StringParams.Add(str);
            }

            // target tags
            var targetTagsCount = r.ReadUInt32();
            for (int i = 0; i < targetTagsCount; i++)
            {
                TargetTags.Add(r.ReadToken());
            }

            TargetRange = r.ReadSingle();
            actionFlags = new BitArray(r.ReadBytes(4));
        }

        public void WriteToStream(BinaryWriter w)
        {
            // name
            w.Write(Name);

            w.Write(NumParams.Count);
            foreach (var param in NumParams)
            {
                w.Write(param);
            }

            w.Write(StringParams.Count);
            foreach (var param in StringParams)
            {
                w.Write((ulong)param.Length);
                w.Write(Encoding.Default.GetBytes(param));
            }

            // target tags
            w.Write(TargetTags.Count);
            foreach (var tag in TargetTags)
            {
                w.Write(tag);
            }

            w.Write(TargetRange);
            w.Write(actionFlags.ToUInt());
        }

    }

    public enum ActionType
    {
        Default = 0,
        Condition = 1,
        Fallback = 2,
        Mandatory = 3,
        ConditionRetry = 4
    }
}
