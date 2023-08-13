using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.ScsMap
{
    public class Hookup : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.Hookup;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public string Name { get; set; }

        public bool WaterReflection
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public bool Shadows
        {
            get => !Kdop.Flags[2];
            set => Kdop.Flags[2] = !value;
        }

        public bool MirrorReflection
        {
            get => !Kdop.Flags[3];
            set => Kdop.Flags[3] = !value;
        }

        public bool LowPolyOnly
        {
            get => Kdop.Flags[4];
            set => Kdop.Flags[4] = value;
        }

        public bool Physics
        {
            get => !Kdop.Flags[5];
            set => Kdop.Flags[5] = !value;
        }

        public bool FixedSeed
        {
            get => !Kdop.Flags[6];
            set => Kdop.Flags[6] = !value;
        }

        private const int NodeAlignmentStart = 8;
        private const int NodeAlignmentLength = 2;
        public HookupNodeAlignment NodeAlignment
        {
            get => (HookupNodeAlignment)Kdop.Flags.GetBitString(NodeAlignmentStart, NodeAlignmentLength);
            set => Kdop.Flags.SetBitString(NodeAlignmentStart, NodeAlignmentLength, (uint)value);
        }

        private const int SpawnProbabilityStart = 12;
        private const int SpawnProbabilityLength = 2;
        public HookupSpawnProbability SpawnProbability
        {
            get => (HookupSpawnProbability)Kdop.Flags.GetBitString(SpawnProbabilityStart, SpawnProbabilityLength);
            set => Kdop.Flags.SetBitString(SpawnProbabilityStart, SpawnProbabilityLength, (uint)value);
        }

        private const int ModelDetailStart = 16;
        private const int ModelDetailLength = 2;
        public HookupModelDetail ModelDetail
        {
            get => (HookupModelDetail)Kdop.Flags.GetBitString(ModelDetailStart, ModelDetailLength);
            set => Kdop.Flags.SetBitString(ModelDetailStart, ModelDetailLength, (uint)value);
        }

        public Hookup() : base() { }

        internal Hookup(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
        }
    }
}
