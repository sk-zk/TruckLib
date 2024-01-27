using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// TODO: What even is this?
    /// </summary>
    /// <remarks>Removed from the editor item menu, but it's still in the game and the
    /// item properties dialog still works.</remarks>
    [Obsolete]
    public class AnimatedModel : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.AnimatedModel;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<Token> Tags { get; set; }

        /// <summary>
        /// Unit name of the model.
        /// </summary>
        public Token Model { get; set; }
     
        public bool InitHidden
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public AnimatedModel() : base() { }

        internal AnimatedModel(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Tags = new List<Token>();
        }

        /// <summary>
        /// Adds an AnimatedModel to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the item.</param>
        /// <param name="model">The unit name of the model.</param>
        /// <param name="tags">A list of tags,</param>
        /// <returns>The newly created AnimatedModel.</returns>
        public static AnimatedModel Add(IItemContainer map, Vector3 position, Token model,
            List<Token> tags = null)
        {
            var anim = Add<AnimatedModel>(map, position);

            anim.Tags = tags;
            anim.Model = model;

            return anim;
        }

    }
}
