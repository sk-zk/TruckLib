using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Models.Ppd
{
    /// <summary>
    /// Represents a navigation curve, used to define AI traffic paths and GPS navigation.
    /// </summary>
    public class NavCurve : IBinarySerializable
    {
        public Token Name { get; set; }

        public (byte EndNode, byte EndLane, byte StartNode, byte StartLane) LeadsToNodes { get; set; }

        public Vector3 StartPosition { get; set; }

        public Vector3 EndPosition { get; set; }

        public Quaternion StartRotation { get; set; }

        public Quaternion EndRotation { get; set; }

        public float Length { get; set; }

        public int[] NextLines { get; set; } = new int[4];

        public int[] PreviousLines { get; set; } = new int[4];

        public uint CountNext { get; set; }

        public uint CountPrevious { get; set; }

        public int SemaphoreId { get; set; }

        public Token TrafficRule { get; set; }

        public uint NewData1Id { get; set; }

        private FlagField flags = new();

        public BlinkerType Blinker
        {
            get => (BlinkerType)flags.GetBitString(2, 3);
            set => flags.SetBitString(2, 3, (uint)value);
        }

        /// <summary>
        /// Determines which AI vehicles can use this curve.
        /// <para>AI vehicles will try to go into most suitable curve, 
        /// but if there will be none, they can also use any other 
        /// even if they are not allowed to.</para>
        /// </summary>
        public AllowedVehicles AllowedVehicles
        {
            get => (AllowedVehicles)flags.GetBitString(5, 2);
            set => flags.SetBitString(5, 2, (uint)value);
        }

        /// <summary>
        /// Determines if the probability of AI vehicles entering this (prefab? nav. path?) is lowered.
        /// </summary>
        public bool LowProbability
        {
            get => flags[13];
            set => flags[13] = value;
        }

        /// <summary>
        /// Property defining extra limited displacement for AI vehicles.
        /// </summary>
        public bool LimitDisplacement
        {
            get => flags[14];
            set => flags[14] = value;
        }

        /// <summary>
        /// Determines if the PriorityModifier value will be added 
        /// to already existing priority for this lane.
        /// </summary>
        public bool AdditivePriority
        {
            get => flags[15];
            set => flags[15] = value;
        }

        public Nibble PriorityModifier
        {
            get => (Nibble)flags.GetBitString(16, 4);
            set => flags.SetBitString(16, 4, (uint)value);
        }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            switch (version)
            {
                case 0x15:
                    Deserialize15(r);
                    break;
                case 0x16:
                case 0x17:
                case 0x18:
                    Deserialize16to18(r);
                    break;
                default:
                    throw new NotSupportedException($"Version {version} is not supported.");
            }
        }

        public void Deserialize16to18(BinaryReader r) 
        {
            Name = r.ReadToken();
            flags = new FlagField(r.ReadUInt32());

            LeadsToNodes = (r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte());

            StartPosition = r.ReadVector3();
            EndPosition = r.ReadVector3();

            StartRotation = r.ReadQuaternion();
            EndRotation = r.ReadQuaternion();

            Length = r.ReadSingle();

            for (int i = 0; i < NextLines.Length; i++)
            {
                NextLines[i] = r.ReadInt32();
            }

            for (int i = 0; i < PreviousLines.Length; i++)
            {
                PreviousLines[i] = r.ReadInt32();
            }

            CountNext = r.ReadUInt32();
            CountPrevious = r.ReadUInt32();

            SemaphoreId = r.ReadInt32();

            TrafficRule = r.ReadToken();

            NewData1Id = r.ReadUInt32();
        }

        public void Deserialize15(BinaryReader r)
        {
            Name = r.ReadToken();
            flags = new FlagField(r.ReadUInt32());

            LeadsToNodes = (r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte());

            StartPosition = r.ReadVector3();
            EndPosition = r.ReadVector3();

            StartRotation = r.ReadQuaternion();
            EndRotation = r.ReadQuaternion();

            Length = r.ReadSingle();

            for (int i = 0; i < NextLines.Length; i++)
            {
                NextLines[i] = r.ReadInt32();
            }

            for (int i = 0; i < PreviousLines.Length; i++)
            {
                PreviousLines[i] = r.ReadInt32();
            }

            CountNext = r.ReadUInt32();
            CountPrevious = r.ReadUInt32();

            SemaphoreId = r.ReadInt32();

            TrafficRule = r.ReadToken();
        }


        public void Serialize(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
