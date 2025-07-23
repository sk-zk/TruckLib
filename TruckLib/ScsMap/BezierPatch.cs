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
    /// A piece of terrain created by a Bézier surface.
    /// </summary>
    public class BezierPatch : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.BezierPatch;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        internal const int ControlPointCols = 4;
        internal const int ControlPointRows = 4;
        private Vector3[,] controlPoints;

        /// <summary>
        /// Control points of the bezier patch, relative to the item node.
        /// </summary>
        public Vector3[,] ControlPoints { 
            get => controlPoints;
            set
            {
                if (value.GetLength(0) != ControlPointCols
                    && value.GetLength(1) != ControlPointRows)
                {
                    throw new ArgumentOutOfRangeException(nameof(ControlPoints),
                        $"ControlPoints must be {ControlPointCols}x{ControlPointRows}.");
                }
                else
                {
                    controlPoints = value;
                }
            }
        }

        internal Tesselation tesselation;
        /// <summary>
        /// <para>The resolution of the terrain quad grid along the X and Z axes.
        /// A value of <i>n</i> will result in in <i>n</i>+1 quads.</para>
        /// <para>Changing this value will modify the <see cref="BezierPatch.QuadData">QuadData</see>.</para>
        /// </summary>
        public Tesselation Tesselation 
        { 
            get => tesselation;
            set
            {
                if (QuadData?.Rows != value.X + 1 || QuadData?.Cols != value.Z + 1)
                {
                    tesselation = value;
                    RecalculateTerrain();
                }
            }
        }

        /// <summary>
        /// Gets or sets the seed for the RNG which determines which vegetation models
        /// to place. The position of the models does not appear to be
        /// affected by this.
        /// </summary>
        public uint RandomSeed { get; set; }

        /// <summary>
        /// The vegetation on this Bézier patch. A maximum of three vegetation types can be used.
        /// </summary>
        public Vegetation[] Vegetation { get; internal set; }

        /// <summary>
        /// The vegetation spheres on this Bézier patch.
        /// </summary>
        public List<VegetationSphere> VegetationSpheres { get; set; }

        /// <summary>
        /// Gets or sets the terrain quad data of this Bézier patch.
        /// </summary>
        public TerrainQuadData QuadData { get; set; }

        private readonly int NoisePowerStart = 2;
        private readonly int NoisePowerLength = 2;

        /// <summary>
        /// Gets or sets the strength of random noise applied to the vertices of the terrain.
        /// </summary>
        public TerrainNoise NoisePower
        {
            get => (TerrainNoise)Kdop.Flags.GetBitString(NoisePowerStart, NoisePowerLength);
            set => Kdop.Flags.SetBitString(NoisePowerStart, NoisePowerLength, (uint)value);
        }

        /// <summary>
        /// Gets or sets if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        /// <summary>
        /// Gets or sets if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        /// <summary>
        /// Gets or sets if vegetation is rendered as flat textures only.
        /// </summary>
        public bool LowPolyVegetation
        {
            get => Kdop.Flags[5];
            set => Kdop.Flags[5] = value;
        }

        /// <summary>
        /// Gets or sets if detail vegetation (small clumps of grass etc.)
        /// is rendered if the selected terrain material supports it.
        /// </summary>
        public bool DetailVegetation
        {
            get => !Kdop.Flags[4];
            set => Kdop.Flags[4] = !value;
        }

        /// <summary>
        /// Gets or sets if collision is enabled.
        /// </summary>
        public bool Collision
        {
            get => !Kdop.Flags[6];
            set => Kdop.Flags[6] = !value;
        }

        /// <summary>
        /// Whether terrain shadows are enabled.
        /// </summary>
        public bool TerrainShadows
        {
            get => !Kdop.Flags[7];
            set => Kdop.Flags[7] = !value;
        }

        /// <summary>
        /// Gets or sets if detail vegetation transitions smoothly in places
        /// where it is affected by brushes.
        /// </summary>
        public bool SmoothDetailVegetation
        {
            get => Kdop.Flags[9];
            set => Kdop.Flags[9] = value;
        }

        /// <summary>
        /// Gets or sets if the player can collide with vegetation.
        /// </summary>
        public bool VegetationCollision
        {
            get => !Kdop.Flags[10];
            set => Kdop.Flags[10] = !value;
        }

        /// <summary>
        /// Gets or sets if vegetation spheres are inverted, only placing vegetation
        /// inside rather than outisde of them.
        /// </summary>
        public bool InvertVegetationSpheres
        {
            get => Kdop.Flags[8];
            set => Kdop.Flags[8] = value;
        }

        public BezierPatch() : base()
        {
            Vegetation = (new Vegetation[3]).Select
                (h => new Vegetation()).ToArray();
        }

        internal BezierPatch(bool initFields) : base(initFields)
        {
            if (initFields) Init();
            Vegetation = (new Vegetation[3]).Select(h => new Vegetation()).ToArray();
        }

        protected override void Init()
        {
            base.Init();
            ControlPoints = new Vector3[ControlPointCols, ControlPointRows];
            tesselation = new(4, 4);
            QuadData = new TerrainQuadData();
            SmoothDetailVegetation = true;
            VegetationCollision = true;
            NoisePower = TerrainNoise.Percent100;
            VegetationSpheres = [];
        }

        /// <summary>
        /// Adds a new Bézier patch to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The center point of the item.</param>
        /// <param name="controlPoints">The control points of the item.</param>
        /// <param name="tesselation">The resolution of the terrain quad grid along the X and Z axes.</param>
        /// <returns>The newly created Bézier patch.</returns>
        public static BezierPatch Add(IItemContainer map, Vector3 position, Vector3[,] controlPoints, Tesselation tesselation)
        {
            if (controlPoints.GetLength(0) != ControlPointCols
                && controlPoints.GetLength(1) != ControlPointRows)
            {
                throw new ArgumentOutOfRangeException(nameof(controlPoints),
                    $"controlPoints must be {ControlPointCols}x{ControlPointRows}.");
            }

            var bezier = Add<BezierPatch>(map, position);

            bezier.ControlPoints = controlPoints;
            bezier.Tesselation = tesselation;

            return bezier;
        }

        /// <summary>
        /// Adds a new Bézier patch to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The center point of the item.</param>
        /// <param name="controlPoints">The control points of the item.</param>
        /// <returns>The newly created Bézier patch.</returns>
        public static BezierPatch Add(IItemContainer map, Vector3 position, Vector3[,] controlPoints)
        {
            return Add(map, position, controlPoints, new(4, 4));
        }

        /// <summary>
        /// Adds a new Bézier patch to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The center point of the item.</param>
        /// <param name="width">Width of the item.</param>
        /// <param name="height">Height of the item.</param>
        /// <param name="tesselation">The resolution of the terrain quad grid along the X and Z axes.</param>
        /// <returns>The newly created Bézier patch.</returns>
        public static BezierPatch Add(IItemContainer map, Vector3 position, float width, float height, Tesselation tesselation)
        {
            var points = CreateControlPointsFromDimensions(width, height);
            return Add(map, position, points, tesselation);
        }

        /// <summary>
        /// Adds a new Bézier patch to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The center point of the item.</param>
        /// <param name="width">Width of the item.</param>
        /// <param name="height">Height of the item.</param>
        /// <returns>The newly created Bézier patch.</returns>
        public static BezierPatch Add(IItemContainer map, Vector3 position, float width, float height)
        {
            var points = CreateControlPointsFromDimensions(width, height);
            return Add(map, position, points, new(4,4));
        }

        private static Vector3[,] CreateControlPointsFromDimensions(float width, float height)
        {
            var points = new Vector3[ControlPointCols, ControlPointRows];

            var thirdWidth = width / 3f;
            var thirdHeight = height / 3f;

            for (int i = 0; i < ControlPointCols; i++)
            {
                for (int j = 0; j < ControlPointRows; j++)
                {
                    var xPos = -width / 2 + j * thirdWidth;
                    var zPos = -height / 2 + i * thirdHeight;
                    points[i, j] = new Vector3(xPos, 0, zPos);
                }
            }

            return points;
        }

        private void RecalculateTerrain()
        {
            QuadData ??= new(true);

            QuadData.Rows = (ushort)(Tesselation.X + 1);
            QuadData.Cols = (ushort)(Tesselation.Z + 1);
            var amount = QuadData.Rows * QuadData.Cols;
            var quads = QuadData.Quads;
            if (quads.Count < amount)
            {
                var missing = amount - quads.Count;
                quads.Capacity += missing;
                for (int i = 0; i < missing; i++)
                {
                    quads.Add(new());
                }
            }
            else
            {
                quads.RemoveRange(amount, quads.Count - amount);
            }
        }
    }

    /// <summary>
    /// Stores the resolution of the terrain quad grid of a Beziér patch item.
    /// </summary>
    /// <param name="X">The amount of terrain quads on the X axis.</param>
    /// <param name="Z">The amount of terrain quads on the Z axis.</param>
    public record struct Tesselation(ushort X, ushort Z);
}
