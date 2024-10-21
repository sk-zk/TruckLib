using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Defines a rectangular area in which, if the camera is inside it,
    /// child items are rendered differently.
    /// </summary>
    public class VisibilityArea : SingleNodeItem, IItemReferences
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.VisibilityArea;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        private const int behaviorPos = 0;
        private const int behaviorLength = 2;
        /// <summary>
        /// The rendering behavior of this visibility area.
        /// </summary>
        public VisibilityAreaBehavior Behavior
        {
            get => (VisibilityAreaBehavior)Kdop.Flags.GetBitString(behaviorPos, behaviorLength);
            set => Kdop.Flags.SetBitString(behaviorPos, behaviorLength, (uint)value);
        }

        /// <summary>
        /// Width of the area.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Height of the area.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// The map items which are affected by this item.
        /// </summary>
        public List<IMapItem> Children { get; set; }

        public VisibilityArea() : base() { }

        internal VisibilityArea(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Width = 20;
            Height = 20;
            Children = [];
        }

        /// <inheritdoc/>
        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is UnresolvedItem 
                    && allItems.TryGetValue(Children[i].Uid, out var resolvedItem))
                {
                    Children[i] = resolvedItem;
                }
            }
        }

        /// <summary>
        /// Adds a visibility area to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the center of the area.</param>
        /// <param name="behavior">The rendering behavior of the area.</param>
        /// <param name="width">The width of the area.</param>
        /// <param name="height">The height of the area.</param>
        /// <returns>The newly created visibility area.</returns>
        public static VisibilityArea Add(IItemContainer map, Vector3 position, VisibilityAreaBehavior behavior,
            float width, float height)
        {
            var va = Add<VisibilityArea>(map, position);

            va.Behavior = behavior;
            va.Width = width;
            va.Height = height;

            return va;
        }
    }
}
