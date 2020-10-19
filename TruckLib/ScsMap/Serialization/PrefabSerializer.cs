using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class PrefabSerializer : MapItemSerializer, IDataPayload
    {
        private const float terrainSizeFactor = 10f;
        private const float vegFromToFactor = 10f;

        public override MapItem Deserialize(BinaryReader r)
        {
            var pf = new Prefab(false);
            ReadKdopItem(r, pf);

            pf.Model = r.ReadToken();
            pf.Variant = r.ReadToken();

            pf.AdditionalParts = ReadObjectList<Token>(r);

            pf.Nodes = ReadNodeRefList(r);

            // Slave uids
            // (link to service items)
            pf.SlaveItems = ReadItemRefList(r);

            var ferryLinkUid = r.ReadUInt64();
            if (ferryLinkUid != 0)
            {
                pf.FerryLink = new UnresolvedItem(ferryLinkUid);
            }

            pf.Origin = r.ReadUInt16();

            foreach (var corner in pf.Corners)
            {
                corner.Terrain = new RoadTerrain(false);
            }

            for (int i = 0; i < pf.Nodes.Count; i++)
            {
                pf.Corners[i].Terrain.Profile = r.ReadToken();
                pf.Corners[i].Terrain.Coefficient = r.ReadSingle();
            }

            pf.SemaphoreProfile = r.ReadToken();

            return pf;
        }

        public void DeserializeDataPayload(BinaryReader r, MapItem item)
        {
            var pf = item as Prefab;
            pf.Look = r.ReadToken();

            foreach (var corner in pf.Corners)
            {
                corner.Terrain.Size = r.ReadUInt16() / terrainSizeFactor;

                corner.DetailVegetationFrom = r.ReadUInt16() / vegFromToFactor;
                corner.DetailVegetationTo = r.ReadUInt16() / vegFromToFactor;

                foreach (var veg in corner.Vegetation)
                {
                    veg.Deserialize(r);
                }
            }

            pf.VegetationParts = ReadObjectList<VegetationPart>(r);

            pf.RandomSeed = r.ReadUInt32();

            pf.VegetationSpheres = ReadObjectList<VegetationSphere>(r);

            foreach (var corner in pf.Corners)
            {
                corner.Model = r.ReadToken();
                corner.Variant = r.ReadToken();
                corner.Look = r.ReadToken();
            }

            foreach (var corner in pf.Corners)
            {
                corner.Terrain.QuadData = new TerrainQuadData(false);
                corner.Terrain.QuadData.Deserialize(r);
            }
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var pf = item as Prefab;
            WriteKdopItem(w, pf);

            w.Write(pf.Model);
            w.Write(pf.Variant);

            WriteObjectList(w, pf.AdditionalParts);

            WriteNodeRefList(w, pf.Nodes);

            WriteItemRefList(w, pf.SlaveItems);

            if (pf.FerryLink is null)
            {
                w.Write(0UL);
            }
            else
            {
                w.Write(pf.FerryLink.Uid);
            }

            w.Write(pf.Origin);

            for (int i = 0; i < pf.Nodes.Count; i++)
            {
                w.Write(pf.Corners[i].Terrain.Profile);
                w.Write(pf.Corners[i].Terrain.Coefficient);
            }

            w.Write(pf.SemaphoreProfile);
        }

        public void SerializeDataPayload(BinaryWriter w, MapItem item)
        {
            var pf = item as Prefab;
            w.Write(pf.Look);

            foreach (var corner in pf.Corners)
            {
                w.Write((ushort)(corner.Terrain.Size * terrainSizeFactor));

                w.Write((ushort)(corner.DetailVegetationFrom * vegFromToFactor));
                w.Write((ushort)(corner.DetailVegetationTo * vegFromToFactor));

                foreach (var veg in corner.Vegetation)
                {
                    veg.Serialize(w);
                }
            }

            WriteObjectList(w, pf.VegetationParts);

            w.Write(pf.RandomSeed);

            WriteObjectList(w, pf.VegetationSpheres);

            foreach (var corner in pf.Corners)
            {
                w.Write(corner.Model);
                w.Write(corner.Variant);
                w.Write(corner.Look);
            }

            foreach (var corner in pf.Corners)
            {
                corner.Terrain.QuadData.Serialize(w);
            }
        }
    }
}
