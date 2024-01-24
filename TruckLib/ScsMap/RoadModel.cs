using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of a road model.
    /// </summary>
    public class RoadModel
    {
        /// <summary>
        /// Unit name of the model.
        /// </summary>
        public Token Name { get; set; }

        private float distance = 40;
        /// <summary>
        /// The spacing at which the model repeats, in meters.
        /// </summary>
        public float Distance
        {
            get => distance;
            set => distance = Utils.SetIfInRange(value, 0, 327.66f);
        }

        private float offset = 0;
        /// <summary>
        /// Offset from the center of the road, in meters.
        /// </summary>
        public float Offset
        {
            get => offset;
            set => offset = Utils.SetIfInRange(value, -327.66f, 327.66f);
        }

        /// <summary>
        /// Gets or sets whether the models are shifted by 1/2 of
        /// <see cref="RoadModel.Distance">Distance</see>.
        /// </summary>
        public bool Shift { get; set; } = false;

        /// <summary>
        /// Gets or sets whether the model is rotated by 180°.
        /// </summary>
        public bool Flip { get; set; } = false;

        /// <summary>
        /// Makes a deep copy of this object.
        /// </summary>
        /// <returns>A deep copy of this object.</returns>
        public RoadModel Clone()
        {
            return (RoadModel)MemberwiseClone();
        }
    }
}
