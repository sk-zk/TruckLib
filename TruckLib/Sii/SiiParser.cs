using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

// Do yourself a favor and don't look.
// I'm as ashamed of this file as you are horrified reading it.

[assembly: InternalsVisibleTo("TruckLibTests")]
namespace TruckLib.Sii
{
    /// <summary>
    /// De/serializes a SII file.
    /// </summary>
    internal class SiiParser
    {
        public bool ShouldCheckForAndInsertIncludes { get; set; } = true;

        public string Indentation { get; set; } = "    ";
        internal string TupleAttribOpen = "(";
        internal string TupleAttribClose = ")";
        private const string SiiHeader = "SiiNunit";
        private const char TupleSeperator = ',';
        private const string IncludeKeyword = "@include";

        /// <summary>
        /// Sets how to handle duplicate attributes in a unit.
        /// </summary>
        private bool OverrideOnDuplicate = true;

        private readonly CultureInfo culture = CultureInfo.InvariantCulture;

        private Dictionary<string, int> arrInsertIndex = new();

        public SiiFile DeserializeFromString(string sii, string siiPath = "")
        {
            var siiFile = new SiiFile();

            sii = RemoveComments(sii);
            if (ShouldCheckForAndInsertIncludes)
                sii = InsertIncludes(sii, siiPath);

            siiFile.GlobalScope = sii.StartsWith(SiiHeader);

            // get units
            string unitDeclarations = @"[\w]+?\s*:\s*\""?[\w.]+?\""?\s*{"; // don't @ me
            foreach (var m in Regex.Matches(sii, unitDeclarations, RegexOptions.Singleline).Cast<Match>())
            {
                // find ending bracket of this unit
                var start = m.Index;
                var startBracket = start + m.Length; // start after the opening bracket of the unit
                int? end = null;
                var bracketsStack = 0;
                for (int i = startBracket; i < sii.Length; i++)
                {
                    if (sii[i] == '{')
                        bracketsStack++;
                    else if (sii[i] == '}')
                        bracketsStack--;

                    if (bracketsStack == -1)
                    {
                        end = i;
                        break;
                    }
                }

                if (end is null)
                    throw new SiiParserException($"Expected '}}' at {sii.Length - 1}, found EOF");
                else
                    siiFile.Units.Add(ParseUnit(sii.Substring(start, end.Value - start + 1)));
            }

            return siiFile;
        }

        private string InsertIncludes(string sii, string siiPath)
        {
            var output = new StringBuilder();

            using var reader = new StringReader(sii);
            string line;
            while ((line = reader.ReadLine()) is not null)
            {
                if (line.StartsWith(IncludeKeyword))
                {
                    var match = Regex.Match(line, @"@include ""(.*)""");
                    if (match.Groups.Count > 0)
                    {
                        var path = match.Groups[1].Value;
                        path = Path.Combine(siiPath, path);
                        if (!File.Exists(path))
                            throw new FileNotFoundException("Included file was not found.", path);

                        var fileContents = File.ReadAllText(path);
                        fileContents = RemoveComments(fileContents);
                        output.AppendLine(fileContents);
                        continue;
                    }
                }
                output.AppendLine(line);
            }

            return output.ToString();
        }

        public SiiFile DeserializeFromFile(string path) =>
            DeserializeFromString(File.ReadAllText(path), Path.GetDirectoryName(path));

        private string RemoveComments(string sii) =>
            Regex.Replace(sii,
                // 🠋 remove C-style comments
                //           🠋 remove # comments
                //                      🠋 remove // comments
                @"\/\*.*\*\/|#[^\n\r]*|\/\/[^\n\r]*",
                "",
                RegexOptions.Singleline);

        private Unit ParseUnit(string unitStr)
        {
            arrInsertIndex.Clear();

            var firstColonPos = unitStr.IndexOf(':');
            var openBracketPos = unitStr.IndexOf('{');
            var closeBracketPos = unitStr.LastIndexOf('}');

            var unit = new Unit
            {
                Class = unitStr[..firstColonPos].Trim(),
                Name = unitStr.Substring(firstColonPos + 1,
                    openBracketPos - firstColonPos - 1).Trim()
            };

            var attributeLines = unitStr.Substring(openBracketPos + 1,
                closeBracketPos - openBracketPos - 1)
                .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in attributeLines)
            {
                if (string.IsNullOrWhiteSpace(line)) 
                    continue;

                var (Name, Value) = ParseAttribute(line);
                if (Name.EndsWith("]")) // list or fixed-length array type
                {
                    ParseListOrArrayAttribute(unit, Name, Value);
                }
                else
                {
                    AddAttribute(unit, Name, Value);
                }
            }
            return unit;
        }

        private void AddAttribute(Unit unit, string name, dynamic value)
        {
            if (unit.Attributes.ContainsKey(name))
            {
                if (OverrideOnDuplicate)
                    unit.Attributes[name] = value;
            }
            else
            {
                unit.Attributes.Add(name, value);
            }
        }

        private (string Name, dynamic Value) ParseAttribute(string line)
        {
            line = line.Trim();
            var firstColonPos = line.IndexOf(':');
            var attributeName = line[..firstColonPos].Trim();
            var valueStr = line.Substring(firstColonPos + 1, line.Length - firstColonPos - 1);
            var value = ParseAttributeValue(valueStr);
            return (attributeName, value);
        }

        private void ParseListOrArrayAttribute(Unit unit, string name, dynamic value)
        {
            var match = Regex.Match(name, @"^(.+)\[(.*)\]$");
            if (!match.Success)
                return;

            var arrName = match.Groups[1].Value;
            var hasArrIndex = int.TryParse(match.Groups[2].Value, out int arrIndex);
            if (!arrInsertIndex.ContainsKey(arrName))
            {
                arrInsertIndex.Add(arrName, 0);
            }

            // figure out if this is a fixed-length array entry or a list entry
            // and create the thing if it doesn't exist yet
            bool isFixedLengthArray;
            if (unit.Attributes.TryGetValue(arrName, out var whatsAllThisThen))
            {
                isFixedLengthArray = whatsAllThisThen is int or Array;
            } 
            else
            {
                isFixedLengthArray = hasArrIndex;
                if (isFixedLengthArray)
                {
                    int initSize = arrIndex + 1;
                    var arr = new dynamic[initSize];
                    AddAttribute(unit, arrName, arr);
                } 
                else
                {
                    AddAttribute(unit, arrName, new List<dynamic>());
                }
            }

            // insert the value
            if (isFixedLengthArray)
            {
                var val = unit.Attributes[arrName];
                dynamic[] arr;
                if (val is int)
                {
                    // existing val is int => it's a fixed-length array
                    // where the length has been read in, and now we need to
                    // create the actual array
                    arr = new dynamic[val];
                    unit.Attributes[arrName] = arr;
                }
                else
                {
                    arr = val;
                }

                if (arr.Length < arrIndex + 1)
                {
                    Array.Resize(ref arr, arrIndex + 1);
                    unit.Attributes[arrName] = arr;
                }

                if (hasArrIndex)
                {
                    arrInsertIndex[arrName] = arrIndex + 1; 
                }
                else
                {
                    arrIndex = arrInsertIndex[arrName]++;
                }
                arr[arrIndex] = value;
            }
            else
            {
                unit.Attributes[arrName].Add(value);
            }
        }

        private dynamic ParseAttributeValue(string valueStr)
        {
            if (string.IsNullOrWhiteSpace(valueStr))
                return "";
            
            valueStr = valueStr.Trim();

            // figure out type:

            // string
            const string doubleQuote = "\"";
            if (valueStr.StartsWith(doubleQuote) && valueStr.EndsWith(doubleQuote))
                return valueStr[1..^1];

            // placement type
            if (Regex.IsMatch(valueStr, @"\(.+, *.+, *.+\) *\(.+; *.+, *.+, *.+\)"))
                return ParsePlacement(valueStr);

            // expicit quaternion - undocumented but used in a few files
            if (Regex.IsMatch(valueStr, @"\(.+;.+,.+,.+\)"))
                return ParseQuaternion(valueStr);

            // inline struct / tuple types like float3, quaternion etc.
            // which type is actually used isn't possible to determine from
            // the .sii file alone
            if (valueStr.StartsWith(TupleAttribOpen) && valueStr.EndsWith(TupleAttribClose)
                && valueStr.Contains(TupleSeperator))
                return ParseTuple(valueStr);

            // bools
            if (valueStr == "true") 
                return true;
            if (valueStr == "false") 
                return false;

            // numerical types
            // the type of an attribute is not included in the .sii file,
            // so if there's no decimal point, I can only guess
            if (StringUtils.IsNumerical(valueStr))
                return ParseNumber(valueStr);

            // unit pointers
            if (valueStr.Contains('.'))
                return valueStr;

            // token
            if (Token.IsValidToken(valueStr))
                return new Token(valueStr);

            // unknown or not implemented
            return valueStr;
        }

        private dynamic ParseQuaternion(string valueStr)
        {
            var vals = valueStr[1..^1].Split(new[] { TupleSeperator, ';' });
            return new Quaternion(
                float.Parse(vals[1], culture), // x
                float.Parse(vals[2], culture), // y
                float.Parse(vals[3], culture), // z
                float.Parse(vals[0], culture)  // w
                );
        }

        private dynamic ParsePlacement(string valueStr)
        {
            var matches = Regex.Matches(valueStr, @"\((.+), *(.+), *(.+)\) *\((.+); *(.+), *(.+), *(.+)\)");
            return new Placement(
                new Vector3(
                    ParseNumber(matches[0].Groups[1].Value),
                    ParseNumber(matches[0].Groups[2].Value),
                    ParseNumber(matches[0].Groups[3].Value)
                ),
                new Quaternion(
                    ParseNumber(matches[0].Groups[5].Value),
                    ParseNumber(matches[0].Groups[6].Value),
                    ParseNumber(matches[0].Groups[7].Value),
                    ParseNumber(matches[0].Groups[4].Value)
                )
            );
        }

        private dynamic ParseTuple(string valueStr)
        {
            var tupleVals = valueStr[1..^1].Split(TupleSeperator);

            // determine the type of the tuple
            var types = new Type[tupleVals.Length];
            for (int i = 0; i < tupleVals.Length; i++)
                types[i] = ParseAttributeValue(tupleVals[i]).GetType();

            if (types.Contains(typeof(float)))
                return TupleValsToArray<float>(tupleVals);
            else if (types.Contains(typeof(ulong)))
                return TupleValsToArray<ulong>(tupleVals);
            else if(types.Contains(typeof(long)))
                return TupleValsToArray<long>(tupleVals);
            else if(types.Contains(typeof(int)))
                return TupleValsToArray<int>(tupleVals);

            return TupleValsToArray<dynamic>(tupleVals); // whatever, dynamic array it is
        }

        private T[] TupleValsToArray<T>(string[] tupleVals)
        {
            var arr = new T[tupleVals.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (T)ParseAttributeValue(tupleVals[i]);
            }
            return arr;
        }

        private dynamic ParseNumber(string valueStr)
        {
            if (StringUtils.IsHexNotationFloat(valueStr))
            {
                var bitsAsInt = int.Parse(valueStr[1..], NumberStyles.HexNumber);
                return BitConverter.Int32BitsToSingle(bitsAsInt);
            }
            else if (valueStr.Contains('.') || valueStr.Contains('e') || valueStr.Contains('E'))
            {
                if (float.TryParse(valueStr, NumberStyles.Float | NumberStyles.AllowExponent,
                    culture, out float floatResult))
                {
                    return floatResult;
                }
            }
            else
            {
                if (int.TryParse(valueStr, NumberStyles.Integer, culture, out int intResult))
                {
                    return intResult;
                }
                if (long.TryParse(valueStr, NumberStyles.Integer, culture, out long longResult))
                {
                    return longResult;
                }
                if (ulong.TryParse(valueStr, NumberStyles.Integer, culture, out ulong ulongResult))
                {
                    return ulongResult;
                }
            }
            return null;
        }

        public void Serialize(SiiFile siiFile, string path)
        {
            var str = Serialize(siiFile);
            File.WriteAllText(path, str);
        }

        public string Serialize(SiiFile siiFile)
        {
            var sb = new StringBuilder();

            if (siiFile.GlobalScope)
            {
                sb.AppendLine(SiiHeader);
                sb.AppendLine("{\n");
            }

            foreach (var unit in siiFile.Units)
            {
                sb.AppendLine($"{unit.Class} : {unit.Name} {{");
                SerializeAttributes(sb, unit);
                sb.AppendLine("}\n");
            }

            if (siiFile.GlobalScope)
                sb.AppendLine("}");

            return sb.ToString();
        }

        private void SerializeAttributes(StringBuilder sb, Unit unit)
        {
            foreach (var attrib in unit.Attributes)
            {
                if (attrib.Value is List<dynamic> list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        sb.Append($"{Indentation}{attrib.Key}[]: ");
                        SerializeAttributeValue(sb, list[i]);
                        sb.AppendLine();
                    }
                }
                else if (attrib.Value is dynamic[] arr)
                {
                    sb.AppendLine($"{Indentation}{attrib.Key}: {arr.Length}");
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (arr[i] is null)
                            continue;

                        sb.Append($"{Indentation}{attrib.Key}[{i}]: ");
                        SerializeAttributeValue(sb, arr[i]);
                        sb.AppendLine();
                    }
                }
                else
                {
                    sb.Append($"{Indentation}{attrib.Key}: ");
                    SerializeAttributeValue(sb, attrib.Value);
                    sb.AppendLine();
                }
            }
        }

        private void SerializeAttributeValue(StringBuilder sb, dynamic attribValue)
        {
            switch (attribValue)
            {
                case string _:
                    sb.Append($"\"{attribValue}\"");
                    break;
                case Array arr:
                    SerializeTuple(sb, arr);
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
                    sb.Append(b ? "true" : "false");
                    break;
                case Token t:
                    sb.Append(t.String);
                    break;
                case Quaternion q:
                    sb.Append($"{TupleAttribOpen}{q.W.ToString(culture)}; ");
                    sb.Append($"{q.X.ToString(culture)}{TupleSeperator} ");
                    sb.Append($"{q.Y.ToString(culture)}{TupleSeperator} ");
                    sb.Append($"{q.Z.ToString(culture)}{TupleAttribClose}");
                    break;
                default:
                    sb.Append(Convert.ToString(attribValue, culture));
                    break;
            }
        }

        private void SerializeTuple(StringBuilder sb, Array arr)
        {
            sb.Append(TupleAttribOpen);

            IList list = arr;
            for (int i = 0; i < list.Count; i++)
            {
                SerializeAttributeValue(sb, list[i]);

                if (i != list.Count - 1)
                {
                    sb.Append(TupleSeperator);
                    sb.Append(" ");
                }
            }

            sb.Append(TupleAttribClose);
        }

        private void SerializeIncludes(StringBuilder sb, List<string> includes)
        {
            foreach (var include in includes)
                sb.AppendLine($"{IncludeKeyword} \"{include}\"");
        }

    }
}
