using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.HashFs
{
    internal struct Entry
    {
        public ulong Hash;
        public ulong Offset;
        public BitArray Flags;
        public uint Crc;
        public uint Size;
        public uint CompressedSize;

        public bool IsDirectory => Flags[0];
        public bool IsCompressed => Flags[1];
        public bool Verify => Flags[2]; //TODO: What is this?
        public bool IsEncrypted => Flags[3];
    }
}
