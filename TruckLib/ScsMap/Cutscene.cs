using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap
{
    public class Cutscene : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Cutscene;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceFar;

        /// <summary>
        /// Tags of the item.
        /// </summary>
        public List<Token> Tags { get; set; }

        public List<CutsceneAction> Actions { get; set; }

        private const int typeStart = 0;
        private const int typeLength = 2;
        public CutsceneType Type
        {
            get => (CutsceneType)Kdop.Flags.GetBitString(typeStart, typeLength);
            set
            {
                Kdop.Flags.SetBitString(typeStart, typeLength, (uint)value);
            }
        }

        /// <summary>
        /// Gets or sets if a viewpoint is only visible in the UI map once discovered.
        /// </summary>
        public bool SecretViewpoint
        {
            get => Kdop.Flags[20];
            set => Kdop.Flags[20] = value;
        }

        public Cutscene() : base() { }

        internal Cutscene(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Tags = new List<Token>();
            Actions = new List<CutsceneAction>();
        }

        /// <summary>
        /// Adds a cutscene item to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the item.</param>
        /// <returns>The newly created cutscene item.</returns>
        public static Cutscene Add(IItemContainer map, Vector3 position)
        {
            var cs = Add<Cutscene>(map, position);
            return cs;
        }

    }
}
