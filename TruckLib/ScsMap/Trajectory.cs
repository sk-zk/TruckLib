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
    /// Used for Special Transport DLC. TODO: Figure out how this works
    /// </summary>
    public class Trajectory : MapItem
    {
        public override ItemType ItemType => ItemType.Trajectory;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<Node> Nodes { get; set; } = new List<Node>();

        public Token AccessRule { get; set; } = "_tj_default";

        public List<TrajectoryRule> Rules { get; set; } 
            = new List<TrajectoryRule>();

        public List<TrajectoryCheckpoint> Checkpoints { get; set; } 
            = new List<TrajectoryCheckpoint>();

        public List<Token> Tags { get; set; } = new List<Token>();

        private BitArray NavigationFlags = new BitArray(32);

        public bool ForceThisWay
        {
            get => NavigationFlags[0];
            set => NavigationFlags[0] = value;
        }

        public bool LowProbability
        {
            get => NavigationFlags[1];
            set => NavigationFlags[1] = value;
        }

        public bool LimitDisplacement
        {
            get => NavigationFlags[3];
            set => NavigationFlags[3] = value;
        }

        public bool Public
        {
            get => NavigationFlags[12];
            set => NavigationFlags[12] = value;
        }

        public bool ParkingSpot
        {
            get => NavigationFlags[13];
            set => NavigationFlags[13] = value;
        }

        public bool OneWayBonus
        {
            get => NavigationFlags[14];
            set => NavigationFlags[14] = value;
        }

        internal override IEnumerable<Node> GetItemNodes()
        {
            return new List<Node>(Nodes);
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Nodes = ReadNodeRefList(r);
            NavigationFlags = new BitArray(r.ReadBytes(4));
            AccessRule = r.ReadToken();
            Rules = ReadObjectList<TrajectoryRule>(r);

            // checkpoints
            var checkpointCount = r.ReadUInt32();
            for (int i = 0; i < checkpointCount; i++)
            {
                var checkpoint = new TrajectoryCheckpoint
                {
                    Route = r.ReadToken(),
                    Checkpoint = r.ReadToken()
                };
                Checkpoints.Add(checkpoint);
            }

            Tags = ReadObjectList<Token>(r);
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            WriteNodeRefList(w, Nodes);
            w.Write(NavigationFlags.ToUInt());
            w.Write(AccessRule);
            WriteObjectList(w, Rules);

            w.Write(Checkpoints.Count);
            foreach(var checkpoint in Checkpoints)
            {
                w.Write(checkpoint.Route);
                w.Write(checkpoint.Checkpoint);
            }

            WriteObjectList(w, Tags);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode && allNodes.ContainsKey(Nodes[i].Uid))
                {
                    Nodes[i] = allNodes[Nodes[i].Uid];
                }
            }
        }
    }
}
