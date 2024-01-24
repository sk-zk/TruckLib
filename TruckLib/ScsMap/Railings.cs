using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents the railings of a <see cref="Terrain"/>.
    /// </summary>
    public class Railings
    {
        /// <summary>
        /// The railings.
        /// </summary>
        public Railing[] Models { get; set; }

        /// <summary>
        /// Gets or sets whether the models are mirrored along the road axis.
        /// Only applies to the first railing.
        /// </summary>
        public bool InvertRailing { get; set; }

        public Railings()
        {
            const int railingCount = 3;
            Models = (new Railing[railingCount]).Select(h => new Railing()).ToArray();
        }
    }
}
