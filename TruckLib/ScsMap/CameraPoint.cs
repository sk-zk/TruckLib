using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A camera point, which defines the camera position of various cutscenes
    /// and is also used to create random events.
    /// </summary>
    public class CameraPoint : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.CameraPoint;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<Token> Tags { get; set; }

        public CameraPoint() : base() { }

        internal CameraPoint(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Tags = new List<Token>();
        }

        public static CameraPoint Add(IItemContainer map, Vector3 position, List<Token> tags)
        {
            var point = Add<CameraPoint>(map, position);
            point.Tags = tags;
            return point;
        }
    }
}
