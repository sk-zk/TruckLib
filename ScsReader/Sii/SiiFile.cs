using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScsReader.Sii
{

    /// <summary>
    /// Represents an SII file.
    /// </summary>
    public class SiiFile
    {
        // https://modding.scssoft.com/wiki/Documentation/Engine/Units

        // TODO:
        // Support fixed length arrays
        // Support placement type
        // Remove block comments

        /// <summary>
        /// Units in this file.
        /// </summary>
        public List<Unit> Units = new List<Unit>();

        /// <summary>
        /// Top-level includes in this file.
        /// </summary>
        public List<string> Includes = new List<string>();

        /// <summary>
        /// Sets if the file has global scope ("SiiNunit {") or not.
        /// </summary>
        public bool GlobalScope = true;

        public SiiFile()
        {
        }

        /// <summary>
        /// Deserializes a string containing a SII file.
        /// </summary>
        /// <param name="sii"></param>
        /// <returns></returns>
        public static SiiFile FromString(string sii)
        {
            return SiiParser.DeserializeFromString(sii);
        }

        /// <summary>
        /// Deserializes a SII file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SiiFile FromFile(string path)
        {
            return SiiParser.DeserializeFromFile(path);
        }

        /// <summary>
        /// Serializes this object to a string.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return SiiParser.Serialize(this);
        }

        /// <summary>
        /// Serializes this object to a file.
        /// </summary>
        /// <param name="path"></param>
        public void Serialize(string path)
        {
            SiiParser.Serialize(this, path);
        }
    }
}
