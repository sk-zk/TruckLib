using Sprache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("TruckLibTests")]
namespace TruckLib.Sii
{
    /// <summary>
    /// De/serializes a SII file.
    /// </summary>
    internal static class SiiParser
    {
        private const string IncludeKeyword = "@include";

        /// <summary>
        /// Sets how to handle duplicate attributes in a unit.
        /// </summary>
        private static readonly bool OverrideOnDuplicate = true;

        private static readonly Dictionary<string, int> arrInsertIndex = new();

        public static SiiFile DeserializeFromString(string sii, string siiPath = "")
        {
            var siiFile = new SiiFile();

            sii = RemoveComments(sii);
            sii = InsertIncludes(sii, siiPath);

            var firstPassUnits = ParserElements.Sii.Parse(sii);
            foreach (var firstPassUnit in firstPassUnits)
            {
                siiFile.Units.Add(SecondPass(firstPassUnit));
            }

            return siiFile;
        }

        private static Unit SecondPass(FirstPassUnit firstPass)
        {
            arrInsertIndex.Clear();
            var secondPass = new Unit(firstPass.ClassName, firstPass.UnitName);
            foreach (var (key, value) in firstPass.Attributes)
            {
                if (key.EndsWith("]"))
                    ParseListOrArrayAttribute(secondPass, key, value);
                else
                    AddAttribute(secondPass, key, value);
            }
            return secondPass;
        }

        private static string InsertIncludes(string sii, string siiPath)
        {
            var output = new StringBuilder();

            using var reader = new StringReader(sii);
            string line;
            while ((line = reader.ReadLine()) is not null)
            {
                if (!line.StartsWith(IncludeKeyword))
                {
                    output.AppendLine(line);
                }
                else
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
                    }
                }
            }

            return output.ToString();
        }

        public static SiiFile DeserializeFromFile(string path) =>
            DeserializeFromString(File.ReadAllText(path), Path.GetDirectoryName(path));

        private static string RemoveComments(string sii) =>
            Regex.Replace(sii,
                // 🠋 remove C-style comments
                //           🠋 remove # comments
                //                      🠋 remove // comments
                @"\/\*.*\*\/|#[^\n\r]*|\/\/[^\n\r]*",
                "",
                RegexOptions.Singleline);

        private static void AddAttribute(Unit unit, string name, dynamic value)
        {
            if (unit.Attributes.ContainsKey(name) && OverrideOnDuplicate)
                unit.Attributes[name] = value;
            else
                unit.Attributes.Add(name, value);
        }

        private static void ParseListOrArrayAttribute(Unit unit, string name, dynamic value)
        {
            var match = Regex.Match(name, @"^(.+)\[(.*)\]$");
            if (!match.Success)
                throw new ArgumentException();

            var arrName = match.Groups[1].Value;
            var hasArrIndex = int.TryParse(match.Groups[2].Value, out int arrIndex);
            if (!arrInsertIndex.ContainsKey(arrName))
                arrInsertIndex.Add(arrName, 0);

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

        public static void Save(SiiFile siiFile, string path, string indentation = "\t")
        {
            var str = Serialize(siiFile, indentation);
            File.WriteAllText(path, str);
        }

        public static string Serialize(SiiFile siiFile, string indentation = "\t")
        {
            var sb = new StringBuilder();

            sb.AppendLine(ParserElements.SiiHeader);
            sb.AppendLine("{\n");

            foreach (var unit in siiFile.Units)
            {
                sb.AppendLine($"{unit.Class} : {unit.Name}\n{{");
                ParserElements.SerializeAttributes(sb, unit.Attributes, indentation);
                sb.AppendLine("}\n");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
