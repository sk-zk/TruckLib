﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents a prefab spawn point belonging to a <see cref="Company"/> item.
    /// </summary>
    public struct CompanySpawnPoint
    {
        /// <summary>
        /// The node of the spawn point.
        /// </summary>
        public INode Node { get; set; }

        internal FlagField Flags;

        /// <summary>
        /// The spawn point type.
        /// </summary>
        public CompanySpawnPointType Type 
        {
            get => (CompanySpawnPointType)Flags.GetBitString(0, 4);
            set => Flags.SetBitString(0, 4, (byte)value);
        }

        /// <summary>
        /// Instantiates a new CompanySpawnPoint.
        /// </summary>
        /// <param name="node">The node of the spawn point.</param>
        /// <param name="flags">The flag field of the spawn point.</param>
        public CompanySpawnPoint(INode node, uint flags)
        {
            Node = node;
            Flags = new FlagField(flags);
        }

        /// <summary>
        /// Instantiates a new CompanySpawnPoint.
        /// </summary>
        /// <param name="node">The node of the spawn point.</param>
        /// <param name="type">The type of the spawn point.</param>
        public CompanySpawnPoint(INode node, CompanySpawnPointType type)
        {
            Node = node;
            Flags = new FlagField(0);
            Type = type;
        }
    }
}
