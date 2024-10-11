using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Models.Ppd.Lookup
{
    public static class PriorityModifier
    {
        // Named presets from the Blender tools

        public static readonly byte MinorRoadLeft = 4;
        public static readonly byte MinorRoadRight = 5;
        public static readonly byte MinorRoadStraight = 6;

        public static readonly byte MajorRoadLeft = 10;
        public static readonly byte MajorRoadRight = 11;
        public static readonly byte MajorRoadStraight = 12;
    }
}
