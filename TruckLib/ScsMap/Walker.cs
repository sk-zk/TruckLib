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
    /// Obsolete item for making pedestrians move along a path.
    /// </summary>
    /// <remarks>
    /// Walkers have been deprecated in 1.36 and replaced with Movers,
    /// but, as of 1.49, have not yet been removed from the game, despite no longer
    /// being used in europe.mbd.
    /// </remarks>
    [Obsolete]
    public class Walker : PathItem, IRecalculatable
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Walker;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => 120;

        public Token NamePrefix { get; set; } 

        public float Speed { get; set; }

        public float EndDelay { get; set; } 

        public uint Count { get; set; } 

        public float Width { get; set; } 

        public float Angle { get; set; } 

        /// <summary>
        /// Cached lengths of the segments.
        /// </summary>
        public List<float> Lengths { get; set; }

        public bool UseCurvedPath
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public bool ActiveDuringDay
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public bool ActiveDuringNight
        {
            get => Kdop.Flags[2];
            set => Kdop.Flags[2] = value;
        }

        public bool BounceAtEnd
        {
            get => Kdop.Flags[3];
            set => Kdop.Flags[3] = value;
        }

        public bool FollowDir
        {
            get => Kdop.Flags[4];
            set => Kdop.Flags[4] = value;
        }

        public bool ActiveDuringNiceWeather
        {
            get => !Kdop.Flags[5];
            set => Kdop.Flags[5] = !value;
        }

        public bool ActiveDuringBadWeather
        {
            get => !Kdop.Flags[6];
            set => Kdop.Flags[6] = !value;
        }

        public bool RandomSpeedFactor
        {
            get => !Kdop.Flags[7];
            set => Kdop.Flags[7] = !value;
        }

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        public Walker() : base() { }

        internal Walker(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Lengths = new List<float>();
            NamePrefix = "walker_";
            Speed = 1f;
            EndDelay = 0f;
            Count = 1;
            Width = 2f;
            Angle = 0f;
        }

        /// <summary>
        /// Adds a walker to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="positions">The points of the path.</param>
        /// <param name="namePrefix">The name prefix.</param>
        /// <returns>The newly created walker.</returns>
        public static Walker Add(IItemContainer map, IList<Vector3> positions, Token namePrefix)
        {
            var walker = Add<Walker>(map, positions);

            walker.NamePrefix = namePrefix;
            walker.Recalculate();

            return walker;
        }

        /// <inheritdoc/>
        public override void Move(Vector3 newPos)
        {
            base.Move(newPos);
            Recalculate();
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);
            Recalculate();
        }

        /// <inheritdoc/>
        public void Recalculate()
        {
            Lengths = MapItemUtils.CalculatePathLengths(Nodes, UseCurvedPath);
        }
    }
}
