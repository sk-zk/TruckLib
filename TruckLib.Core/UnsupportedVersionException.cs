using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    /// <summary>
    /// Thrown if the version of a file format is not supported by this library.
    /// </summary>
    public class UnsupportedVersionException : Exception
    {
        /// <inheritdoc/>
        public UnsupportedVersionException(string message) : base(message)
        {
        }
    }
}
