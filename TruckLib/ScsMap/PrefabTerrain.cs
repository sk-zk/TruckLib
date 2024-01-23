using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Properties of procedurally generated terrain of one edge of a <see cref="Prefab"/>. 
    /// </summary>
    public class PrefabTerrain : EdgeTerrain
    {
        /// <summary>
        /// Instantiates a PrefabTerrain with its default values.
        /// </summary>
        public PrefabTerrain()
        {
            Init();
        }

        internal PrefabTerrain(bool initFields)
        {
            if (initFields) Init();
        }

        /// <summary>
        /// Sets the PrefabTerrain's properties to its default values.
        /// </summary>
        protected override void Init()
        {
            base.Init();
        }
    }
}
