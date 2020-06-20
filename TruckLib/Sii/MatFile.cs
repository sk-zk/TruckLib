using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Sii
{
    /// <summary>
    /// A .mat material file.
    /// </summary>
    public class MatFile
    {
        /// <summary>
        /// Name of the shader, e.g. "eut2.dif.spec.rfx".
        /// </summary>
        public string Effect { get; set; }

        /// <summary>
        /// Attributes of the shader.
        /// </summary>
        public Dictionary<string, dynamic> Attributes { get; set; }
            = new Dictionary<string, dynamic>();

        public static MatFile FromString(string mat) =>
            MatParser.DeserializeFromString(mat);

        public static MatFile FromFile(string path) =>
            MatParser.DeserializeFromFile(path);

        public string Serialize() =>
            MatParser.Serialize(this);

        public void Serialize(string path) =>
            MatParser.Serialize(this, path);

    }
}
