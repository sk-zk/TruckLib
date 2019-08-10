using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Terrain vegetation settings.
    /// </summary>
    public class Vegetation
    {
        /// <summary>
        /// The vegetation file used.
        /// </summary>
        public Token VegetationName { get; set; }

        /// <summary>
        /// The scale of vegetation models.
        /// </summary>
        public VegetationScale Scale { get; set; }

        private float density = 400f;
        public float Density
        {
            get => density;
            set => density = Utils.SetIfInRange(value, 0f, 6553.5f);
        }
    }
}
