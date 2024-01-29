using System;
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
    /// Represents an SII file.
    /// </summary>
    public class SiiFile
    {
        // https://modding.scssoft.com/wiki/Documentation/Engine/Units

        // TODO:
        // Support placement type (currently unused in the game though (?))

        /// <summary>
        /// Units in this file.
        /// </summary>
        public List<Unit> Units { get; set; } = new List<Unit>();

        /// <summary>
        /// Gets or sets if the file has global scope ("<c>SiiNunit {</c>") or not.
        /// </summary>
        public bool GlobalScope { get; set; } = true;

        /// <summary>
        /// Instantiates an empty SII file.
        /// </summary>
        public SiiFile() { }

        /// <summary>
        /// Deserializes a string containing a SII file.
        /// </summary>
        /// <param name="sii">The string containing the SII file.</param>
        /// <param name="siiDirectory">The path of the directory in which the SII file is located.
        /// Required for inserting <c>@include</c>s. Can be ommitted if the file is known not to
        /// have <c>@include</c>s.</param>
        /// <returns>A SiiFile object.</returns>
        public static SiiFile Load(string sii, string siiDirectory = "") =>
            new SiiParser().DeserializeFromString(sii, siiDirectory);

        /// <summary>
        /// Opens a SII file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>A SiiFile object.</returns>
        public static SiiFile Open(string path) =>
            new SiiParser().DeserializeFromFile(path);

        /// <summary>
        /// Serializes this object to a string.
        /// </summary>
        public string Serialize() =>
            new SiiParser().Serialize(this);

        /// <summary>
        /// Serializes this object and writes it to a file.
        /// </summary>
        public void Serialize(string path) =>
            new SiiParser().Serialize(this, path);
    }
}
