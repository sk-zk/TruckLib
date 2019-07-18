using System;
using System.Collections.Generic;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Interface for classes which hold references to other map items.
    /// </summary>
    internal interface IItemReferences
    {
        void UpdateItemReferences(Dictionary<ulong, MapItem> allItems);
    }
}