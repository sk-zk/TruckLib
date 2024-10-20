using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap.Lookup
{
    /// <summary>
    /// DLC guard IDs used in ETS2 and ATS.
    /// </summary>
    public sealed class DlcGuard
    {
        public static readonly byte None = 0;

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
        public static readonly byte Ibera = 11;
        public static readonly byte IberaAndFrance = 12;
        public static readonly byte Russia = 13;
        public static readonly byte BalticAndRussia = 14;
        public static readonly byte Krone = 15;
        public static readonly byte WestBalkans = 16;
        public static readonly byte WestBalkansAndEast = 17;
        public static readonly byte WestBalkansAndBlackSea = 18;
        public static readonly byte Feldbinder = 19;

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
        public static readonly byte Wyoming = 16;
        public static readonly byte ColoradoAndWyoming = 17;
        public static readonly byte IdahoAndWyoming = 18;
        public static readonly byte UtahAndWyoming = 19;
        public static readonly byte Texas = 20;
        public static readonly byte TexasAndNewMexico = 21;
        public static readonly byte Montana = 22;
        public static readonly byte MontanaAndIdaho = 23;
        public static readonly byte MontanaAndWyoming = 24;
        public static readonly byte Oklahoma = 25;
        public static readonly byte OklahomaAndColorado = 26;
        public static readonly byte OklahomaAndNewMexico = 27;
        public static readonly byte OklahomaAndTexas = 28;
        public static readonly byte Kansas = 29;
        public static readonly byte KansasAndColorado = 30;
        public static readonly byte KansasAndOklahoma = 31;
        public static readonly byte Nebraska = 32;
        public static readonly byte NebraskaAndColorado = 33;
        public static readonly byte NebraskaAndKansas = 34;
        public static readonly byte NebraskaAndWyoming = 35;
        public static readonly byte Arkansas = 36;
        public static readonly byte ArkansasAndOklahoma = 37;
        public static readonly byte ArkansasAndTexas = 38;
    }
}
