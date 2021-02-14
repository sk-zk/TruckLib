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

        public override ItemFile DefaultItemFile => ItemFile.Snd;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public Token Name { get; set; }

        public Token Reverb { get; set; }

        public float AreaWidth { get; set; }

        public float AreaHeight { get; set; }

        private const int typeStart = 0;
        private const int typeLength = 2;
        public SoundType Type
        {
            get => (SoundType)Kdop.Flags.GetBitString(typeStart, typeLength);
            set
            {
                Kdop.Flags.SetBitString(typeStart, typeLength, (uint)value);
            }
        }

        public Sound() : base() { }

        internal Sound(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            AreaWidth = 100f;
            AreaHeight = 100f;
        }

        public static Sound Add(IItemContainer map, Vector3 position, Token name)
        {
            var sound = Add<Sound>(map, position);

            sound.Name = name;

            return sound;
        }
    }
}
