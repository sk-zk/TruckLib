using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// A building segment.
    /// </summary>
    public class Building : PolylineItem
    {
        public override ItemType ItemType => ItemType.Building;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The unit name of the building.
        /// </summary>
        public Token BuildingName { get; set; }

        /// <summary>
        /// The building look.
        /// </summary>
        public Token Look { get; set; }

        public byte DlcGuard
        {
            get => Flags.GetByte(1);
            set => Flags.SetByte(1, value);
        }

        /// <summary>
        /// The seed for this building.
        /// </summary>
        public uint RandomSeed { get; set; } = 1;

        /// <summary>
        /// Stretch coefficient.
        /// </summary>
        public float Stretch { get; set; } = 1f;

        /// <summary>
        /// Height offsets for individual elements of the building.
        /// </summary>
        /// <remarks>
        /// This feature exists in the format and works ingame, but I have not
        /// been able to find a way to apply it in the official editor.
        /// </remarks>
        public List<float> HeightOffsets { get; set; } = new List<float>();

        public bool Collision
        {
            get => !Flags[3];
            set => Flags[3] = !value;
        }

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Flags[2];
            set => Flags[2] = value;
        }

        /// <summary>
        /// Determines if the item is reflected in water.
        /// </summary>
        public bool WaterReflection
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// TODO: What is this?
        /// </summary>
        public bool Acc
        {
            get => !Flags[0];
            set => Flags[0] = !value;
        }

        /// <summary>
        /// Determines if this item casts shadows.
        /// </summary>
        public bool Shadows
        {
            get => !Flags[4];
            set => Flags[4] = !value;
        }

        /// <summary>
        /// Determines if this item is visible in mirrors.
        /// </summary>
        public bool MirrorReflection
        {
            get => !Flags[5];
            set => Flags[5] = !value;
        }

        /// <summary>
        /// Adds a single building segment to the map.
        /// </summary>
        /// <param name="name">Unit name of the building.</param>
        /// <param name="look">The look.</param>
        /// <param name="backwardPos">The position of the backward (red) node.</param>
        /// <param name="forwardPos">The position of the forward (green) node.</param>
        /// <returns>The newly created building.</returns>
        public static Building Add(IItemContainer map, Vector3 backwardPos, Vector3 forwardPos, Token name, Token look)
        {
            var building = Add<Building>(map, backwardPos, forwardPos);
            building.InitFromAddOrAppend(name, look, backwardPos, forwardPos);
            return building;
        }

        /// <summary>
        /// Initializes a building which has been created via Add or Append.
        /// </summary>
        private void InitFromAddOrAppend(Token name, Token look, Vector3 backwardPos, Vector3 forwardPos)
        {
            BuildingName = name;
            Look = look;
            Length = Vector3.Distance(backwardPos, forwardPos);
        }

        /// <summary>
        /// Appends a building segment to this building. 
        /// </summary>
        /// <param name="position">The position of the ForwardNode of the new building.</param>
        /// <returns>The newly created building.</returns>
        public Building Append(Vector3 position)
        {
            return Append(position, BuildingName, Look);
        }

        /// <summary>
        /// Appends a building segment to this building. 
        /// </summary>
        /// <param name="position">The position of the ForwardNode of the new building.</param>
        /// <param name="name">The unit name.</param>
        /// <param name="look">The look.</param>
        /// <returns>The newly created building.</returns>
        public Building Append(Vector3 position, Token name, Token look)
        {
            var building = Append<Building>(position);
            building.InitFromAddOrAppend(name, look, ForwardNode.Position, position);
            return building;
        }

        /// <summary>
        /// Reads the building.
        /// </summary>
        /// <param name="r">The reader.</param>
        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            BuildingName = r.ReadToken();
            Look = r.ReadToken();

            Node = new UnresolvedNode(r.ReadUInt64());
            ForwardNode = new UnresolvedNode(r.ReadUInt64());

            Length = r.ReadSingle();
            RandomSeed = r.ReadUInt32();
            Stretch = r.ReadSingle();
            HeightOffsets = ReadObjectList<float>(r);
        }

        /// <summary>
        /// Writes the building.
        /// </summary>
        /// <param name="w">The writer.</param>
        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(BuildingName);
            w.Write(Look);

            w.Write(Node.Uid);
            w.Write(ForwardNode.Uid);

            w.Write(Length);
            w.Write(RandomSeed);
            w.Write(Stretch);
            WriteObjectList(w, HeightOffsets);
        }
    }
}
