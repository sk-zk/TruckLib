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

        public List<Token> Tags { get; set; } = new List<Token>();

        public static CameraPoint Add(IItemContainer map, Vector3 position, List<Token> tags)
        {
            var point = Add<CameraPoint>(map, position);
            point.Tags = tags;
            return point;
        }
    }
}
