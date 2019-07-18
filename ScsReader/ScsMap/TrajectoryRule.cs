using System;
using System.Collections.Generic;
using System.IO;

namespace ScsReader.ScsMap
{
    public class TrajectoryRule : IMapSerializable
    {
        public uint NodeIndex;

        public Token Rule;

        public List<float> RuleParameters = new List<float>();

        public override string ToString()
        {
            return $"[{NodeIndex}] {Rule}";
        }

        public void ReadFromStream(BinaryReader r)
        {
            NodeIndex = r.ReadUInt32();
            Rule = r.ReadToken();

            var paramCount = r.ReadUInt32();
            for (var i = 0; i < paramCount; i++)
            {
                RuleParameters.Add(r.ReadSingle());
            }
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(NodeIndex);
            w.Write(Rule);

            w.Write(RuleParameters.Count);
            foreach(var param in RuleParameters)
            {
                w.Write(param);
            }
        }

    }
}
