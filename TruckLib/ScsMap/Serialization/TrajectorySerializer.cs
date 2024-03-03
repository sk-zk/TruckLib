using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap.Serialization
{
    class TrajectorySerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var trj = new Trajectory(false);
            ReadKdopItem(r, trj);

            trj.Nodes = new PathNodeList(trj);
            trj.Nodes.AddRange(ReadNodeRefList(r), false);
            trj.AccessRule = r.ReadToken();
            trj.Rules = ReadObjectList<TrajectoryRule>(r);
            
            var checkpointCount = r.ReadUInt32();
            trj.Checkpoints = new List<TrajectoryCheckpoint>((int)checkpointCount);
            for (int i = 0; i < checkpointCount; i++)
            {
                var checkpoint = new TrajectoryCheckpoint
                {
                    Route = r.ReadToken(),
                    Checkpoint = r.ReadToken()
                };
                trj.Checkpoints.Add(checkpoint);
            }

            trj.Tags = ReadObjectList<Token>(r);

            return trj;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var trj = item as Trajectory;
            WriteKdopItem(w, trj);

            WriteNodeRefList(w, trj.Nodes);
            w.Write(trj.AccessRule);
            WriteObjectList(w, trj.Rules);

            w.Write(trj.Checkpoints.Count);
            foreach (var checkpoint in trj.Checkpoints)
            {
                w.Write(checkpoint.Route);
                w.Write(checkpoint.Checkpoint);
            }

            WriteObjectList(w, trj.Tags);
        }
    }
}
