using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class MapOverlay : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.MapOverlay;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public Token Look { get; set; }

        public bool[] HideForZoomLevel
        {
            get => throw new NotImplementedException(); //Kdop.Flags.GetByteAsBools(0);
            // TODO: Allow setting values.
            set => throw new NotImplementedException();
        }

        public byte DlcGuard
        {
            get => Kdop.Flags.GetByte(1);
            set => Kdop.Flags.SetByte(1, value);
        }

        public OverlayType Type
        {
            get => (OverlayType)Kdop.Flags.GetByte(2);
            set => Kdop.Flags.SetByte(2, (byte)value);
        }

        public MapOverlay() : base() { }

        internal MapOverlay(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
        }

        public static MapOverlay Add(IItemContainer map, Vector3 position, Token look,
            OverlayType type = OverlayType.RoadName)
        {
            var overlay = Add<MapOverlay>(map, position);

            overlay.Look = look;
            overlay.Type = type;
            /*overlay.HideForZoomLevel = (hideForZoomLevel == null)
                ? new bool[8] : hideForZoomLevel;*/

            return overlay;
        }
    }
}
