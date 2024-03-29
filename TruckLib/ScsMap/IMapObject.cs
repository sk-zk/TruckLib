﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// An interface implemented by both types of objects in a map: 
    /// <see cref="MapItem">items</see> and <see cref="Node">nodes</see>.
    /// </summary>
    public interface IMapObject
    {
        /// <summary>
        /// Gets or sets the UID of the object.
        /// </summary>
        ulong Uid { get; set; }
    }
}
