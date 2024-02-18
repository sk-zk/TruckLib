using TruckLib.Models.Ppd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Code for adding a prefab to the map.
    /// </summary>
    internal class PrefabCreator
    {
        private PrefabDescriptor ppd;
        private IItemContainer map;
        private Vector3 prefabPos;
        private Quaternion prefabRot;
        private Quaternion inherentRot;
        private Prefab prefab;
        private List<SpawnPoint> clonedPoints;

        public Prefab FromPpd(IItemContainer map, Token unitName, PrefabDescriptor ppd,
            Vector3 position, Quaternion rotation)
        {
            this.map = map;
            this.ppd = ppd;
            this.prefabPos = position;

            // the game reorients prefab models such that a rotation of 0° means the 0th
            // control node has a direction of (0, 0, -1). this means that we need to
            // rotate the model in the same way to place nodes at the correct positions
            // and set map node rotations to the correct values.
            this.inherentRot = GetNodeRotation(ppd.Nodes[0].Direction);
            this.prefabRot = Quaternion.Inverse(inherentRot) * rotation;

            prefab = new Prefab { Model = unitName };

            CreateMapNodes();

            if (ppd.SpawnPoints.Count > 0)
                CreateSlaveItems();

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

            if (clonedPoints.Count > 0)
            {
                Trace.WriteLine($"Unhandled spawn points in {prefab.Model.String}");
            }
        }

        /// <summary>
        /// Creates a Company item for company prefabs.
        /// </summary>
        private void CreateCompany()
        {
            var company = CreateSlaveItem<Company>(SpawnPointType.CompanyPoint);

            for (int i = 0; i < clonedPoints.Count; i++)
            {
                switch (clonedPoints[i].Type)
                {
                    case SpawnPointType.UnloadEasy:
                        i = CreateCompanySpawnPoint(company, i, CompanySpawnPointType.UnloadEasy);
                        break;
                    case SpawnPointType.UnloadMedium:
                        i = CreateCompanySpawnPoint(company, i, CompanySpawnPointType.UnloadMedium);
                        break;
                    case SpawnPointType.UnloadHard:
                        i = CreateCompanySpawnPoint(company, i, CompanySpawnPointType.UnloadHard);
                        break;
                    case SpawnPointType.Trailer:
                        i = CreateCompanySpawnPoint(company, i, CompanySpawnPointType.Trailer);
                        break;
                    case SpawnPointType.LongTrailer:
                        i = CreateCompanySpawnPoint(company, i, CompanySpawnPointType.Trailer);
                        break;
                }
            }

            int CreateCompanySpawnPoint(Company company, int i, CompanySpawnPointType type)
            {
                var node = CreateSpawnPointNode(company, clonedPoints[i]);
                var spawnPointStruct = new CompanySpawnPoint(node, type);
                company.SpawnPoints.Add(spawnPointStruct);
                clonedPoints.RemoveAt(i);
                i--;
                return i;
            }
        }

        private void CreateGarage()
        {
            var garage = CreateSlaveItem<Garage>(SpawnPointType.GaragePoint);
            garage.IsBuyPoint = false;
            garage.TrailerSpawnPoints = CreateSpawnPointNodes(garage, SpawnPointType.TrailerSpawn);

            var buy = CreateSlaveItem<Garage>(SpawnPointType.BuyPoint);
            buy.IsBuyPoint = true;

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
        /// Creates a slave item of the first spawn point of the given type.
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
        /// Creates a slave item for the given spawn point.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spawnPoint"></param>
        /// <returns></returns>
        private T CreateSlaveItem<T>(SpawnPoint spawnPoint) where T : PrefabSlaveItem, new()
        {
            var pos = GetAbsolutePosition(spawnPoint.Position);

            var item = PrefabSlaveItem.Add<T>(map, prefab, pos);
            item.Node.Rotation = prefabRot;
            item.Node.ForwardItem = item;

            return item;
        }

        /// <summary>
        /// Creates map nodes for all spawn points of the given type.
        /// </summary>
        /// <param name="item">The company item.</param>
        /// <param name="spawnPointType">The spawn point type.</param>
        /// <returns>A list of map nodes.</returns>
        private List<INode> CreateSpawnPointNodes(PrefabSlaveItem item, SpawnPointType spawnPointType)
        {
            var selected = clonedPoints.Where(x => x.Type == spawnPointType).ToList();
            var list = new List<INode>(selected.Count);
            for (int i = 0; i < selected.Count; i++)
            {
                list.Add(CreateSpawnPointNode(item, selected[i]));
                clonedPoints.Remove(selected[i]);
            }
            return list;
        }

        private Node CreateSpawnPointNode(PrefabSlaveItem item, SpawnPoint spawnPoint)
        {
            var spawnPos = GetAbsolutePosition(spawnPoint.Position);
            var spawnNode = map.AddNode(spawnPos, false);
            spawnNode.Rotation = new Quaternion(0, 1 ,0, 0) * spawnPoint.Rotation * prefabRot;
            spawnNode.ForwardItem = item;
            return spawnNode;
        }

        /// <summary>
        /// Converts a point which is relative to the prefab's origin to an absolute map point.
        /// </summary>
        /// <param name="ppdPointPos">The ppd point to convert.</param>
        /// <returns>The position of the point in the map.</returns>
        private Vector3 GetAbsolutePosition(Vector3 ppdPointPos)
        {
            var rotated = RotatePointAroundNode0(ppdPointPos, prefabRot);
            var absolute = prefabPos - (rotated - ppd.Nodes[0].Position);
            return absolute;
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

                var nodePos = GetAbsolutePosition(ppdNodePos);

                var mapNode = map.AddNode(nodePos, i == 0);
                mapNode.Rotation = GetNodeRotation(ppdNode.Direction) * prefabRot;
                mapNode.ForwardItem = prefab;

                prefab.Nodes.Add(mapNode);
            }
        }

        /// <summary>
        /// Converts the direction of a control node to the rotation of a map node.
        /// </summary>
        /// <param name="direction">The direction of the control node.</param>
        /// <returns>The equivalent rotation of the map node.</returns>
        private Quaternion GetNodeRotation(Vector3 direction)
        {
            var angle = MathEx.AngleOffAroundAxis(direction, Vector3.UnitZ, Vector3.UnitY, false);
            angle = MathEx.Mod(angle, Math.Tau);
            var rot = Quaternion.CreateFromYawPitchRoll((float)angle, 0, 0);
            return rot;
        }

        /// <summary>
        /// Rotates a ppd point around the position of node 0. The returned point is
        /// still in the coordinate system of the prefab.
        /// </summary>
        /// <param name="point">The point to rotate.</param>
        /// <param name="rot">The rotation.</param>
        /// <returns>The rotated point.</returns>
        private Vector3 RotatePointAroundNode0(Vector3 point, Quaternion rot) =>
            MathEx.RotatePointAroundPivot(point, ppd.Nodes[0].Position, rot);

        private bool Has(SpawnPointType type) 
            => clonedPoints.Any(x => x.Type == type);
    }
}
