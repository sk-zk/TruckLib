using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    public class MapOverlay : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.MapOverlay;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public Token Look;

        public bool[] HideForZoomLevel
        {
            get => Flags.GetByteAsBools(0);
            // TODO: Allow setting values.
            // Indexer class?
            set => throw new NotImplementedException();
        }

        public byte DlcGuard
        {
            get => Flags.GetByte(1);
            set => Flags.SetByte(1, value);
        }

        public OverlayType Type
        {
            get => (OverlayType)Flags.GetByte(2);
            set => Flags.SetByte(2, (byte)value);
        }

        public static MapOverlay Add(IItemContainer map, Vector3 position, Token look, OverlayType type = OverlayType.RoadName)
        {
            var overlay = Add<MapOverlay>(map, position);

            overlay.Look = look;
            overlay.Type = type;
            /*overlay.HideForZoomLevel = (hideForZoomLevel == null)
                ? new bool[8] : hideForZoomLevel;*/

            return overlay;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Look = r.ReadToken();
            Node = new UnresolvedNode(r.ReadUInt64());
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(Look);
            w.Write(Node.Uid);
        }
    }
}
