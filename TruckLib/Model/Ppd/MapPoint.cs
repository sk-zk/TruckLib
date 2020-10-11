using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Model.Ppd
{
    /// <summary>
    /// Represents a map point which is used to draw a representation 
    /// of the item to the UI map.
    /// </summary>
    public class MapPoint : IBinarySerializable
    {
        public Vector3 Position { get; set; }

        public int[] Neighbours { get; set; } = new int[6];

        protected FlagField VisFlags = new FlagField();

        // TODO: Figure out Assigned Node / Destination Nodes
        public FlagField NavFlags = new FlagField();

        /// <summary>
        /// Mark the approximate location of the prefab exit. 
        /// (useful for company prefabs where navigation will navigate from/to this point.)
        /// </summary>
        public bool PrefabExit
        {
            get => NavFlags[10];
            set => NavFlags[10] = value;
        }

        /// <summary>
        /// The type of road this map point should visualize. 
        /// <para>If set to Polygon, this map point will be used for visualizing polygons
        /// instead of a road and has to be connected to three other points to form a quad.</para>
        /// </summary>
        public RoadSize RoadSize
        {
            get => (RoadSize)VisFlags.GetBitString(8, 4);
            set => VisFlags.SetBitString(8, 4, (uint)value);
        }

        /// <summary>
        /// The distance between road lanes.
        /// </summary>
        public RoadOffset RoadOffset
        {
            get => (RoadOffset)VisFlags.GetBitString(12, 3);
            set => VisFlags.SetBitString(12, 3, (uint)value);
        }

        /// <summary>
        /// Color of the polygon if RoadSize is set to Polygon.
        /// </summary>
        public CustomColor Color
        {
            get => (CustomColor)VisFlags.GetBitString(17, 4);
            set => VisFlags.SetBitString(17, 4, (uint)value);
        }

        /// <summary>
        /// TODO: What is this?
        /// </summary>
        public byte ExtValue
        {
            get => VisFlags.GetByte(0);
            set => VisFlags.SetByte(0, value);
        }

        /// <summary>
        /// TODO: What is this?
        /// </summary>
        public bool RoadOffsetLane
        {
            get => VisFlags[16];
            set => VisFlags[16] = value;
        }

        /// <summary>
        /// Determines if this map point will be drawn on top of all map points without this flag.
        /// </summary>
        public bool RoadOver
        {
            get => VisFlags[17];
            set => VisFlags[17] = value;
        }

        /// <summary>
        /// Property marking no outline drawing. This might be useful for buildings drawing. 
        /// </summary>
        public bool Outline
        {
            get => !VisFlags[21];
            set => VisFlags[21] = !value;
        }

        /// <summary>
        /// Determines if the UI map will display green arrows or not.
        /// <para>This is useful in the case prefabs are using more that 2 control nodes 
        /// and paths for navigation are still clear.</para>
        /// </summary>
        public bool Arrow
        {
            get => !VisFlags[22];
            set => VisFlags[22] = !value;
        }

        public void Deserialize(BinaryReader r)
        {
            VisFlags = new FlagField(r.ReadUInt32());
            NavFlags = new FlagField(r.ReadUInt32());

            Position = r.ReadVector3();
            for (int i = 0; i < Neighbours.Length; i++)
            {
                Neighbours[i] = r.ReadInt32();
            }

            // TODO: I think we can ignore this?
            var neighbourCount = r.ReadUInt32(); 
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(VisFlags.Bits);
            w.Write(NavFlags.Bits);
            w.Write(Position);

            foreach(var neighbour in Neighbours)
            {
                w.Write(neighbour);
            }

            //neighbour_count
            const int nullValue = -1;
            w.Write(Neighbours.Count(x => x != nullValue));
        }
    }
}
