﻿using System;
using System.Collections.Generic;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Interface for classes which hold references to other map items.
    /// </summary>
    internal interface IItemReferences
    {
        /// <summary>
        /// Searches a list of all map items for the map items referenced by UID in
        /// this map item and updates the respective references. This is used for loading
        /// a map and does not need to be called by the user at any point.
        /// </summary>
        /// <param name="allItems">A dictionary of all items in the entire map.</param>
        void UpdateItemReferences(Dictionary<ulong, MapItem> allItems);
    }
}