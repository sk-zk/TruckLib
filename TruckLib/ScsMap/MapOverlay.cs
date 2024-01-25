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
    /// <summary>
    /// Overlays an image or text onto the UI map.
    /// </summary>
    public class MapOverlay : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.MapOverlay;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Unit name of the image or city.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// Determines which zoom levels the overlay will be visible for. (Not yet implemented;
        /// will throw <see cref="NotImplementedException"/>.)
        /// </summary>
        public bool[] HideForZoomLevel
        {
            // TODO
            get => throw new NotImplementedException(); //Kdop.Flags.GetByteAsBools(0);
            set => throw new NotImplementedException();
        }

        public byte DlcGuard
        {
            get => Kdop.Flags.GetByte(1);
            set => Kdop.Flags.SetByte(1, value);
        }

        /// <summary>
        /// The overlay type.
        /// </summary>
        public OverlayType Type
        {
            get => (OverlayType)Kdop.Flags.GetByte(0);
            set => Kdop.Flags.SetByte(0, (byte)value);
        }

        /// <summary>
        /// Gets or sets if this overlay is only visible in the UI map once discovered.
        /// </summary>
        public bool Secret
        {
            get => Kdop.Flags[16];
            set => Kdop.Flags[16] = value;
        }

        public MapOverlay() : base() { }

        internal MapOverlay(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Adds a map overlay to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">Position of the center of the overlay.</param>
        /// <param name="type">The overlay type.</param>
        /// <returns>The newly created map overlay.</returns>
        public static MapOverlay Add(IItemContainer map, Vector3 position, OverlayType type)
        {
            var overlay = Add<MapOverlay>(map, position);

            overlay.Type = type;

            return overlay;
        }
    }
}
