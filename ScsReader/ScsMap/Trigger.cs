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
    public class Trigger : PolygonItem
    {
        public override ItemType ItemType => ItemType.Trigger;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<Token> Tags { get; set; } = new List<Token>();

        public List<TriggerAction> Actions { get; set; } = new List<TriggerAction>();

        public float Range { get; set; } = 1f;

        public float ResetDelay { get; set; }

        public float ResetDistance { get; set; }

        public float ActivationSpeedFrom { get; set; }

        public float ActivationSpeedTo { get; set; }

        private BitArray TriggerFlags = new BitArray(32);

        public byte DlcGuard
        {
            get => Flags.GetByte(1);
            set => Flags.SetByte(1, value);
        }

        public bool SphereArea
        {
            get => TriggerFlags[1];
            set => TriggerFlags[1] = value;
        }

        public bool PartialActivation
        {
            get => TriggerFlags[2];
            set => TriggerFlags[2] = value;
        }

        public bool ManualActivation
        {
            get => TriggerFlags[0];
            set => TriggerFlags[0] = value;
        }

        public bool SpeedActivation
        {
            get => TriggerFlags[6];
            set => TriggerFlags[6] = value;
        }

        public bool OneTimeActivation
        {
            get => TriggerFlags[3];
            set => TriggerFlags[3] = value;
        }

        public bool OrientedActivation
        {
            get => TriggerFlags[5];
            set => TriggerFlags[5] = value;
        }

        public static Trigger Add(IItemContainer map, Vector3[] nodePositions, List<TriggerAction> actions)
        {
            var trigger = Add<Trigger>(map, nodePositions);
            trigger.Actions = actions;
            return trigger;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Tags = ReadObjectList<Token>(r);
            Nodes = ReadNodeRefList(r);
            Actions = ReadObjectList<TriggerAction>(r);
            Range = r.ReadSingle();
            ResetDelay = r.ReadSingle();
            ResetDistance = r.ReadSingle();
            ActivationSpeedFrom = r.ReadSingle() * 3.6f;
            ActivationSpeedTo = r.ReadSingle() * 3.6f;
            TriggerFlags = new BitArray(BitConverter.GetBytes(r.ReadUInt32()));
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            WriteObjectList(w, Tags);
            WriteNodeRefList(w, Nodes);
            WriteObjectList(w, Actions);
            w.Write(Range);
            w.Write(ResetDelay);
            w.Write(ResetDistance);
            w.Write(ActivationSpeedFrom / 3.6f);
            w.Write(ActivationSpeedTo / 3.6f);
            w.Write(TriggerFlags.ToUInt());
        }
    }
}
