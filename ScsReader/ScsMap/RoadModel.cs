using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// A road model.
    /// </summary>
    public class RoadModel
    {
        /// <summary>
        /// The name of the model.
        /// </summary>
        public Token ModelName;

        private float distance = 40;
        public float Distance
        {
            get => distance;
            set => distance = Utils.SetIfInRange(value, 0, 327.66f);
        }

        private float offset = 0;
        public float Offset
        {
            get => offset;
            set => offset = Utils.SetIfInRange(value, -327.66f, 327.66f);
        }

        public bool Shift = false;

        public bool Flip = false;

        public RoadModel Clone()
        {
            return (RoadModel)MemberwiseClone();
        }
    }
}
