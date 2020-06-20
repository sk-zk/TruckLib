using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.Sii
{
    internal static class MatParser
    {
        public static MatFile DeserializeFromString(string matStr)
        {
            var siiParser = new SiiParser
            {
                TupleAttribOpen = "{",
                TupleAttribClose = "}"
            };
            var sii = siiParser.DeserializeFromString(matStr);

            var mat = new MatFile
            {
                Attributes = sii.Units[0].Attributes,
                Effect = sii.Units[0].Name.Replace("\"", "")
            };
            return mat;
        }

        public static MatFile DeserializeFromFile(string path) =>
            DeserializeFromString(File.ReadAllText(path));

        public static string Serialize(MatFile mat)
        {
            var siiParser = new SiiParser
            {
                TupleAttribOpen = "{",
                TupleAttribClose = "}"
            };

            var siiFile = new SiiFile();
            siiFile.GlobalScope = false;
            var unit = new Unit();
            siiFile.Units.Add(unit);
            unit.Class = "material";
            unit.Name = $"\"{mat.Effect}\"";
            unit.Attributes = mat.Attributes;

            return siiParser.Serialize(siiFile);
        }

        public static void Serialize(MatFile mat, string path)
        {
            var str = Serialize(mat);
            File.WriteAllText(path, str);
        }

    }
    
}
