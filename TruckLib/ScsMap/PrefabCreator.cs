using TruckLib.Model.Ppd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TruckLib.ScsMap
{
    internal class PrefabCreator
    {
        private PrefabDescriptor ppd;
        private Vector3 Node0Pos => ppd.Nodes[0].Position;
        private IItemContainer map;
        private Vector3 prefabPos;
        private Quaternion prefabRot;
        private Prefab prefab;
        private List<SpawnPoint> clonedPoints;

        public Prefab FromPpd(IItemContainer map, string unitName, PrefabDescriptor ppd,
            Vector3 pos, Quaternion rot)
        {
            this.map = map;
            this.ppd = ppd;
            this.prefabPos = pos;
            this.prefabRot = rot;

            prefab = new Prefab
            {
                Model = unitName,
            };

            // create map nodes from ppd
            CreateMapNodes();

            if (ppd.SpawnPoints.Count > 0)
            {
                CreateSlaveItems();
            }

            map.AddItem(prefab);
            return prefab;
        }

        private void CreateSlaveItems()
        {
            clonedPoints = new List<SpawnPoint>(ppd.SpawnPoints);

            if (Has(SpawnPointType.CompanyPoint))
            {
                CreateCompany();
            }
            if (Has(SpawnPointType.GaragePoint))
            {
                CreateGarage();
            }
            if (Has(SpawnPointType.TruckDealer))
            {
                CreateTruckDealer();
            }

            //////

            if (Has(SpawnPointType.BusStation))
            {
                CreateSlaveItem<BusStop>(SpawnPointType.BusStation);
            }
            if (Has(SpawnPointType.GasStation))
            {
                CreateServiceItemsOfType(SpawnPointType.GasStation,
                    ServiceType.GasStation);
            }
            if (Has(SpawnPointType.Parking))
            {
                CreateServiceItemsOfType(SpawnPointType.Parking,
                    ServiceType.Parking);
            }
            if (Has(SpawnPointType.Recruitment))
            {
                CreateServiceItemsOfType(SpawnPointType.Recruitment,
                    ServiceType.Recruitment);
            }
            if (Has(SpawnPointType.ServiceStation))
            {
                CreateServiceItemsOfType(SpawnPointType.ServiceStation,
                    ServiceType.ServiceStation);
            }
            if (Has(SpawnPointType.WeighStation))
            {
                CreateServiceItemsOfType(SpawnPointType.WeighStation,
                    ServiceType.WeighStation);
            }
            if (Has(SpawnPointType.WeighStationCat))
            {
                CreateServiceItemsOfType(SpawnPointType.WeighStationCat, 
                    ServiceType.WeighStationCat);
            }

            if(clonedPoints.Count > 0)
            {
                Trace.WriteLine($"Unhandled spawn points in {prefab.Model.String}");
            }
        }

        /// <summary>
        /// Creates a Company item for company prefabs.
        /// </summary>
        private void CreateCompany()
        {
            // create company item
            var company = CreateSlaveItem<Company>(SpawnPointType.CompanyPoint);

            // create spawn point nodes
            company.SpawnPoints.AddRange(
                CreateSpawnPointNodes(company, SpawnPointType.UnloadEasy)
                .Select(x => new CompanySpawnPoint(x, (uint)CompanySpawnPointType.UnloadEasy))
                .ToList());
            company.SpawnPoints.AddRange(
                CreateSpawnPointNodes(company, SpawnPointType.UnloadMedium)
                .Select(x => new CompanySpawnPoint(x, (uint)CompanySpawnPointType.UnloadMedium))
                .ToList());
            company.SpawnPoints.AddRange(
                CreateSpawnPointNodes(company, SpawnPointType.UnloadHard)
                .Select(x => new CompanySpawnPoint(x, (uint)CompanySpawnPointType.UnloadHard))
                .ToList());
            company.SpawnPoints.AddRange(
                CreateSpawnPointNodes(company, SpawnPointType.Trailer)
                .Select(x => new CompanySpawnPoint(x, (uint)CompanySpawnPointType.Trailer))
                .ToList());
            company.SpawnPoints.AddRange(
                CreateSpawnPointNodes(company, SpawnPointType.LongTrailer)
                .Select(x => new CompanySpawnPoint(x, (uint)CompanySpawnPointType.Trailer))
                .ToList());
        }

        private void CreateGarage()
        {
            var garage = CreateSlaveItem<Garage>(SpawnPointType.GaragePoint);
            garage.BuyMode = 0;
            garage.TrailerSpawnPoints = CreateSpawnPointNodes(garage, SpawnPointType.TrailerSpawn);

            var buy = CreateSlaveItem<Garage>(SpawnPointType.BuyPoint);
            buy.BuyMode = 1;

            CreateSlaveItem<FuelPump>(SpawnPointType.GasStation);
        }

        private void CreateTruckDealer()
        {
            var dealer = CreateSlaveItem<Service>(SpawnPointType.TruckDealer);
            dealer.ServiceType = ServiceType.TruckDealer;
            dealer.Nodes = CreateSpawnPointNodes(dealer, SpawnPointType.TrailerSpawn);

            CreateServiceItemsOfType(SpawnPointType.ServiceStation,
                ServiceType.ServiceStation);
        }

        private void CreateServiceItemsOfType(SpawnPointType type, ServiceType serviceType)
        {
            foreach (var point in clonedPoints.Where(x => x.Type == type))
            {
                var item = CreateSlaveItem<Service>(point);
                item.ServiceType = serviceType;
            }
            clonedPoints.RemoveAll(x => x.Type == type);
        }

        /// <summary>
        /// Creates slave item of the first spawnpoint of the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        private T CreateSlaveItem<T>(SpawnPointType type) where T : PrefabSlaveItem, new()
        {
            var first = clonedPoints.First(x => x.Type == type);
            var item = CreateSlaveItem<T>(first);
            clonedPoints.Remove(first);
            return item;
        }

        /// <summary>
        /// Creates slave item for the given spawnpoint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spawnPoint"></param>
        /// <returns></returns>
        private T CreateSlaveItem<T>(SpawnPoint spawnPoint) where T : PrefabSlaveItem, new()
        {
            var pos = GetAbsolutePosition(spawnPoint.Position);

            var item = PrefabSlaveItem.Add<T>(map, prefab, pos);
            item.Node.Rotation = spawnPoint.Rotation * prefabRot;
            item.Node.ForwardItem = item;

            return item;
        }

        /// <summary>
        /// Creates map nodes for all spawn points of the given type.
        /// </summary>
        /// <param name="item">The company item.</param>
        /// <param name="spawnPointType">The spawn point type.</param>
        /// <param name="node0Pos">The ppd position of the prefab's red control node.</param>
        /// <returns>A list of map nodes.</returns>
        private List<INode> CreateSpawnPointNodes(PrefabSlaveItem item, SpawnPointType spawnPointType)
        {
            var selected = clonedPoints.Where(x => x.Type == spawnPointType).ToList();
            var list = new List<INode>(selected.Count);
            foreach (var spawnPoint in selected)
            {
                var spawnPos = GetAbsolutePosition(spawnPoint.Position);
                var spawnNode = map.AddNode(spawnPos, false);
                spawnNode.Rotation = Quaternion.Normalize(
                    spawnPoint.Rotation * prefabRot);
                spawnNode.ForwardItem = item;
                list.Add(spawnNode);
            }
            for (int i = 0; i < selected.Count; i++)
            {
                clonedPoints.Remove(selected[i]);
            }
            return list;
        }

        /// <summary>
        /// Converts a point which is relative to the prefab's origin to an absolute map point.
        /// </summary>
        /// <param name="ppdPointPos">The ppd point to convert.</param>
        /// <param name="node0Pos">The ppd position of the prefab's red control node.</param>
        /// <returns>The position of the point in the map.</returns>
        private Vector3 GetAbsolutePosition(Vector3 ppdPointPos)
        {
            var rotated = RotateNode(ppdPointPos, prefabRot);
            var abs = prefabPos + (rotated - Node0Pos);
            return abs;
        }

        /// <summary>
        /// Creates map nodes for the control nodes of the prefab.
        /// </summary>
        private void CreateMapNodes()
        {
            for (int i = 0; i < ppd.Nodes.Count; i++)
            {
                var ppdNode = ppd.Nodes[i];
                var ppdNodePos = ppdNode.Position;

                // set map node position
                var nodePos = GetAbsolutePosition(ppdNodePos);

                var mapNode = map.AddNode(nodePos, i == 0);
                mapNode.Rotation = GetNodeRotation(ppdNode.Direction);

                mapNode.ForwardItem = prefab;

                prefab.Nodes.Add(mapNode);
            }
        }

        private Quaternion GetNodeRotation(Vector3 nodeDirection)
        {
            // TODO: Fix angle
            var angle = MathEx.AngleOffAroundAxis(nodeDirection, -Vector3.UnitZ, Vector3.UnitY, false);
            var rot = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)angle);
            rot *= prefabRot;
            return rot;
        }

        /// <summary>
        /// Rotates a point around the position of the red control node.
        /// </summary>
        /// <param name="ppdPointPos">The ppd point to rotate.</param>
        /// <param name="node0Pos">The ppd position of the prefab's red control node.</param>
        /// <returns>The rotated point.</returns>
        private Vector3 RotateNode(Vector3 ppdPointPos, Quaternion rot)
        {
            ppdPointPos = MathEx.RotatePointAroundPivot(ppdPointPos, Node0Pos, rot);
            return ppdPointPos;
        }

        private bool Has(SpawnPointType type) 
            => clonedPoints.Any(x => x.Type == type);

    }
}
