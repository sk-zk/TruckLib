using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap
{
    public class KdopBounds
    {
        private const int arrSize = 5;

        /// <summary>
        /// Minimums of the kDOP bounding box which is used for rendering and collision detection.
        /// </summary>
        public float[] Minimums { get; set; }

        /// <summary>
        /// Maximums of the kDOP bounding box which is used for rendering and collision detection.
        /// </summary>
        public float[] Maximums { get; set; }

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
