using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents the railings of a <see cref="Road"/>.
    /// </summary>
    public class RoadRailings : Railings
    {
        /// <summary>
        /// Gets or sets whether a mirrored version of the models is placed on the
        /// opposite side of the road. Only applies to the first railing.
        /// </summary>
        public bool DoubleSided { get; set; }

        /// <summary>
        /// Gets or sets whether only the center part of the model is placed.
        /// Only applies to the second railing.
        /// </summary>
        public bool CenterPartOnly { get; set; }

        /// <summary>
        /// Makes a deep copy of this object.
        /// </summary>
        /// <returns>A deep copy of this object.</returns>
        public RoadRailings Clone()
        {
            var rr = (RoadRailings)MemberwiseClone();
            rr.Models = (Railing[])Models.Clone();
            return rr;
        }
    }
}
