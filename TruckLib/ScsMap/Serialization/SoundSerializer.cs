﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class SoundSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var sound = new Sound(false);
            ReadKdopItem(r, sound);

            sound.Name = r.ReadToken();
            sound.FullIntensityDistance = r.ReadSingle();
            sound.ActivationDistance = r.ReadSingle();      
            sound.SoundFlags = new FlagField(r.ReadUInt32());
            sound.Node = new UnresolvedNode(r.ReadUInt64());

            return sound;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var sound = item as Sound;
            WriteKdopItem(w, sound);
            w.Write(sound.Name);
            w.Write(sound.FullIntensityDistance);
            w.Write(sound.ActivationDistance);
            w.Write(sound.SoundFlags.Bits);
            w.Write(sound.Node.Uid);
        }
    }
}
