using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class CompanySerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var company = new Company(false);
            ReadKdopItem(r, company);

            company.CompanyName = r.ReadToken();
            company.CityName = r.ReadToken();

            company.PrefabLink = new UnresolvedItem(r.ReadUInt64());

            company.Node = new UnresolvedNode(r.ReadUInt64());

            var spawnPointUids = ReadNodeRefList(r);
            company.SpawnPoints = new List<CompanySpawnPoint>(spawnPointUids.Count);
            for (int i = 0; i < spawnPointUids.Count; i++)
            {
                var type = r.ReadUInt32();
                // TODO does the upper nibble do anything?
                company.SpawnPoints.Add(new CompanySpawnPoint(
                    spawnPointUids[i], 
                    (CompanySpawnPointType)(type & 0x0F)));
            }

            return company;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var company = item as Company;
            WriteKdopItem(w, company);

            w.Write(company.CompanyName);
            w.Write(company.CityName);

            w.Write(company.PrefabLink.Uid);

            w.Write(company.Node.Uid);

            w.Write(company.SpawnPoints.Count);
            foreach (var spawnPoint in company.SpawnPoints)
            {
                w.Write(spawnPoint.Node.Uid);
            }
            foreach (var spawnPoint in company.SpawnPoints)
            {
                w.Write(0xF0 + (int)spawnPoint.Type);
            }
        }
    }
}
