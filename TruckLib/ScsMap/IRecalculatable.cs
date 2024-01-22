using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// An interface for map items which have properties that may need to be recalculated
    /// if other properties change.
    /// </summary>
    interface IRecalculatable
    {
        /// <summary>
        /// Recalculates properties that may need to be recalculated.
        /// </summary>
        void Recalculate();
    }
}
