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
    /// Represents a navigation curve, used to define AI traffic paths and GPS navigation.
    /// </summary>
    public class NavCurve : IBinarySerializable
    {
        public Token Name { get; set; }

        public (byte EndNode, byte EndLane, byte StartNode, byte StartLane) LeadsToNodes;

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

        protected FlagField Flags = new FlagField();

        // TODO: Check if these flags are correct

        public byte PriorityModifier
        {
            get => (byte)Flags.GetBitString(16, 4);
            set => Flags.SetBitString(16, 4, value);
        }

        public bool ForceNoBlinker
        {
            get => Flags[2];
            set => Flags[2] = value;
        }

        public bool RightBlinker
        {
            get => Flags[3];
            set => Flags[3] = value;
        }

        public bool LeftBlinker
        {
            get => Flags[4];
            set => Flags[4] = value;
        }

        /// <summary>
        /// Determines if small AI vehicles can use this curve.
        /// <para>AI vehicles will try to go into most suitable curve, 
        /// but if there will be none, they can also use any other 
        /// even if they are not allowed to.</para>
        /// </summary>
        public bool AllowSmallVehicles
        {
            get => Flags[5];
            set => Flags[5] = value;
        }

        /// <summary>
        /// Determines if large AI vehicles can use this curve.
        /// <para>AI vehicles will try to go into most suitable curve, 
        /// but if there will be none, they can also use any other 
        /// even if they are not allowed to.</para>
        /// </summary>
        public bool AllowLargeVehicles
        {
            get => Flags[6];
            set => Flags[6] = value;
        }

        /// <summary>
        /// Determines if the probability of AI vehicles entering this (prefab? nav. path?) is lowered.
        /// </summary>
        public bool LowProbability
        {
            get => Flags[14];
            set => Flags[14] = value;
        }

        /// <summary>
        /// Property defining extra limited displacement for AI vehicles.
        /// </summary>
        public bool LimitDisplacement
        {
            get => Flags[15];
            set => Flags[15] = value;
        }

        /// <summary>
        /// Determines if the PriorityModifier value will be added 
        /// to already existing priority for this lane.
        /// </summary>
        public bool AdditivePriority
        {
            get => Flags[16];
            set => Flags[16] = value;
        }

        public void Deserialize(BinaryReader r)
        {
            Name = r.ReadToken();
            Flags = new FlagField(r.ReadUInt32());

            LeadsToNodes.EndNode = r.ReadByte();
            LeadsToNodes.EndLane = r.ReadByte();
            LeadsToNodes.StartNode = r.ReadByte();
            LeadsToNodes.StartLane = r.ReadByte();

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

        public void Serialize(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}

