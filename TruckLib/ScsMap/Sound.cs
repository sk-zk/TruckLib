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
    /// A sound which is played when the player is in range.
    /// </summary>
    public class Sound : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.Sound;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public Token Name { get; set; }

        public float FullIntensityDistance { get; set; }

        public float ActivationDistance { get; set; }

        private BitArray SoundFlags { get; set; } = new BitArray(32);

        public bool Stream
        {
            get => SoundFlags[2];
            set => SoundFlags[2] = value;
        }
        public bool ThreeDimensional
        {
            get => SoundFlags[0];
            set => SoundFlags[0] = value;
        }

        public bool Looped
        {
            get => SoundFlags[1];
            set => SoundFlags[1] = value;
        }

        public Sound()
        {
            Looped = true;
            ThreeDimensional = true;
        }

        public static Sound Add(IItemContainer map, Vector3 position, Token name, float fullIntensityDistance, 
            float activationDistance)
        {
            var sound = Add<Sound>(map, position);

            sound.Name = name;
            sound.FullIntensityDistance = fullIntensityDistance;
            sound.ActivationDistance = activationDistance;

            return sound;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Name = r.ReadToken();
            FullIntensityDistance = r.ReadSingle();
            ActivationDistance = r.ReadSingle();

            SoundFlags = new BitArray(r.ReadBytes(4));

            Node = new UnresolvedNode(r.ReadUInt64());
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(Name);
            w.Write(FullIntensityDistance);
            w.Write(ActivationDistance);

            w.Write(SoundFlags.ToUInt());

            w.Write(Node.Uid);
        }
    }
}
