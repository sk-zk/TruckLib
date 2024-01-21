using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents the k-DOP bounding box of a map item.
    /// </summary>
    public class KdopBounds
    {
        private const int arrSize = 5;

        /// <summary>
        /// Minimums of the k-DOP bounding box.
        /// </summary>
        public float[] Minimums { get; set; }

        /// <summary>
        /// Maximums of the k-DOP bounding box.
        /// </summary>
        public float[] Maximums { get; set; }

        /// <summary>
        /// Instantiates a KdopBounds object.
        /// </summary>
        public KdopBounds()
        {
            Minimums = new float[arrSize];
            Maximums = new float[arrSize];

            Init();
        }

        internal KdopBounds(bool initFields)
        {
            Minimums = new float[arrSize];
            Maximums = new float[arrSize];

            if (initFields) Init();
        }

        /// <summary>
        /// Sets the arrays to safe default values.
        /// If too many items with all 0s are loaded, the game will crash.
        /// </summary>
        protected void Init()
        {
            Minimums[0] = 1;
            Minimums[1] = 1;
            Minimums[2] = 1;
            Maximums[0] = 2;
            Maximums[1] = 2;
            Maximums[2] = 2;
        }
    }
}
