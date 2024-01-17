using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Sii
{
    /// <summary>
    /// Represents a .mat material file.
    /// </summary>
    public class MatFile
    {
        /// <summary>
        /// Name of the shader, e.g. <c>eut2.dif.spec.rfx</c>.
        /// </summary>
        public string Effect { get; set; }

        /// <summary>
        /// Attributes of the shader.
        /// </summary>
        public Dictionary<string, dynamic> Attributes { get; set; }
            = new Dictionary<string, dynamic>();

        /// <summary>
        /// Deserializes a string containing a .mat file.
        /// </summary>
        /// <param name="mat">The string containing the .mat file.</param>
        /// <returns>A MatFile object.</returns>
        public static MatFile Load(string mat) =>
            MatParser.DeserializeFromString(mat);

        /// <summary>
        /// Opens a .mat file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>A MatFile object.</returns>
        public static MatFile Open(string path) =>
            MatParser.DeserializeFromFile(path);

        /// <summary>
        /// Serializes this object to a string.
        /// </summary>
        /// <returns>The serialized object.</returns>
        public string Serialize() =>
            MatParser.Serialize(this);

        /// <summary>
        /// Serializes this object and writes it to a file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public void Serialize(string path) =>
            MatParser.Serialize(this, path);
    }
}
