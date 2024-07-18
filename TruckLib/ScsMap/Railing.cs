using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents one railing model of a road.
    /// </summary>
    public struct Railing
    {
        /// <summary>
        /// Unit name of the model, as defined in <c>/def/world/railing.sii</c>.
        /// </summary>
        public Token Model { get; set; }

        private float offset;
        /// <summary>
        /// Offset from the center of the road, in meters.
        /// </summary>
        public float Offset
        {
            get => offset;
            set => offset = Utils.SetIfInRange(value, -327.66f, 327.66f);
        }
    }

}
