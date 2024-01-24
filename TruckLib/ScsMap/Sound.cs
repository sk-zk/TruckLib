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
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Sound;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Snd;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        ///  Unit name of the sound, as defined in <c>/def/world/sound_item_data.sii</c>.
        /// </summary>
        public Token Name { get; set; }

        /// <summary>
        /// Unit name of the reverb type, as defined in <c>/def/world/sound_item_reverb.sii</c>.
        /// </summary>
        public Token Reverb { get; set; }

        /// <summary>
        /// Width of the area if the sound is an ambient area or reverb area.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Height of the area if the sound is an ambient area or reverb area.
        /// </summary>
        public float Height { get; set; }

        private const int typeStart = 0;
        private const int typeLength = 2;
        /// <summary>
        /// The sound type.
        /// </summary>
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

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Width = 100f;
            Height = 100f;
        }

        /// <summary>
        /// Adds a sound to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The center point of the sound.</param>
        /// <param name="name">The unit name of the sound.</param>
        /// <returns>The newly created sound.</returns>
        public static Sound Add(IItemContainer map, Vector3 position, Token name)
        {
            var sound = Add<Sound>(map, position);

            sound.Name = name;

            return sound;
        }
    }
}
