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
            var company = new Company();
            ReadKdopItem(r, company);

            company.CompanyName = r.ReadToken();
            company.CityName = r.ReadToken();

            company.PrefabLink = new UnresolvedItem(r.ReadUInt64());

            company.Node = new UnresolvedNode(r.ReadUInt64());

            company.UnloadPointsEasy = ReadNodeRefList(r);
            company.UnloadPointsMedium = ReadNodeRefList(r);
            company.UnloadPointsHard = ReadNodeRefList(r);
            company.TrailerSpawnPoints = ReadNodeRefList(r);
            company.Unknown1 = ReadNodeRefList(r);
            company.LongTrailerSpawnPoints = ReadNodeRefList(r);

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

            WriteNodeRefList(w, company.UnloadPointsEasy);
            WriteNodeRefList(w, company.UnloadPointsMedium);
            WriteNodeRefList(w, company.UnloadPointsHard);
            WriteNodeRefList(w, company.TrailerSpawnPoints);
            WriteNodeRefList(w, company.Unknown1);
            WriteNodeRefList(w, company.LongTrailerSpawnPoints);
        }
    }
}
