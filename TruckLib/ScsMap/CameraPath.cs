using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.ScsMap
{
    public class CameraPath : PathItem
    {
        public override ItemType ItemType => ItemType.CameraPath;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceFar;

        public List<Token> Tags { get; set; }

        public List<Node> TrackNodes { get; set; }

        public List<Keyframe> Keyframes { get; set; }

        public float CameraSpeed { get; set; }

        public CameraPath() : base() { }

        internal CameraPath(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Tags = new List<Token>();
            TrackNodes = new List<Node>();
            Keyframes = new List<Keyframe>();
            CameraSpeed = 1f;
        }



        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode
                    && allNodes.TryGetValue(Nodes[i].Uid, out var resolvedNode))
                {
                    Nodes[i] = resolvedNode;
                }
            }

            for (int i = 0; i < TrackNodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode
                    && allNodes.TryGetValue(Nodes[i].Uid, out var resolvedNode))
                {
                    Nodes[i] = resolvedNode;
                }
            }
        }
    }
}
