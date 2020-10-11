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
        // Remove block comments

        /// <summary>
        /// Units in this file.
        /// </summary>
        public List<Unit> Units { get; set; } = new List<Unit>();

        /// <summary>
        /// Top-level includes in this file.
        /// </summary>
        public List<string> Includes { get; set; } = new List<string>();

        /// <summary>
        /// Sets if the file has global scope ("SiiNunit {") or not.
        /// </summary>
        public bool GlobalScope { get; set; } = true;

        public SiiFile() { }

        /// <summary>
        /// Deserializes a string containing a SII file.
        /// </summary>
        public static SiiFile Load(string sii) =>
            new SiiParser().DeserializeFromString(sii);

        /// <summary>
        /// Deserializes a SII file.
        /// </summary>
        public static SiiFile Open(string path) =>
            new SiiParser().DeserializeFromFile(path);

        /// <summary>
        /// Serializes this object to a string.
        /// </summary>
        public string Serialize() =>
            new SiiParser().Serialize(this);

        /// <summary>
        /// Serializes this object to a file.
        /// </summary>
        public void Serialize(string path) =>
            new SiiParser().Serialize(this, path);
    }
}
