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

            company.Prefab = new UnresolvedItem(r.ReadUInt64());

            company.Node = new UnresolvedNode(r.ReadUInt64());

            var spawnPointUids = ReadNodeRefList(r);
            company.SpawnPoints = new(company);
            for (int i = 0; i < spawnPointUids.Count; i++)
            {
                var flags = r.ReadUInt32();
                // TODO does the upper nibble do anything?
                company.SpawnPoints.Add(new CompanySpawnPoint(spawnPointUids[i], flags));
            }

            return company;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var company = item as Company;
            WriteKdopItem(w, company);

            w.Write(company.CompanyName);
            w.Write(company.CityName);

            if (company.Prefab is null)
            {
                w.Write(0UL);
            }
            else
            {
                w.Write(company.Prefab.Uid);
            }

            w.Write(company.Node.Uid);

            w.Write(company.SpawnPoints.Count);
            foreach (var spawnPoint in company.SpawnPoints)
            {
                w.Write(spawnPoint.Node.Uid);
            }
            foreach (var spawnPoint in company.SpawnPoints)
            {
                w.Write(spawnPoint.Flags.Bits);
            }
        }
    }
}
