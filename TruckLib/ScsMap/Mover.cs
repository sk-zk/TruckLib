using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// An animated model which can optionally be moved along a path.
    /// </summary>
    public class Mover : PathItem, IPathItemWithCachedLengths
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Mover;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// Unit name of the model, as defined in <c>/def/world/mover.sii</c>.
        /// </summary>
        public Token Model { get; set; }

        /// <summary>
        /// Look of the model.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// Variant of the model.
        /// </summary>
        public Token Variant { get; set; }

        /// <summary>
        /// Movement speed of the model.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// The duration in seconds a model will pause at the end of the path, in seconds.
        /// </summary>
        public float EndDelay { get; set; }

        /// <summary>
        /// Width of the area along the path in which models will be spawned.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// The amount of models that will appear on the path.
        /// </summary>
        public uint Count { get; set; }

        /// <summary>
        /// Tags.
        /// </summary>
        public List<Token> Tags { get; set; }

        /// <summary>
        /// Cached lengths of the segments.
        /// </summary>
        public List<float> Lengths => Nodes.Lengths;

        /// <summary>
        /// Gets or sets if the mover is active when street lamps are off.
        /// </summary>
        public bool ActiveDuringDay
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        /// <summary>
        /// Gets or sets if the mover is active when street lamps are on.
        /// </summary>
        public bool ActiveDuringNight
        {
            get => Kdop.Flags[2];
            set => Kdop.Flags[2] = value;
        }

        /// <summary>
        /// Gets or sets if models turn around when they reach the end of the path
        /// rather than respawning at the start.
        /// </summary>
        public bool BounceAtEnd
        {
            get => Kdop.Flags[3];
            set => Kdop.Flags[3] = value;
        }

        /// <summary>
        /// Gets or sets if the rotation of the model follows the path.
        /// </summary>
        public bool FollowDir
        {
            get => Kdop.Flags[4];
            set => Kdop.Flags[4] = value;
        }

        /// <summary>
        /// Gets or sets if the item uses a curved path 
        /// rather than a linear path.
        /// </summary>
        public bool UseCurvedPath
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        /// <summary>
        /// Gets or sets if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[5];
            set => Kdop.Flags[5] = value;
        }

        /// <summary>
        /// Gets or sets if sound is enabled.
        /// </summary>
        public bool UseSound
        {
            get => !Kdop.Flags[9];
            set => Kdop.Flags[9] = !value;
        }

        /// <summary>
        /// Gets or sets if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[6];
            set => Kdop.Flags[6] = value;
        }

        /// <summary>
        /// Gets or sets if the mover is active during bad weather.
        /// </summary>
        public bool ActiveDuringBadWeather
        {
            get => Kdop.Flags[11];
            set => Kdop.Flags[11] = value;
        }

        /// <summary>
        /// Gets or sets if the mover is active during nice weather.
        /// </summary>
        public bool ActiveDuringNiceWeather
        {
            get => Kdop.Flags[10];
            set => Kdop.Flags[10] = value;
        }

        public bool PreferNonMovableAnimation
        {
            get => Kdop.Flags[12];
            set => Kdop.Flags[12] = value;
        }

        /// <summary>
        /// Gets or sets if models are evenly spaced.
        /// </summary>
        public bool UniformItemPlacement
        {
            get => Kdop.Flags[13];
            set => Kdop.Flags[13] = value;
        }

        /// <summary>
        /// Gets or sets if models keep ther rotation rather than turn 180°
        /// when <paramref name="BounceAtEnd">BounceAtEnd</paramref> is enabled.
        /// </summary>
        public bool KeepOrientationOnBounce
        {
            get => Kdop.Flags[14];
            set => Kdop.Flags[14] = value;
        }

        public Mover() : base() { }

        internal Mover(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            ActiveDuringDay = true;
            ActiveDuringNight = true;
            ActiveDuringNiceWeather = true;
            ActiveDuringBadWeather = true;
            FollowDir = true;
            UseCurvedPath = true;
            Tags = [];
            Speed = 1;
            Count = 1;
        }

        /// <summary>
        /// Adds a mover to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="positions">The points of the path.</param>
        /// <param name="model">Unit name of the model.</param>
        /// <param name="look">The look of the model.</param>
        /// <param name="variant">The variant of the model.</param>
        /// <returns>The newly created mover.</returns>
        public static Mover Add(IItemContainer map, IList<Vector3> positions,
            Token model, Token look, Token variant)
        {
            var mover = Add<Mover>(map, positions);

            mover.Model = model;
            mover.Look = look;
            mover.Variant = variant;
            mover.Recalculate();

            return mover;
        }

        /// <inheritdoc/>
        public override void Move(Vector3 newPos)
        {
            base.Move(newPos);
            Recalculate(); // TODO does this need to be here?
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);
            Recalculate(); // TODO does this need to be here?
        }
    }
}
