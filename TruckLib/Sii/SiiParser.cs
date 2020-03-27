using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TruckLib.Sii
{
    /// <summary>
    /// De/serializes a SII file.
    /// </summary>
    internal class SiiParser
    {
        internal string TupleAttribOpen = "(";
        internal string TupleAttribClose = ")";
        private readonly string SiiHeader = "SiiNunit";
        private readonly char TupleSeperator = ',';
        private readonly string IncludeKeyword = "@include";
        private string Indentation = "    "; // no tabs for you.

        public SiiFile DeserializeFromString(string sii)
        {
            var siiFile = new SiiFile();

            sii = RemoveComments(sii);

            siiFile.GlobalScope = sii.StartsWith(SiiHeader);

            // get units
            string unitDeclarations = @"[\w]+?\s*:\s*\""?[\w.]+?\""?\s*{"; // don't @ me
            foreach (Match m in Regex.Matches(sii, unitDeclarations, RegexOptions.Singleline))
            {
                // find ending bracket of this unit
                var start = m.Index;
                var startBracket = start + m.Length; // start after the opening bracket of the unit
                int? end = null;
                var bracketsStack = 0;
                for(int i = startBracket; i < sii.Length; i++)
                {
                    if (sii[i] == '{') bracketsStack++;
                    if (sii[i] == '}') bracketsStack--;

                    if (bracketsStack == -1)
                    {
                        end = i;
                        break;
                    }
                }

                if(end is null)
                {
                    throw new SiiParserException($"Expected '}}' at {sii.Length - 1}, found EOF");
                }
                else
                {
                    siiFile.Units.Add(ParseUnit(sii.Substring(start, end.Value - start + 1)));
                }
            }

            // parse top level includes
            GetTopLevelIncludes(sii, siiFile);

            return siiFile;
        }

        public SiiFile DeserializeFromFile(string path)
        {
            var str = File.ReadAllText(path);
            return DeserializeFromString(str);
        }

        private void GetTopLevelIncludes(string sii, SiiFile siiFile)
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
                    foreach (var character in line)
                    {
                        if (character == '{') ++bracketsStack;
                        if (character == '}') --bracketsStack;
                    }

                    if (bracketsStack == 0 && line.StartsWith(IncludeKeyword))
                    {
                        var include = ParseInclude(line);
                        siiFile.Includes.Add(include);
                    }
                }
            }
        }

        private string RemoveComments(string sii)
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

        private Unit ParseUnit(string unitStr)
        {
            var firstColonPos = unitStr.IndexOf(':');
            var openBracketPos = unitStr.IndexOf('{');
            var closeBracketPos = unitStr.LastIndexOf('}');

            var unit = new Unit
            {
                Class = unitStr.Substring(0, firstColonPos).Trim(),
                Name = unitStr.Substring(firstColonPos + 1,
                openBracketPos - firstColonPos - 1).Trim()
            };

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
                    if (unit.Attributes.TryGetValue(arrName, out var existingAttrib))
                    {
                        (existingAttrib as List<object>).Add(attrib.Value);
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

        private (string Name, object Value) ParseAttribute(string line)
        {
            line = line.Trim();
            var firstColonPos = line.IndexOf(':');
            var attributeName = line.Substring(0, firstColonPos).Trim();
            var valueStr = line.Substring(firstColonPos + 1, line.Length - firstColonPos - 1);
            var value = ParseAttributeValue(valueStr);
            return (attributeName, value);
        }

        private object ParseAttributeValue(string valueStr)
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
            if (valueStr.StartsWith(TupleAttribOpen) && valueStr.EndsWith(TupleAttribClose)
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
            if (valueStr.Contains("."))
            {
                return valueStr;
            }

            // token
            if (Token.IsValidToken(valueStr))
            {
                return new Token(valueStr);
            }

            // unknown or not implemented
            return valueStr;
        }

        private object ParseTuple(string valueStr)
        {
            var tupleVals = valueStr.Substring(1, valueStr.Length - 2)
                .Split(TupleSeperator);

            // determine the type of the tuple
            Type[] types = new Type[tupleVals.Count()];
            for (int i = 0; i < tupleVals.Length; i++)
            {
                types[i] = ParseAttributeValue(tupleVals[i]).GetType();
            }

            if (types.Contains(typeof(float)))
            {
                return TupleValsToArray<float>(tupleVals);
            }
            if (types.Contains(typeof(ulong)))
            {
                return TupleValsToArray<ulong>(tupleVals);
            }
            if (types.Contains(typeof(long)))
            {
                return TupleValsToArray<long>(tupleVals);
            }
            if (types.Contains(typeof(int)))
            {
                return TupleValsToArray<int>(tupleVals);
            }

            return TupleValsToArray<object>(tupleVals); // whatever, object array it is
        }

        private T[] TupleValsToArray<T>(string[] tupleVals)
        {
            var arr = new T[tupleVals.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                if (typeof(T) == typeof(float))
                {
                    arr[i] = (T)(object)float.Parse(tupleVals[i], 
                        NumberStyles.Float, CultureInfo.InvariantCulture);
                }
                else
                {
                    arr[i] = (T)ParseAttributeValue(tupleVals[i]);
                }
            }
            return arr;
        }

        private object ParseNumber(string valueStr)
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
                if (int.TryParse(valueStr, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out int intResult))
                {
                    return intResult;
                }
                if (long.TryParse(valueStr, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out long longResult))
                {
                    return longResult;
                }
                if (ulong.TryParse(valueStr, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out ulong ulongResult))
                {
                    return ulongResult;
                }
            }
            return null;
        }

        private string ParseInclude(string line)
        {
            var firstQuotePos = line.IndexOf('"');
            var lastQuotePos = line.LastIndexOf('"');
            var include = line.Substring(firstQuotePos + 1, lastQuotePos - firstQuotePos - 1);
            return include;
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

        private void SerializeAttributes(StringBuilder sb, Unit unit)
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

        private void SerializeAttributeValue(StringBuilder sb, object attribValue)
        {
            // TODO: Finish this method

            if (attribValue is string)
            {
                sb.Append($"\"{attribValue}\"");
            }
            else if(attribValue is Array arr)
            {
                sb.Append(TupleAttribOpen);
                IList list = arr;
                for (int i = 0; i < list.Count; i++)
                {
                    sb.Append(Convert.ToString(list[i], CultureInfo.InvariantCulture));
                    if (i != list.Count - 1)
                        sb.Append(TupleSeperator);
                }
                sb.Append(TupleAttribClose);
            }
            else
            {
                sb.Append(Convert.ToString(attribValue, CultureInfo.InvariantCulture));
            }
        }

        private void SerializeIncludes(StringBuilder sb, List<string> includes)
        {
            foreach (var include in includes)
            {
                sb.AppendLine($"{IncludeKeyword} \"{include}\"");
            }
        }

    }
}
