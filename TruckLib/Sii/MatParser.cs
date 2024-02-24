using Sprache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TruckLib.Sii
{
    internal static class MatParser
    {
        public static MatFile DeserializeFromString(string mat)
        {
            var firstPass = ParserElements.Mat.Parse(mat);

            var matFile = new MatFile { Effect = firstPass.UnitName };

            foreach (var (key, value) in firstPass.Attributes)
            {
                if (value is FirstPassUnit unit)
                {
                    var attribDict = unit.Attributes
                        .ToDictionary(k => k.Key, v => v.Value);
                    matFile.Textures.Add(new Texture()
                    {
                        Name = unit.UnitName,
                        Attributes = attribDict
                    });
                } 
                else
                {
                    matFile.Attributes.Add(key, value);
                }
            }

            return matFile;
        }

        public static MatFile DeserializeFromFile(string path) =>
            DeserializeFromString(File.ReadAllText(path));

        public static string Serialize(MatFile matFile, string indentation = "\t")
        {
            var sb = new StringBuilder();

            sb.AppendLine($"effect : \"{matFile.Effect}\" {{");

            ParserElements.SerializeAttributes(sb, matFile.Attributes, indentation);
            foreach (var texture in matFile.Textures)
            {
                sb.AppendLine($"{indentation}texture: \"{texture.Name}\" {{");
                ParserElements.SerializeAttributes(sb, texture.Attributes, indentation + indentation, true);
                sb.AppendLine($"{indentation}}}");
            }

            sb.AppendLine("}\n");

            return sb.ToString();
        }

        public static void Serialize(MatFile matFile, string path, string indentation = "\t")
        {
            var str = Serialize(matFile, indentation);
            File.WriteAllText(path, str);
        }

    }
    
}
