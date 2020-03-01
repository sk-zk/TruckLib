using TruckLib.Model.Ppd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    internal class PrefabCreator
    {
        private PpdFile ppd;
        private IItemContainer map;
        private Vector3 prefabPos;
        private Prefab prefab;
        private List<SpawnPoint> clonedPoints;

        public Prefab FromPpd(IItemContainer map, string unitName, string variant, string look,
            PpdFile ppd, Vector3 prefabPos)
        {
            this.map = map;
            this.ppd = ppd;
            this.prefabPos = prefabPos;

            prefab = new Prefab();

            prefab.Model = unitName;
            prefab.Variant = variant;
            prefab.Look = look;

            // create map nodes from ppd
            CreateMapNodes();

            if (ppd.SpawnPoints.Count > 0)
            {
                CreateSlaveItems();
            }

            map.AddItem(prefab, prefab.Nodes[0]);
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
            if (Has(SpawnPointType.WeightStation))
            {
                CreateServiceItemsOfType(SpawnPointType.WeightStation,
                    ServiceType.WeighStation);
            }
            if (Has(SpawnPointType.WeightStationCat))
            {
                CreateServiceItemsOfType(SpawnPointType.WeightStationCat, 
                    ServiceType.WeighStationCat);
            }

        }

        /// <summary>
        /// Creates a Company item for company prefabs.
        /// </summary>
        private void CreateCompany()
        {
            var node0Pos = ppd.Nodes[0].Position;

            // create company item
            var company = CreateSlaveItem<Company>(SpawnPointType.CompanyPoint);

            // set unloading points
            company.UnloadPointsEasy = CreateSpawnPointNodes(company, SpawnPointType.UnloadEasy, node0Pos);
            company.UnloadPointsMedium = CreateSpawnPointNodes(company, SpawnPointType.UnloadMedium, node0Pos);
            company.UnloadPointsHard = CreateSpawnPointNodes(company, SpawnPointType.UnloadHard, node0Pos);

            // set trailer spawn points
            company.TrailerSpawnPoints = CreateSpawnPointNodes(company, SpawnPointType.Trailer, node0Pos);
        }

        private void CreateGarage()
        {
            var node0Pos = ppd.Nodes[0].Position;

            var garage = CreateSlaveItem<Garage>(SpawnPointType.GaragePoint);
            garage.BuyMode = 0;
            garage.TrailerSpawnPoints = CreateSpawnPointNodes(garage, SpawnPointType.TrailerSpawn, node0Pos);

            var buy = CreateSlaveItem<Garage>(SpawnPointType.BuyPoint);
            buy.BuyMode = 1;

            CreateSlaveItem<FuelPump>(SpawnPointType.GasStation);
        }

        private void CreateTruckDealer()
        {
            var node0Pos = ppd.Nodes[0].Position;
            var dealer = CreateSlaveItem<Service>(SpawnPointType.TruckDealer);
            dealer.ServiceType = ServiceType.TruckDealer;
            dealer.Nodes = CreateSpawnPointNodes(dealer, SpawnPointType.TrailerSpawn, node0Pos);

            CreateServiceStation();
        }

        private void CreateServiceStation()
        {
            var service = CreateSlaveItem<Service>(SpawnPointType.ServiceStation);
            service.ServiceType = ServiceType.ServiceStation;
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
        /// Creates slave item for the given spawnpoint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spawnPoint"></param>
        /// <returns></returns>
        private T CreateSlaveItem<T>(SpawnPoint spawnPoint)
            where T : PrefabSlaveItem, new()
        {
            var node0Pos = ppd.Nodes[0].Position;

            var pos = GetAbsolutePosition(spawnPoint.Position, node0Pos);

            var item = PrefabSlaveItem.Add<T>(map, prefab, pos);
            item.Node.Rotation = spawnPoint.Rotation;
            item.Node.ForwardItem = item;

            return item;
        }

        /// <summary>
        /// Creates slave item of the first spawnpoint of the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        private T CreateSlaveItem<T>(SpawnPointType type)
            where T : PrefabSlaveItem, new()
        {
            var first = clonedPoints.First(x => x.Type == type);
            var item = CreateSlaveItem<T>(first);
            clonedPoints.Remove(first);
            return item;
        }

        /// <summary>
        /// Creates map nodes for all spawn points of the given type.
        /// </summary>
        /// <param name="item">The company item.</param>
        /// <param name="spawnPointType">The spawn point type.</param>
        /// <param name="node0Pos">The ppd position of the prefab's red control node.</param>
        /// <returns>A list of map nodes.</returns>
        private List<Node> CreateSpawnPointNodes(PrefabSlaveItem item, SpawnPointType spawnPointType, Vector3 node0Pos)
        {
            var list = new List<Node>();
            var selected = clonedPoints.Where(x => x.Type == spawnPointType).ToList();
            foreach (var spawnPoint in selected)
            {
                var spawnPos = GetAbsolutePosition(spawnPoint.Position, node0Pos);
                var spawnNode = map.AddNode(spawnPos, false);
                spawnNode.Rotation = spawnPoint.Rotation;
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
        private Vector3 GetAbsolutePosition(Vector3 ppdPointPos, Vector3 node0Pos)
        {
            return prefabPos + (ppdPointPos - node0Pos);
        }

        /// <summary>
        /// Creates map nodes for the control nodes of the prefab.
        /// </summary>
        private void CreateMapNodes()
        {
            var node0Pos = ppd.Nodes[0].Position;
            for (int i = 0; i < ppd.Nodes.Count; i++)
            {
                var ppdNode = ppd.Nodes[i];

                var ppdNodePos = ppdNode.Position;

                // set map node position
                Vector3 nodePos;
                if (i == 0)
                {
                    nodePos = prefabPos;
                }
                else
                {
                    nodePos = GetAbsolutePosition(ppdNodePos, node0Pos);
                }
                var mapNode = map.AddNode(nodePos, i == 0);

                // set map node rotation
                var angle = MathEx.AngleOffAroundAxis(ppdNode.Direction, -Vector3.UnitZ, Vector3.UnitY);
                mapNode.Rotation = Quaternion.CreateFromYawPitchRoll((float)angle, 0, 0);

                mapNode.ForwardItem = prefab;

                prefab.Nodes.Add(mapNode);
            }
        }

        /// <summary>
        /// Rotates a point around the position of the red control node.
        /// </summary>
        /// <param name="ppdPointPos">The ppd point to rotate.</param>
        /// <param name="node0Pos">The ppd position of the prefab's red control node.</param>
        /// <returns>The rotated point.</returns>
        private Vector3 RotateNode(Vector3 ppdPointPos, Vector3 node0Pos, float angle)
        {
            var rot = Quaternion.CreateFromYawPitchRoll(angle, 0, 0);
            ppdPointPos = MathEx.RotatePointAroundPivot(ppdPointPos, node0Pos, rot);
            return ppdPointPos;
        }

        private bool Has(SpawnPointType type) 
            => clonedPoints.Any(x => x.Type == type);

    }
}
