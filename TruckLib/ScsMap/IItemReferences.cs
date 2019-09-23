using System;
using System.Collections.Generic;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Interface for classes which hold references to other map items.
    /// </summary>
    internal interface IItemReferences
    {
        void UpdateItemReferences(Dictionary<ulong, MapItem> allItems);
    }
}