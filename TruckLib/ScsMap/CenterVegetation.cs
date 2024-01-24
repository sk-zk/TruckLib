using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of vegetation in the center of a dual carriageway <see cref="Road">road</see>.
    /// </summary>
    public class CenterVegetation : Vegetation
    {
        private float offset;
        /// <summary>
        /// Offset from the center of the road, in meters.
        /// </summary>
        public float Offset
        {
            get => offset;
            set => offset = Utils.SetIfInRange(value, 0, 25);
        }

        public CenterVegetation Clone() => (CenterVegetation)MemberwiseClone();
    }
}
