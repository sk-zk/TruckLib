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

        public List<INode> TrackNodes { get; set; }

        public List<INode> CurveControlNodes { get; set; }

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
            TrackNodes = new List<INode>();
            Keyframes = new List<Keyframe>();
            CameraSpeed = 1f;
        }

        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);
            ResolveNodeReferences(TrackNodes, allNodes);
        }
    }
}
