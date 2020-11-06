using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap.Lookup
{
    public sealed class DlcGuard
    {
        public static byte None = 0;

        // ETS2
        public static readonly byte East = 1;
        public static readonly byte Scandinavia = 2;
        public static readonly byte France = 3;
        public static readonly byte Italy = 4;
        public static readonly byte FranceAndItaly = 5;
        public static readonly byte Baltic = 6;
        public static readonly byte BalticAndEast = 7;
        public static readonly byte BalticAndScandinavia = 8;
        public static readonly byte BlackSea = 9;
        public static readonly byte BlackSeaAndEast = 10;

        // ATS
        public static readonly byte Nevada = 1;
        public static readonly byte Arizona = 2;
        public static readonly byte NewMexico = 3;
        public static readonly byte Oregon = 4;
        public static readonly byte Washington = 5;
        public static readonly byte OregonAndWashington = 6;
        public static readonly byte Utah = 7;
        public static readonly byte NewMexicoAndUtah = 8;
        public static readonly byte Idaho = 9;
        public static readonly byte IdahoAndOregon = 10;
        public static readonly byte IdahoAndUtah = 11;
        public static readonly byte IdahoAndWashington = 12;
        public static readonly byte Colorado = 13;
        public static readonly byte ColoradoAndNewMexico = 14;
        public static readonly byte ColoradoAndUtah = 15;
    }
}
