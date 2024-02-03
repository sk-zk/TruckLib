using System;
using System.Collections.Generic;
using System.IO;

namespace TruckLib.ScsMap
{
    public class TrajectoryRule : IBinarySerializable
    {
        public uint NodeIndex { get; set; }

        public Token Rule { get; set; }

        public List<float> RuleParameters { get; set; } = new List<float>();

        public override string ToString()
        {
            return $"[{NodeIndex}] {Rule}";
        }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            NodeIndex = r.ReadUInt32();
            Rule = r.ReadToken();

            var paramCount = r.ReadUInt32();
            for (int i = 0; i < paramCount; i++)
            {
                RuleParameters.Add(r.ReadSingle());
            }
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(NodeIndex);
            w.Write(Rule);

            w.Write(RuleParameters.Count);
            foreach (var param in RuleParameters)
            {
                w.Write(param);
            }
        }

    }
}
