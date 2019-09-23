using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents the railing section of a Road / Terrain.
    /// </summary>
    public class Railings
    {
        /// <summary>
        /// The railings.
        /// </summary>
        public Railing[] Models { get; set; }

        public bool InvertRailing { get; set; }

        public Railings()
        {
            const int railingCount = 3;
            Models = (new Railing[railingCount]).Select(h => new Railing()).ToArray();
        }
    }
}
