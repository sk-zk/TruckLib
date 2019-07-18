using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScsReader.Sii
{
    /// <summary>
    /// De/serializes a SII file.
    /// </summary>
    internal static class SiiParser
    {
        private static readonly string SiiHeader = "SiiNunit";
        private static readonly char TupleSeperator = ',';
        private static readonly string IncludeKeyword = "@include";
        private static string Indentation = "    "; // no tabs for you.

        public static SiiFile DeserializeFromString(string sii)
        {
            var siiFile = new SiiFile();

            sii = RemoveComments(sii);

            siiFile.GlobalScope = sii.StartsWith(SiiHeader);

            // get units
            string matchUnits = @"[\w]+?\s*:\s*[\w.]+?\s*{.*?}"; // don't @ me
            foreach (Match m in Regex.Matches(sii, matchUnits, RegexOptions.Singleline))
            {
                siiFile.Units.Add(ParseUnit(m.Value));
            }

            // parse top level includes
            GetTopLevelIncludes(sii, siiFile);

            return siiFile;
        }

        public static SiiFile DeserializeFromFile(string path)
        {
            var str = File.ReadAllText(path);
            return DeserializeFromString(str);
        }

        private static void GetTopLevelIncludes(string sii, SiiFile siiFile)
        {
            using (var sr = new StringReader(sii))
            {
                var bracketsStack = 0;
                // in files that use the global scope SiiNunit { },
                // ignore first bracket level:
                if (sii.StartsWith(SiiHeader)) bracketsStack = -1;

                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null) break;

                    // only parse top level includes, so
                    // make sure we're not inside a unit
                    foreach (var c in line)
                    {
                        if (c == '{') ++bracketsStack;
                        if (c == '}') --bracketsStack;
                    }

                    if (bracketsStack == 0 && line.StartsWith(IncludeKeyword))
                    {
                        var include = ParseInclude(line);
                        siiFile.Includes.Add(include);
                    }
                }
            }
        }

        private static string RemoveComments(string sii)
        {
            // TODO: Remove block comments
            var siiNoComments = new StringBuilder();
            using (var sr = new StringReader(sii))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null) break;

                    line = StringUtils.RemoveStartingAtPattern(line, "#");
                    line = StringUtils.RemoveStartingAtPattern(line, "//");

                    siiNoComments.AppendLine(line);
                }
            }
            return siiNoComments.ToString();
        }

        private static Unit ParseUnit(string unitStr)
        {
            var firstColonPos = unitStr.IndexOf(':');
            var openBracketPos = unitStr.IndexOf('{');
            var closeBracketPos = unitStr.IndexOf('}');

            var unit = new Unit();
            unit.Class = unitStr.Substring(0, firstColonPos).Trim();
            unit.Name = new UnitName(unitStr.Substring(firstColonPos + 1,
                openBracketPos - firstColonPos - 1).Trim());

            var attributeLines = unitStr.Substring(openBracketPos + 1,
                closeBracketPos - openBracketPos - 1).Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in attributeLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith(IncludeKeyword)) // no whitespace allowed
                {
                    var include = ParseInclude(line);
                    unit.Includes.Add(include);
                    continue;
                }

                var attrib = ParseAttribute(line);
                if (attrib.Name.EndsWith("[]")) // list type
                {
                    var arrName = attrib.Name.Substring(0, attrib.Name.Length - 2);
                    if (unit.Attributes.ContainsKey(arrName))
                    {
                        (unit.Attributes[arrName] as List<object>).Add(attrib.Value);
                    }
                    else
                    {
                        unit.Attributes.Add(arrName, new List<object> { attrib.Value });
                    }
                }
                else
                {
                    unit.Attributes.Add(attrib.Name, attrib.Value);
                }

            }
            return unit;
        }

        private static (string Name, object Value) ParseAttribute(string line)
        {
            line = line.Trim();
            var firstColonPos = line.IndexOf(':');
            var attributeName = line.Substring(0, firstColonPos).Trim();
            var valueStr = line.Substring(firstColonPos + 1, line.Length - firstColonPos - 1);
            var value = ParseAttributeValue(valueStr);
            return (attributeName, value);
        }

        private static object ParseAttributeValue(string valueStr)
        {
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                return "";
            }
            valueStr = valueStr.Trim();

            // figure out type:

            // string
            const string doubleQuote = "\"";
            if (valueStr.StartsWith(doubleQuote) && valueStr.EndsWith(doubleQuote))
            {
                return valueStr.Substring(1, valueStr.Length - 2);
            }

            // inline struct / tuple types like float3, quaternion etc.
            // which type is actually used isn't possible to determine from
            // the .sii file alone
            if (valueStr.StartsWith("(") && valueStr.EndsWith(")")
                && valueStr.Contains(TupleSeperator))
            {
                return ParseTuple(valueStr);

            }

            // bools
            if (valueStr == "true") return true;
            if (valueStr == "false") return false;

            // numerical types
            // the type of an attribute is not included in the .sii file,
            // so if there's no decimal point, I can only guess
            if (StringUtils.IsNumerical(valueStr))
            {
                return ParseNumber(valueStr);
            }

            // unit pointers
            // TODO: proper type check
            if (valueStr.Contains("."))
            {
                return new UnitName(valueStr);
            }

            // token
            if (Token.IsValidToken(valueStr))
            {
                return new Token(valueStr);
            }

            // unknown or not implemented
            return valueStr;
        }

        private static object ParseTuple(string valueStr)
        {
            var tupleVals = valueStr.Substring(1, valueStr.Length - 2)
                .Split(TupleSeperator);

            // read the first which determines the type
            var first = ParseAttributeValue(tupleVals[0]);
            if (first is int)
            {
                return FinishArray(tupleVals, (int)first);
            }
            if (first is long)
            {
                return FinishArray(tupleVals, (long)first);
            }
            if (first is ulong)
            {
                return FinishArray(tupleVals, (ulong)first);
            }
            if (first is float)
            {
                return FinishArray(tupleVals, (float)first);
            }
            return FinishArray(tupleVals, first); // whatever, object array it is
        }

        private static T[] FinishArray<T>(string[] tupleVals, T first)
        {
            var arr = new T[tupleVals.Length];
            arr[0] = (T)first;
            for (int i = 1; i < arr.Length; i++)
            {
                arr[i] = (T)ParseAttributeValue(tupleVals[i]);
            }
            return arr;
        }

        private static object ParseNumber(string valueStr)
        {
            if (valueStr.Contains("."))
            {
                if (float.TryParse(valueStr, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out float floatResult))
                {
                    return floatResult;
                }
            }
            else
            {
                if (int.TryParse(valueStr, out int intResult))
                {
                    return intResult;
                }
                if (long.TryParse(valueStr, out long longResult))
                {
                    return longResult;
                }
                if (ulong.TryParse(valueStr, out ulong ulongResult))
                {
                    return ulongResult;
                }
            }
            return null;
        }

        private static string ParseInclude(string line)
        {
            var firstQuotePos = line.IndexOf('"');
            var lastQuotePos = line.LastIndexOf('"');
            var include = line.Substring(firstQuotePos + 1, lastQuotePos - firstQuotePos - 1);
            return include;
        }

        public static void Serialize(SiiFile siiFile, string path)
        {
            var str = Serialize(siiFile);
            File.WriteAllText(path, str);
        }

        public static string Serialize(SiiFile siiFile)
        {
            var sb = new StringBuilder();

            if (siiFile.GlobalScope)
            {
                sb.AppendLine(SiiHeader);
                sb.AppendLine("{");
            }

            SerializeIncludes(sb, siiFile.Includes);

            foreach (var unit in siiFile.Units)
            {
                sb.AppendLine($"{unit.Class} : {unit.Name} {{");
                SerializeAttributes(sb, unit);
                SerializeIncludes(sb, unit.Includes);
                sb.AppendLine("}");
            }

            if (siiFile.GlobalScope)
            {
                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        private static void SerializeAttributes(StringBuilder sb, Unit unit)
        {
            foreach (var attrib in unit.Attributes)
            {
                if (attrib.Value is List<object> list)
                {
                    foreach (var entry in list)
                    {
                        sb.Append($"{Indentation}{attrib.Key}[]: ");
                        SerializeAttributeValue(sb, entry);
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

        private static void SerializeAttributeValue(StringBuilder sb, object attribValue)
        {
            // TODO: Finish this method

            if (attribValue is string)
            {
                sb.Append($"\"{attribValue}\"");
            }
            else
            {
                sb.Append(attribValue);
            }
        }

        private static void SerializeIncludes(StringBuilder sb, List<string> includes)
        {
            foreach (var include in includes)
            {
                sb.AppendLine($"{IncludeKeyword} \"{include}\"");
            }
        }
    }
}
