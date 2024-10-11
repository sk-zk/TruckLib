using Sprache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("TruckLibTests")]
namespace TruckLib.Sii
{
    internal static class ParserElements
    {
        internal const string SiiHeader = "SiiNunit";
        internal const char StringDelimiterChar = '"';
        internal const char TupleOpenChar = '(';
        internal const char TupleCloseChar = ')';
        internal const char MatTupleOpenChar = '{';
        internal const char MatTupleCloseChar = '}';
        internal const char TupleSeparatorChar = ',';
        internal const char QuaternionWSeparatorChar = ';';
        internal const string True = "true";
        internal const string False = "false";
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        internal static readonly Parser<dynamic> Token =
            from _ in Parse.Chars(" \t").Many()
            from t in Parse.Chars(TruckLib.Token.CharacterSet).Repeat(1, 12).Text()
            from __ in Parse.Chars(" \t").Many()
            from ___ in Parse.LineTerminator
            select (dynamic)new Token(t);

        internal static readonly Parser<IEnumerable<char>> StringDelimiter =
            Parse.Char(StringDelimiterChar).Once().Token();

        internal static readonly Parser<dynamic> DelimitedString =
            from _ in StringDelimiter
            from s in Parse.AnyChar.Except(StringDelimiter).Many().Text()
            from __ in StringDelimiter
            select (dynamic)s;

        internal static readonly Parser<IOption<char>> Sign =
            Parse.Chars("+-").Optional().Token();

        internal static readonly Parser<IEnumerable<char>> ExponentPart =
            from _ in Parse.Char('e')
            from s in Sign
            from d in Parse.Numeric.AtLeastOnce().Text()
            select "e" + (s.IsDefined ? s.Get() : "") + d;

        internal static readonly Parser<dynamic> Float =
            from s in Sign
            from n in Parse.DecimalInvariant
            from e in ExponentPart.Optional()
            select (dynamic)float.Parse(
                (s.IsDefined ? s.Get() : "") + 
                n + 
                (e.IsDefined ? e.Get() : ""),
                CultureInfo.InvariantCulture);

        internal static readonly Parser<dynamic> FloatInHexNotation =
            from _ in Parse.Char('&')
            from n in Parse.Chars("0123456789abcdefABCDEF").Repeat(8).Text()
            select (dynamic)BitConverter.Int32BitsToSingle(int.Parse(n, NumberStyles.HexNumber));

        internal static readonly Parser<char> TupleOpen =
            Parse.Char(TupleOpenChar).Token();

        internal static readonly Parser<char> TupleClose =
            Parse.Char(TupleCloseChar).Token();

        internal static readonly Parser<dynamic> FloatTupleValue =
            from n in Float.XOr(FloatInHexNotation)
            from _ in Parse.Char(TupleSeparatorChar).Optional().Token()
            select n;

        internal static readonly Parser<dynamic> FloatVector2 =
            from _ in TupleOpen
            from x in FloatTupleValue
            from y in FloatTupleValue
            from __ in TupleClose
            select (dynamic)new Vector2(x, y);

        internal static readonly Parser<dynamic> FloatVector3 =
            from _ in TupleOpen
            from x in FloatTupleValue
            from y in FloatTupleValue
            from z in FloatTupleValue
            from __ in TupleClose
            select (dynamic)new Vector3(x, y, z);

        internal static readonly Parser<dynamic> MatFloatVector2 =
            from _ in Parse.Char(MatTupleOpenChar).Token()
            from x in FloatTupleValue
            from y in FloatTupleValue
            from __ in Parse.Char(MatTupleCloseChar).Token()
            select (dynamic)new Vector2(x, y);

        internal static readonly Parser<dynamic> MatFloatVector3 =
            from _ in Parse.Char(MatTupleOpenChar).Token()
            from x in FloatTupleValue
            from y in FloatTupleValue
            from z in FloatTupleValue
            from __ in Parse.Char(MatTupleCloseChar).Token()
            select (dynamic)new Vector3(x, y, z);

        internal static readonly Parser<dynamic> Integer =
            from s in Sign
            from n in Parse.Chars("0123456789").AtLeastOnce().Text()
            select ParseInt(s.GetOrElse(' ') + n);

        internal static readonly Parser<dynamic> IndividualInteger =
            from _ in Parse.Chars(" \t").Many()
            from n in Integer
            from __ in Parse.Chars(" \t").Many()
            from ___ in Parse.LineTerminator
            select n;

        internal static readonly Parser<dynamic> IntegerTupleValue =
            from n in Integer
            from _ in Parse.Char(TupleSeparatorChar).Optional().Token()
            select n;

        internal static readonly Parser<dynamic> IntTuple2 =
            from _ in TupleOpen
            from x in IntegerTupleValue
            from y in IntegerTupleValue
            from __ in TupleClose
            select (dynamic)(x, y);

        internal static readonly Parser<dynamic> IntTuple3 =
            from _ in TupleOpen
            from x in IntegerTupleValue
            from y in IntegerTupleValue
            from z in IntegerTupleValue
            from __ in TupleClose
            select (dynamic)(x, y, z);

        internal static readonly Parser<dynamic> IntTuple4 =
            from _ in TupleOpen
            from x in IntegerTupleValue
            from y in IntegerTupleValue
            from z in IntegerTupleValue
            from w in IntegerTupleValue
            from __ in TupleClose
            select (dynamic)(x, y, z, w);

        internal static readonly Parser<dynamic> Quaternion =
            from _ in TupleOpen
            from w in FloatTupleValue
            from x in FloatTupleValue
            from y in FloatTupleValue
            from z in FloatTupleValue
            from __ in TupleClose
            select (dynamic)new Quaternion(x, y, z, w);

        internal static readonly Parser<dynamic> QuaternionWValue =
            from n in Float.XOr(FloatInHexNotation)
            from _ in Parse.Char(QuaternionWSeparatorChar).Token()
            select n;

        internal static readonly Parser<dynamic> QuaternionWithSemicolon =
            from _ in TupleOpen
            from w in QuaternionWValue
            from x in FloatTupleValue
            from y in FloatTupleValue
            from z in FloatTupleValue
            from __ in TupleClose
            select (dynamic)new Quaternion(x, y, z, w);

        internal static readonly Parser<dynamic> Placement =
            from v in FloatVector3
            from q in QuaternionWithSemicolon
            select (dynamic)new Placement(v, q);

        internal static readonly Parser<dynamic> Boolean =
        Parse.String(True).Or(Parse.String(False)).Token().Text()
            .Select(b => (dynamic)bool.Parse(b));

        internal static readonly Parser<dynamic> LinkPointer =
        Parse.AnyChar.Except(Parse.WhiteSpace).AtLeastOnce().Text().Token()
            .Select(x => (dynamic)new LinkPointer(x));

        internal static readonly Parser<dynamic> OwnerPointer =
            from dot in Parse.Char('.')
            from rest in LinkPointer
            select (dynamic)new OwnerPointer(dot + rest.Value);

        internal static readonly Parser<IEnumerable<char>> Colon =
            Parse.Char(':').Once().Token();

        internal static readonly Parser<string> Key =
            Parse.AnyChar.Except(Colon).Except(Parse.WhiteSpace).AtLeastOnce().Text().Token();

        internal static readonly Parser<dynamic> Value =
            from v in Placement
                .Or(IntTuple2)
                .Or(IntTuple3)
                .Or(IntTuple4)
                .Or(FloatVector2)
                .Or(FloatVector3)
                .Or(MatFloatVector2)
                .Or(MatFloatVector3)
                .Or(Quaternion)
                .Or(QuaternionWithSemicolon)
                .Or(IndividualInteger)
                .Or(Float)
                .Or(FloatInHexNotation)
                .Or(DelimitedString)
                .Or(Boolean)
                .Or(Token)
                .Or(OwnerPointer)
                .Or(LinkPointer)
            select v;

        internal static readonly Parser<KeyValuePair<string, dynamic>> Pair =
            from k in Key
            from _ in Colon
            from v in Value
            select new KeyValuePair<string, dynamic>(k, v);

        internal static readonly Parser<string> ClassName =
        Parse.AnyChar.Except(Colon).Except(Parse.WhiteSpace).AtLeastOnce().Text().Token();

        internal static readonly Parser<string> UnitName =
        Parse.AnyChar.Except(Parse.WhiteSpace).AtLeastOnce().Text().Token();

        internal static readonly Parser<char> OpenCurly =
            Parse.Char('{').Token();

        internal static readonly Parser<char> CloseCurly =
            Parse.Char('}').Token();

        internal static readonly Parser<UnitHeader> SiiUnitHeader =
            from c in ClassName
            from _ in Colon
            from u in UnitName
            select new UnitHeader(c, u);

        internal static readonly Parser<FirstPassUnit> SiiUnit =
            from h in SiiUnitHeader.Token()
            from _ in OpenCurly
            from p in Pair.Many()
            from __ in CloseCurly
            select new FirstPassUnit(h.ClassName, h.UnitName, p.ToList());

        internal static readonly Parser<List<FirstPassUnit>> SiiUnits =
            SiiUnit.Token().Many().Select(x => x.ToList());

        internal static readonly Parser<List<FirstPassUnit>> Sii =
            from header in Parse.String(SiiHeader).Token()
            from oc in OpenCurly
            from u in SiiUnits
            from cc in CloseCurly
            select u;

        internal static readonly Parser<UnitHeader> MatUnitHeader =
            from c in ClassName
            from _ in Colon
            from u in DelimitedString
            select new UnitHeader(c, u);

        internal static readonly Parser<KeyValuePair<string, dynamic>> MatUnitValue =
            Parse.Ref(() => MatUnit).Select(x => new KeyValuePair<string, dynamic>(x.ClassName, x));

        internal static readonly Parser<KeyValuePair<string, dynamic>> MatUnitElement =
            MatUnitValue.Or(Pair);

        internal static readonly Parser<FirstPassUnit> MatUnit =
            from h in MatUnitHeader.Token()
            from _ in OpenCurly
            from p in MatUnitElement.Many()
            from __ in CloseCurly
            select new FirstPassUnit(h.ClassName, h.UnitName, p.ToList());

        internal static readonly Parser<FirstPassUnit> Mat = MatUnit.Select(x => x);

        internal static dynamic ParseInt(string n)
        {
            var culture = CultureInfo.InvariantCulture;
            if (int.TryParse(n, NumberStyles.Integer, culture, out int intResult))
            {
                return intResult;
            }
            if (long.TryParse(n, NumberStyles.Integer, culture, out long longResult))
            {
                return longResult;
            }
            if (ulong.TryParse(n, NumberStyles.Integer, culture, out ulong ulongResult))
            {
                return ulongResult;
            }
            throw new ArgumentException("Unable to parse", nameof(n));
        }

        internal static void SerializeAttributes(StringBuilder sb, Dictionary<string, dynamic> attributes, 
            string indentation, bool isMat = false)
        {
            foreach (var attrib in attributes)
            {
                if (attrib.Value is Array arr)
                {
                    sb.AppendLine($"{indentation}{attrib.Key}: {arr.Length}");
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (arr.GetValue(i) is null)
                            continue;

                        sb.Append($"{indentation}{attrib.Key}[{i}]: ");
                        SerializeAttributeValue(sb, arr.GetValue(i), isMat);
                        sb.AppendLine();
                    }
                }
                else if (attrib.Value is IList list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        sb.Append($"{indentation}{attrib.Key}[]: ");
                        SerializeAttributeValue(sb, list[i], isMat);
                        sb.AppendLine();
                    }
                }
                else
                {
                    sb.Append($"{indentation}{attrib.Key}: ");
                    SerializeAttributeValue(sb, attrib.Value, isMat);
                    sb.AppendLine();
                }
            }
        }

        private static void SerializeAttributeValue(StringBuilder sb, dynamic attribValue, bool isMat = false)
        {
            switch (attribValue)
            {
                case string _:
                    sb.Append($"{StringDelimiterChar}{attribValue}{StringDelimiterChar}");
                    break;
                case Array arr:
                    SerializeTuple(sb, arr, isMat);
                    break;
                case double d:
                    // force ".0" if number has no decimals so the game will definitely parse it as float.
                    if (d % 1 == 0)
                        sb.Append(d.ToString("F1", culture));
                    else
                        sb.Append(d.ToString(culture));
                    break;
                case float f:
                    // force ".0" if number has no decimals so the game will definitely parse it as float.
                    if (f % 1 == 0)
                        sb.Append(f.ToString("F1", culture));
                    else
                        sb.Append(f.ToString(culture));
                    break;
                case bool b:
                    sb.Append(b ? True : False);
                    break;
                case Token t:
                    sb.Append(t.String);
                    break;
                case Quaternion q:
                    SerializeQuaternion(sb, q, isMat);
                    break;
                case Vector2 v2:
                    SerializeTuple(sb, new[] { v2.X, v2.Y }, isMat);
                    break;
                case Vector3 v3:
                    SerializeTuple(sb, new[] { v3.X, v3.Y, v3.Z }, isMat);
                    break;
                case Vector4 v4:
                    SerializeTuple(sb, new[] { v4.X, v4.Y, v4.Z, v4.W }, isMat);
                    break;
                case Placement p:
                    SerializeAttributeValue(sb, p.Position);
                    sb.Append(' ');
                    SerializeAttributeValue(sb, p.Rotation);
                    break;
                default:
                    sb.Append(Convert.ToString(attribValue, culture));
                    break;
            }
        }

        private static void SerializeQuaternion(StringBuilder sb, Quaternion q, bool isMat = false)
        {
            sb.Append(isMat ? MatTupleOpenChar : TupleOpenChar);
            SerializeAttributeValue(sb, q.W);
            sb.Append($"{QuaternionWSeparatorChar} ");
            SerializeAttributeValue(sb, q.X);
            sb.Append($"{TupleSeparatorChar} ");
            SerializeAttributeValue(sb, q.Y);
            sb.Append($"{TupleSeparatorChar} ");
            SerializeAttributeValue(sb, q.Z);
            sb.Append(isMat ? MatTupleCloseChar : TupleCloseChar);
        }

        private static void SerializeTuple(StringBuilder sb, IList arr, bool isMat = false)
        {
            sb.Append(isMat ? MatTupleOpenChar : TupleOpenChar);
            for (int i = 0; i < arr.Count; i++)
            {
                SerializeAttributeValue(sb, arr[i]);
                if (i != arr.Count - 1)
                    sb.Append($"{TupleSeparatorChar} ");
            }
            sb.Append(isMat ? MatTupleCloseChar : TupleCloseChar);
        }
    }

    internal record UnitHeader(string ClassName, string UnitName);

    internal record FirstPassUnit(string ClassName, string UnitName, 
        List<KeyValuePair<string, dynamic>> Attributes);
}
