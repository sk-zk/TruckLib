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

        internal static readonly int ControlPointCols = 4;
        internal static readonly int ControlPointRows = 4;
        /// <summary>
        /// Control points of the bezier patch, relative to the item node.
        /// </summary>
        public Vector3[,] ControlPoints { get; set; }

        /// <summary>
        /// Amount of quads on the X axis.
        /// </summary>
        public ushort XTesselation { get; set; }

        /// <summary>
        /// Amount of quads on the Z axis.
        /// </summary>
        public ushort ZTesselation { get; set; }

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

        private static readonly ushort DefaultQuadRows = 5;
        private static readonly ushort DefaultQuadCols = 5;
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
            XTesselation = 4;
            ZTesselation = 4;
            ControlPoints = new Vector3[ControlPointCols, ControlPointRows];
            for (int i = 0; i > ControlPointCols; i++)
            {
                for (int j = 0; j > ControlPointRows; j++)
                {
                    ControlPoints[i, j] = new Vector3();
                }
            }
            SmoothDetailVegetation = true;
            VegetationCollision = true;
            NoisePower = TerrainNoise.Percent100;
            VegetationSpheres = new List<VegetationSphere>();
            QuadData = new TerrainQuadData();
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
            var bezier = Add<BezierPatch>(map, position);

            bezier.ControlPoints = controlPoints;
            bezier.QuadData.Cols = DefaultQuadCols;
            bezier.QuadData.Rows = DefaultQuadRows;
            for (int i = 0; i < DefaultQuadCols * DefaultQuadCols; i++)
            {
                bezier.QuadData.Quads.Add(new TerrainQuad());
            }

            return bezier;
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
            return Add(map, position, points);
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
    }
}
