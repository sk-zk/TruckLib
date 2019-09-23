using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Model.Ppd
{
    /// <summary>
    /// Prism Prefab Descriptor.
    /// </summary>
    public class PpdFile : IBinarySerializable
    {
        private uint Version = 0x16;

        public List<ControlNode> Nodes { get; set; } = new List<ControlNode>();

        public List<NavCurve> NavCurves { get; set; } = new List<NavCurve>();

        public List<Sign> Signs { get; set; } = new List<Sign>();

        public List<Semaphore> Semaphores { get; set; } = new List<Semaphore>();

        public List<SpawnPoint> SpawnPoints { get; set; } = new List<SpawnPoint>();

        public List<Vector3> TerrainPointPositions { get; set; } = new List<Vector3>();

        public List<Vector3> TerrainPointNormals { get; set; } = new List<Vector3>();

        public List<TerrainPointVariant> TerrainPointVariants { get; set; } = new List<TerrainPointVariant>();

        public List<MapPoint> MapPoints { get; set; } = new List<MapPoint>();

        public List<TriggerPoint> TriggerPoints { get; set; } = new List<TriggerPoint>();

        public List<Intersection> Intersections { get; set; } = new List<Intersection>();

        public List<uint[]> Unknown { get; set; } = new List<uint[]>();

        public void Open(string path)
        {
            using(var r = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                ReadFromStream(r);
            }
        }

        public void ReadFromStream(BinaryReader r)
        {
            Version = r.ReadUInt32();

            var nodeCount = r.ReadUInt32();
            var navCurveCount = r.ReadUInt32();
            var signCount = r.ReadUInt32();
            var semaphoreCount = r.ReadUInt32();
            var spawnPointCount = r.ReadUInt32();
            var terrainPointCount = r.ReadUInt32();
            var terrainPointVariantCount = r.ReadUInt32();
            var mapPointCount = r.ReadUInt32();
            var triggerPointCount = r.ReadUInt32();
            var intersectionCount = r.ReadUInt32();
            var newdata1Count = r.ReadUInt32();

            var nodeOffset = r.ReadUInt32();
            var navCurveOffset = r.ReadUInt32();
            var signOffset = r.ReadUInt32();
            var semaphoreOffset = r.ReadUInt32();
            var spawnPointOffset = r.ReadUInt32();
            var terrainPointPosOffset = r.ReadUInt32();
            var terrainPointNormalOffset = r.ReadUInt32();
            var terrainPointVariantOffset = r.ReadUInt32();
            var mapPointOffset = r.ReadUInt32();
            var triggerPointOffset = r.ReadUInt32();
            var intersectionOffset = r.ReadUInt32();
            var newdata1Offset = r.ReadUInt32();

            Nodes = r.ReadObjectList<ControlNode>(nodeCount);
            NavCurves = r.ReadObjectList<NavCurve>(navCurveCount);
            Signs = r.ReadObjectList<Sign>(signCount);
            Semaphores = r.ReadObjectList<Semaphore>(semaphoreCount);
            SpawnPoints = r.ReadObjectList<SpawnPoint>(spawnPointCount);
            TerrainPointPositions = r.ReadObjectList<Vector3>(terrainPointCount);
            TerrainPointNormals = r.ReadObjectList<Vector3>(terrainPointCount);
            TerrainPointVariants = r.ReadObjectList<TerrainPointVariant>(terrainPointVariantCount);
            MapPoints = r.ReadObjectList<MapPoint>(mapPointCount);
            TriggerPoints = r.ReadObjectList<TriggerPoint>(triggerPointCount);
            Intersections = r.ReadObjectList<Intersection>(intersectionCount);

            // TODO: What is this?
            for(int i = 0; i < newdata1Count; i++)
            {
                var newdata = new uint[24];
                for(int j = 0; j < newdata.Length; j++)
                {
                    newdata[j] = r.ReadUInt32();
                }
                Unknown.Add(newdata);
            }
        }

        public void WriteToStream(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
