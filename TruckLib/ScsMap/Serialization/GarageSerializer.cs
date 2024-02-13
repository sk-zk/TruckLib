using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class GarageSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var garage = new Garage(false);
            ReadKdopItem(r, garage);

            garage.CityName = r.ReadToken();
            garage.IsBuyPoint = r.ReadUInt32() > 0;
             
            garage.Node = new UnresolvedNode(r.ReadUInt64());
            garage.Prefab = new UnresolvedItem(r.ReadUInt64());
             
            garage.TrailerSpawnPoints = ReadNodeRefList(r);

            return garage;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var garage = item as Garage;
            WriteKdopItem(w, garage);

            w.Write(garage.CityName);
            w.Write(garage.IsBuyPoint ? 1 : 0);
 
            w.Write(garage.Node.Uid);
            w.Write(garage.Prefab.Uid);

            WriteNodeRefList(w, garage.TrailerSpawnPoints);
        }
    }
}
