using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Vegetation in the center of a road.
    /// </summary>
    public class CenterVegetation : Vegetation
    {
        private float offset;
        /// <summary>
        /// Offset from the center of the road.
        /// </summary>
        public float Offset
        {
            get => offset;
            set => offset = Utils.SetIfInRange(value, 0, 25);
        }
    }
}
