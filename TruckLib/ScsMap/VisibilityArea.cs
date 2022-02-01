using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.ScsMap
{
    public class VisibilityArea : SingleNodeItem, IItemReferences
    {
        public override ItemType ItemType => ItemType.VisibilityArea;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        private const int behaviorPos = 0;
        private const int behaviorLength = 2;
        public VisibilityAreaBehavior Behavior
        {
            get => (VisibilityAreaBehavior)Kdop.Flags.GetBitString(behaviorPos, behaviorLength);
            set => Kdop.Flags.SetBitString(behaviorPos, behaviorLength, (uint)value);
        }

        public float Width { get; set; }

        public float Height { get; set; }

        public List<IMapItem> Children { get; set; }

        public VisibilityArea() : base() { }

        internal VisibilityArea(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Width = 20;
            Height = 20;
            Children = new List<IMapItem>();
        }

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
    }
}
