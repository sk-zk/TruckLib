using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Holds k-DOP bounding box values.
    /// </summary>
    public class KdopBounds : IBinarySerializable
    {
        public float[] Minimums { get; set; } = new float[5];
        public float[] Maximums { get; set; } = new float[5];

        public KdopBounds()
        {
            // If there are too many empty or invalid kDOP items, the game
            // will log this error a few hundred times and then crash:
            // "Protos to render array overflow! view_id: 0x1"
            // To avoid crashing the game until you can hit Recompute,
            // all new kDOP items are set to these arbitrary temp values.
            Minimums[0] = 1;
            Minimums[1] = 1;
            Minimums[2] = 1;
            Maximums[0] = 2;
            Maximums[1] = 2;
            Maximums[2] = 2;
        }

        public void Deserialize(BinaryReader r)
        {
            foreach(var arr in new float[][] { Minimums, Maximums })
            {
                for (int i = 0; i < 5; i++)
                {
                    arr[i] = r.ReadSingle();
                }
            }
        }

        public void Serialize(BinaryWriter w)
        {
            foreach (var arr in new float[][] { Minimums, Maximums })
            {
                for (int i = 0; i < 5; i++)
                {
                    w.Write(arr[i]);
                }
            }
        }

    }
}
