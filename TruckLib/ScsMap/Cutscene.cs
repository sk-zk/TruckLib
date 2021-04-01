using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap
{
    public class Cutscene : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.Cutscene;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceFar;

        public List<Token> Tags { get; set; }

        public List<CutsceneAction> Actions { get; set; }

        private const int typeStart = 0;
        private const int typeLength = 1;
        public CutsceneType Type
        {
            get => (CutsceneType)Kdop.Flags.GetBitString(typeStart, typeLength);
            set
            {
                Kdop.Flags.SetBitString(typeStart, typeLength, (uint)value);
            }
        }

        public Cutscene() : base() { }

        internal Cutscene(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Tags = new List<Token>();
            Actions = new List<CutsceneAction>();
        }

        public static Cutscene Add(IItemContainer map, Vector3 position)
        {
            var cs = Add<Cutscene>(map, position);
            return cs;
        }

    }
}
