using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Represents the railing section of a Road / Terrain.
    /// </summary>
    public class Railings
    {
        /// <summary>
        /// The railings.
        /// </summary>
        public RoadRailing[] Models;

        public bool InvertRailing;

        public Railings()
        {
            const int railingCount = 3;
            Models = (new RoadRailing[railingCount]).Select(h => new RoadRailing()).ToArray();
        }
    }
}
