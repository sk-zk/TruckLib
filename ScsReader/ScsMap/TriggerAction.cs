using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    public class TriggerAction : IBinarySerializable
    {
        /// <summary>
        /// Unit name of the action.
        /// </summary>
        public Token Name;

        /// <summary>
        /// Custom parameters. Only saved if OverrideParameters is set to true.
        /// </summary>
        public List<float> NumParams = new List<float>();

        /// <summary>
        /// Custom parameters. Only saved if OverrideParameters is set to true.
        /// </summary>
        public List<string> StringParams = new List<string>();

        public bool OverrideParameters;

        public ulong TargetId;

        public List<Token> TargetTags = new List<Token>();

        public void ReadFromStream(BinaryReader r)
        {
            // name
            Name = r.ReadToken();

            // num params
            // MaxValue = default params
            // everything else means overwritten params
            var numParamCount = r.ReadUInt32();
            if (numParamCount == uint.MaxValue)
            {
                OverrideParameters = false;
            }
            else
            {
                OverrideParameters = true;
                if (numParamCount != uint.MaxValue)
                {
                    for (int i = 0; i < numParamCount; i++)
                    {
                        NumParams.Add(r.ReadSingle());
                    }
                }
            }
            
            // string params
            var strParamCount = r.ReadUInt32();
            if (OverrideParameters)
            {
                for (int i = 0; i < strParamCount; i++)
                {
                    var strLen = (int)r.ReadUInt64();
                    var strBytes = r.ReadBytes(strLen);
                    var str = Encoding.Default.GetString(strBytes);
                    StringParams.Add(str);

                }
            }

            // target ID
            TargetId = r.ReadUInt64();

            // target tags
            var targetTagsCount = r.ReadUInt32();
            for (int i = 0; i < targetTagsCount; i++)
            {
                TargetTags.Add(r.ReadToken());
            }
        }

        public void WriteToStream(BinaryWriter w)
        {
            // name
            w.Write(Name);

            // num params
            if (OverrideParameters)
            {
                w.Write(NumParams.Count);
                foreach(var param in NumParams)
                {
                    w.Write(param);
                }
            }
            else
            {
                w.Write(uint.MaxValue);
            }

            // string params
            if (OverrideParameters)
            {
                w.Write(StringParams.Count);
                foreach (var param in StringParams)
                {
                    w.Write((ulong)param.Length);
                    w.Write(Encoding.Default.GetBytes(param));
                }
            }
            else
            {
                w.Write(uint.MaxValue);
            }

            // target ID
            w.Write(TargetId);

            // target tags
            w.Write(TargetTags.Count);
            foreach (var tag in TargetTags)
            {
                w.Write(tag);
            }
        }

    }
}
