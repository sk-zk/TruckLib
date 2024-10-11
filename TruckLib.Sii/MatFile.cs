using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Sii
{
    /// <summary>
    /// Represents a .mat file.
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
        /// Textures of the shader.
        /// </summary>
        public List<Texture> Textures { get; set; } = new List<Texture>();

        /// <summary>
        /// Deserializes a .mat file.
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
        /// <param name="indentation">The string which will be used as one level of indentation.</param>
        /// <returns>The serialized object.</returns>
        public string Serialize(string indentation = "\t") =>
            MatParser.Serialize(this, indentation);

        /// <summary>
        /// Serializes this object and writes it to a file.
        /// </summary>
        /// <param name="path">The output path.</param>
        /// <param name="indentation">The string which will be used as one level of indentation.</param>
        public void Serialize(string path, string indentation = "\t") =>
            MatParser.Serialize(this, path, indentation);
    }
}
