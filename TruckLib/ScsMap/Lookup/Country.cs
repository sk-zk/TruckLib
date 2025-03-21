﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap.Lookup
{
    /// <summary>
    /// Country IDs as defined by SCS. Mods may use other IDs.
    /// </summary>
    public static class CountryId
    {
        /// <summary>
        /// Country IDs used or recommended for use in Euro Truck Simulator 2.
        /// </summary>
        public static class Ets2
        {
            // Officially implemented:
            public static readonly byte Albania = 14;
            public static readonly byte Austria = 1;
            public static readonly byte Belgium = 2;
            public static readonly byte BosniaAndHerzegovina = 17;
            public static readonly byte Bulgaria = 18;
            public static readonly byte Croatia = 19;
            public static readonly byte Czechia = 3;
            public static readonly byte Denmark = 21;
            public static readonly byte Estonia = 22;
            public static readonly byte Finland = 23;
            public static readonly byte France = 4;
            public static readonly byte Germany = 5;
            public static readonly byte Hungary = 13;
            public static readonly byte Italy = 6;
            public static readonly byte Latvia = 28;
            public static readonly byte Lithuania = 30;
            public static readonly byte Luxembourg = 12;
            public static readonly byte Montenegro = 34;
            public static readonly byte Netherlands = 7;
            public static readonly byte Norway = 35;
            public static readonly byte Poland = 8;
            public static readonly byte Portugal = 36;
            public static readonly byte RepublicOfMacedonia = 203;
            public static readonly byte Romania = 37;
            public static readonly byte Russia = 38;
            public static readonly byte Serbia = 40;
            public static readonly byte Slovakia = 11;
            public static readonly byte Slovenia = 41;
            public static readonly byte Spain = 42;
            public static readonly byte Sweden = 43;
            public static readonly byte Switzerland = 9;
            public static readonly byte Turkey = 44;
            public static readonly byte UnitedKingdom = 10;

            // Recommended by SCS for mods:
            public static readonly byte Andorra = 15;
            public static readonly byte Belarus = 16;
            public static readonly byte Cyprus = 20;
            public static readonly byte Georgia = 24;
            public static readonly byte Greece = 25;
            public static readonly byte Iceland = 26;
            public static readonly byte RepublicOfIreland = 27;
            public static readonly byte Liechtenstein = 29;
            public static readonly byte Malta = 31;
            public static readonly byte Moldova = 32;
            public static readonly byte Monaco = 33;
            public static readonly byte SanMarino = 39;
            public static readonly byte Ukraine = 45;

            public static readonly byte Afghanistan = 46;
            public static readonly byte Armenia = 47;
            public static readonly byte Azerbaijan = 48;
            public static readonly byte Bahrain = 49;
            public static readonly byte Bangladesh = 50;
            public static readonly byte Bhutan = 51;
            public static readonly byte Brunei = 52;
            public static readonly byte Cambodia = 53;
            public static readonly byte China = 54;
            public static readonly byte EastTimor = 55;
            public static readonly byte India = 56;
            public static readonly byte Indonesia = 57;
            public static readonly byte Iran = 58;
            public static readonly byte Iraq = 59;
            public static readonly byte Israel = 60;
            public static readonly byte Palestine = 61;
            public static readonly byte Japan = 62;
            public static readonly byte Jordan = 63;
            public static readonly byte Kazakhstan = 64;
            public static readonly byte Kuwait = 65;
            public static readonly byte Kyrgyzstan = 66;
            public static readonly byte Laos = 67;
            public static readonly byte Lebanon = 68;
            public static readonly byte Malaysia = 69;
            public static readonly byte Maldives = 70;
            public static readonly byte Mongolia = 71;
            public static readonly byte Myanmar = 72;
            public static readonly byte Nepal = 73;
            public static readonly byte NorthKorea = 74;
            public static readonly byte Oman = 75;
            public static readonly byte Pakistan = 76;
            public static readonly byte Philippines = 77;
            public static readonly byte Qatar = 78;
            public static readonly byte SaudiArabia = 79;
            public static readonly byte Singapore = 80;
            public static readonly byte SouthKorea = 81;
            public static readonly byte SriLanka = 82;
            public static readonly byte Syria = 83;
            public static readonly byte Taiwan = 84;
            public static readonly byte Tajikistan = 85;
            public static readonly byte Thailand = 86;
            public static readonly byte Tibet = 87;
            public static readonly byte Turkmenistan = 88;
            public static readonly byte UnitedArabEmirates = 89;
            public static readonly byte Uzbekistan = 90;
            public static readonly byte Vietnam = 91;
            public static readonly byte Yemen = 92;

            public static readonly byte Algeria = 93;
            public static readonly byte Angola = 94;
            public static readonly byte Benin = 95;
            public static readonly byte Botswana = 96;
            public static readonly byte BurkinaFaso = 97;
            public static readonly byte Burundi = 98;
            public static readonly byte Cameroon = 99;
            public static readonly byte CapeVerde = 100;
            public static readonly byte CentralAfricanRepublic = 101;
            public static readonly byte Chad = 102;
            public static readonly byte Comoros = 103;
            public static readonly byte RepublicOfTheCongo = 104;
            public static readonly byte DemocraticRepublicOfTheCongo = 105;
            public static readonly byte CoteDIvoire = 106;
            public static readonly byte Djibouti = 107;
            public static readonly byte Egypt = 108;
            public static readonly byte EquatorialGuinea = 109;
            public static readonly byte Eritrea = 110;
            public static readonly byte Ethiopia = 111;
            public static readonly byte Gabon = 112;
            public static readonly byte TheGambia = 113;
            public static readonly byte Ghana = 114;
            public static readonly byte Guinea = 115;
            public static readonly byte GuineaBissau = 116;
            public static readonly byte Kenya = 117;
            public static readonly byte Lesotho = 118;
            public static readonly byte Liberia = 119;
            public static readonly byte Libya = 120;
            public static readonly byte Madagascar = 121;
            public static readonly byte Malawi = 122;
            public static readonly byte Mali = 123;
            public static readonly byte Mauritania = 124;
            public static readonly byte Mauritius = 125;
            public static readonly byte Morocco = 126;
            public static readonly byte Mozambique = 127;
            public static readonly byte Namibia = 128;
            public static readonly byte Niger = 129;
            public static readonly byte Nigeria = 130;
            public static readonly byte Rwanda = 131;
            public static readonly byte SaoTomeAndPrincipe = 132;
            public static readonly byte Senegal = 133;
            public static readonly byte Seychelles = 134;
            public static readonly byte SierraLeone = 135;
            public static readonly byte Somalia = 136;
            public static readonly byte SouthAfrica = 137;
            public static readonly byte SouthSudan = 138;
            public static readonly byte Sudan = 139;
            public static readonly byte Swaziland = 140;
            public static readonly byte Tanzania = 141;
            public static readonly byte Togo = 142;
            public static readonly byte Tunisia = 143;
            public static readonly byte Uganda = 144;
            public static readonly byte WesternSahara = 145;
            public static readonly byte Zambia = 146;
            public static readonly byte Zaire = 147;
            public static readonly byte Zimbabwe = 148;

            public static readonly byte UnitedStates = 149;
            public static readonly byte Mexico = 150;
            public static readonly byte Canada = 151;
            public static readonly byte Greenland = 152;

            public static readonly byte AntiguaAndBarbuda = 153;
            public static readonly byte Bahamas = 154;
            public static readonly byte Barbados = 155;
            public static readonly byte Belize = 156;
            public static readonly byte CaymanIslands = 157;
            public static readonly byte CostaRica = 158;
            public static readonly byte Cuba = 159;
            public static readonly byte Dominica = 160;
            public static readonly byte DominicanRepublic = 161;
            public static readonly byte ElSalvador = 162;
            public static readonly byte Grenada = 163;
            public static readonly byte Guatemala = 164;
            public static readonly byte Haiti = 165;
            public static readonly byte Honduras = 166;
            public static readonly byte Jamaica = 167;
            public static readonly byte Nicaragua = 168;
            public static readonly byte Panama = 169;
            public static readonly byte PuertoRico = 170;
            public static readonly byte SaintKittsAndNevis = 171;
            public static readonly byte SaintLucia = 172;
            public static readonly byte SaintVincentAndTheGrenadines = 173;
            public static readonly byte TrinidadAndTobago = 174;
            public static readonly byte TurksAndCaicos = 175;

            public static readonly byte Argentina = 176;
            public static readonly byte Bolivia = 177;
            public static readonly byte Brazil = 178;
            public static readonly byte Chile = 179;
            public static readonly byte Colombia = 180;
            public static readonly byte Ecuador = 181;
            public static readonly byte FrenchGuiana = 182;
            public static readonly byte Guyana = 183;
            public static readonly byte Paraguay = 184;
            public static readonly byte Peru = 185;
            public static readonly byte Suriname = 186;
            public static readonly byte Uruguay = 187;
            public static readonly byte Venezuela = 188;

            public static readonly byte Australia = 189;
            public static readonly byte NewZealand = 190;
            public static readonly byte Fiji = 191;
            public static readonly byte PapuaNewGuinea = 192;
            public static readonly byte SolomonIslands = 193;
            public static readonly byte Vanuatu = 194;
            public static readonly byte Kiribati = 195;
            public static readonly byte MarshallIslands = 196;
            public static readonly byte Micronesia = 197;
            public static readonly byte Nauru = 198;
            public static readonly byte Palau = 199;
            public static readonly byte Samoa = 200;
            public static readonly byte Tonga = 201;
            public static readonly byte Tuvalu = 202;
        }

        /// <summary>
        /// Country IDs used or recommended for use in American Truck Simulator.
        /// </summary>
        public static class Ats
        {
            // Officially implemented:
            public static readonly byte Arizona = 3;
            public static readonly byte Arkansas = 6;
            public static readonly byte California = 1;
            public static readonly byte Colorado = 7;
            public static readonly byte Idaho = 13;
            public static readonly byte Kansas = 17;
            public static readonly byte Missouri = 26;
            public static readonly byte Montana = 27;
            public static readonly byte Nebraska = 28;
            public static readonly byte Nevada = 2;
            public static readonly byte NewMexico = 31;
            public static readonly byte Oklahoma = 36;
            public static readonly byte Oregon = 37;
            public static readonly byte Texas = 43;
            public static readonly byte Utah = 44;
            public static readonly byte Washington = 47;
            public static readonly byte Wyoming = 50;

            // Recommended by SCS for mods:
            public static readonly byte Alabama = 4;
            public static readonly byte Alaska = 5;
            public static readonly byte Connecticut = 8;
            public static readonly byte Delaware = 9;
            public static readonly byte Florida = 10;
            public static readonly byte Georgia = 11;
            public static readonly byte Hawaii = 12;
            public static readonly byte Illinois = 14;
            public static readonly byte Indiana = 15;
            public static readonly byte Iowa = 16;
            public static readonly byte Kentucky = 18;
            public static readonly byte Louisiana = 19;
            public static readonly byte Maine = 20;
            public static readonly byte Maryland = 21;
            public static readonly byte Massachusetts = 22;
            public static readonly byte Michigan = 23;
            public static readonly byte Minnesota = 24;
            public static readonly byte Mississippi = 25;
            public static readonly byte NewHampshire = 29;
            public static readonly byte NewJersey = 30;
            public static readonly byte NewYork = 32;
            public static readonly byte NorthCarolina = 33;
            public static readonly byte NorthDakota = 34;
            public static readonly byte Ohio = 35;
            public static readonly byte Pennsylvania = 38;
            public static readonly byte RhodeIsland = 39;
            public static readonly byte SouthCarolina = 40;
            public static readonly byte SouthDakota = 41;
            public static readonly byte Tennessee = 42;
            public static readonly byte Vermont = 45;
            public static readonly byte Virginia = 46;
            public static readonly byte WestVirginia = 48;
            public static readonly byte Wisconsin = 49;

            public static readonly byte Mexico = 51;
            public static readonly byte Canada = 52;
            public static readonly byte Greenland = 53;

            public static readonly byte AntiguaAndBarbuda = 54;
            public static readonly byte Bahamas = 55;
            public static readonly byte Barbados = 56;
            public static readonly byte Belize = 57;
            public static readonly byte CaymanIslands = 58;
            public static readonly byte CostaRica = 59;
            public static readonly byte Cuba = 60;
            public static readonly byte Dominica = 61;
            public static readonly byte DominicanRepublic = 62;
            public static readonly byte ElSalvador = 63;
            public static readonly byte Grenada = 64;
            public static readonly byte Guatemala = 65;
            public static readonly byte Haiti = 66;
            public static readonly byte Honduras = 67;
            public static readonly byte Jamaica = 68;
            public static readonly byte Nicaragua = 69;
            public static readonly byte Panama = 70;
            public static readonly byte PuertoRico = 71;
            public static readonly byte SaintKittsAndNevis = 72;
            public static readonly byte SaintLucia = 73;
            public static readonly byte SaintVincentAndTheGrenadines = 74;
            public static readonly byte TrinidadAndTobago = 75;
            public static readonly byte TurksAndCaicos = 76;

            public static readonly byte Argentina = 77;
            public static readonly byte Bolivia = 78;
            public static readonly byte Brazil = 79;
            public static readonly byte Chile = 80;
            public static readonly byte Colombia = 81;
            public static readonly byte Ecuador = 82;
            public static readonly byte FrenchGuiana = 83;
            public static readonly byte Guyana = 84;
            public static readonly byte Paraguay = 85;
            public static readonly byte Peru = 86;
            public static readonly byte Suriname = 87;
            public static readonly byte Uruguay = 88;
            public static readonly byte Venezuela = 89;
        }
    }
}
