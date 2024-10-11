using System.Collections.Generic;

namespace TruckLib.Sii
{
    /// <summary>
    /// Represents an SII file.
    /// </summary>
    public class SiiFile
    {
        // https://modding.scssoft.com/wiki/Documentation/Engine/Units

        /// <summary>
        /// Units in this file.
        /// </summary>
        public List<Unit> Units { get; set; } = new List<Unit>();

        /// <summary>
        /// Instantiates an empty SII file.
        /// </summary>
        public SiiFile() { }

        /// <summary>
        /// Deserializes a SII file.
        /// </summary>
        /// <param name="sii">The string containing the SII file.</param>
        /// <param name="siiDirectory">The path of the directory in which the SII file is located.
        /// Required for inserting <c>@include</c>s. Can be omitted if the file is known not to
        /// have <c>@include</c>s.</param>
        /// <returns>A SiiFile object.</returns>
        public static SiiFile Load(string sii, string siiDirectory = "") =>
            SiiParser.DeserializeFromString(sii, siiDirectory);

        /// <summary>
        /// Opens a SII file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>A SiiFile object.</returns>
        public static SiiFile Open(string path) =>
            SiiParser.DeserializeFromFile(path);

        /// <summary>
        /// Serializes this object to a string.
        /// </summary>
        /// <param name="indentation">The indentation inside units.</param>
        public string Serialize(string indentation = "\t") =>
            SiiParser.Serialize(this, indentation);

        /// <summary>
        /// Serializes this object and writes it to a file.
        /// </summary>
        /// <param name="path">The output path.</param>
        /// <param name="indentation">The indentation inside units.</param>
        public void Save(string path, string indentation = "\t") =>
            SiiParser.Save(this, path, indentation);
    }
}
