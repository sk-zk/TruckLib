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
    /// Creates a new prefab item and associated prefab slave items from a prefab descriptor.
    /// </summary>
    internal class PrefabCreator
    {
        /// <summary>
        /// The prefab descriptor for which map items are being created.
        /// </summary>
        private PrefabDescriptor ppd;

        /// <summary>
        /// The map which the items are being added to.
        /// </summary>
        private IItemContainer map;

        /// <summary>
        /// The position of the prefab's 0th node in map space.
        /// </summary>
        private Vector3 prefabPos;

        /// <summary>
        /// The rotation of the prefab's 0th node in map space.
        /// </summary>
        private Quaternion prefabRot;

        /// <summary>
        /// A rotation applied by the game before the user-defined node rotation is applied. 
        /// </summary>
        private Quaternion inherentRot;

        /// <summary>
        /// The prefab item which is being built.
        /// </summary>
        private Prefab prefab;

        /// <summary>
        /// Spawn points from the prefab descriptor which have not yet been turned into
        /// their corresponding map objects.
        /// </summary>
        private List<SpawnPoint> pendingPoints;

        public Prefab FromPpd(IItemContainer map, Token unitName, PrefabDescriptor ppd,
            Vector3 position, Quaternion rotation)
        {
            this.map = map;
            this.ppd = ppd;
            this.prefabPos = position;

            // The game reorients prefab models such that a rotation of 0° means the 0th
            // control node has a direction of (0, 0, -1). This means that we need to
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
            pendingPoints = new List<SpawnPoint>(ppd.SpawnPoints);

            var has = Enum.GetValues<SpawnPointType>().ToDictionary(k => k, v => false);
            foreach (var point in pendingPoints)
            {
                has[point.Type] = true;
            }

            if (has[SpawnPointType.CompanyPoint])
            {
                CreateCompany();
            }
            if (has[SpawnPointType.GaragePoint])
            {
                CreateGarage();
            }
            if (has[SpawnPointType.TruckDealer])
            {
                CreateTruckDealer();
            }

            //////

            if (has[SpawnPointType.BusStation])
            {
                CreateSlaveItem<BusStop>(SpawnPointType.BusStation);
            }
            if (has[SpawnPointType.GasStation])
            {
                CreateServiceItemsOfType(SpawnPointType.GasStation,
                    ServiceType.GasStation);
            }
            if (has[SpawnPointType.Parking])
            {
                CreateServiceItemsOfType(SpawnPointType.Parking,
                    ServiceType.Parking);
            }
            if (has[SpawnPointType.Recruitment])
            {
                CreateServiceItemsOfType(SpawnPointType.Recruitment,
                    ServiceType.Recruitment);
            }
            if (has[SpawnPointType.ServiceStation])
            {
                CreateServiceItemsOfType(SpawnPointType.ServiceStation,
                    ServiceType.ServiceStation);
            }
            if (has[SpawnPointType.WeighStation])
            {
                CreateServiceItemsOfType(SpawnPointType.WeighStation,
                    ServiceType.WeighStation);
            }
            if (has[SpawnPointType.WeighStationCat])
            {
                CreateServiceItemsOfType(SpawnPointType.WeighStationCat, 
                    ServiceType.WeighStationCat);
            }

            if (pendingPoints.Count > 0)
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

            for (int i = 0; i < pendingPoints.Count; i++)
            {
                var point = pendingPoints[i];

                if (point.Type is SpawnPointType.UnloadEasy 
                    or SpawnPointType.UnloadMedium 
                    or SpawnPointType.UnloadHard
                    or SpawnPointType.UnloadRigid
                    or SpawnPointType.Trailer
                    or SpawnPointType.LongTrailer
                    or SpawnPointType.Custom)
                {
                    CreatePoint(company, point);
                    pendingPoints.RemoveAt(i);
                    i--;
                }
            }

            void CreatePoint(Company company, SpawnPoint point)
            {
                var node = CreateSpawnPointNode(company, point);
                CompanySpawnPoint spawnPointStruct; 
                if (point.Type == SpawnPointType.Custom)
                {
                    spawnPointStruct = new CompanySpawnPoint(node, point.Flags.Bits);
                } 
                else
                {
                    var depot = point.Type switch
                    {
                        SpawnPointType.Trailer or SpawnPointType.LongTrailer => CompanyDepotType.Load,
                        _ => CompanyDepotType.Unload,
                    };
                    var difficulty = point.Type switch 
                    { 
                        SpawnPointType.UnloadEasy => CompanyUnloadDifficulty.Easy,
                        SpawnPointType.UnloadMedium => CompanyUnloadDifficulty.Medium,
                        SpawnPointType.UnloadHard => CompanyUnloadDifficulty.Hard,
                        SpawnPointType.Trailer => CompanyUnloadDifficulty.Easy,
                        SpawnPointType.LongTrailer => CompanyUnloadDifficulty.Easy,
                        _ => CompanyUnloadDifficulty.Easy,
                    };            
                    spawnPointStruct = new CompanySpawnPoint(node, depot, difficulty);
                }
                company.SpawnPoints.Add(spawnPointStruct);
            }
        }

        /// <summary>
        /// Creates a Garage item for player garage prefabs.
        /// </summary>
        private void CreateGarage()
        {
            var garage = CreateSlaveItem<Garage>(SpawnPointType.GaragePoint);
            garage.IsBuyPoint = false;
            garage.TrailerSpawnPoints = CreateSpawnPointNodes(garage, SpawnPointType.TrailerSpawn);

            var buy = CreateSlaveItem<Garage>(SpawnPointType.BuyPoint);
            buy.IsBuyPoint = true;

            CreateSlaveItem<FuelPump>(SpawnPointType.GasStation);
        }

        /// <summary>
        /// Creates a truck dealer Service item for truck dealer prefabs.
        /// </summary>
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
            foreach (var point in pendingPoints.Where(x => x.Type == type))
            {
                var item = CreateSlaveItem<Service>(point);
                item.ServiceType = serviceType;
            }
            pendingPoints.RemoveAll(x => x.Type == type);
        }

        /// <summary>
        /// Creates a slave item of the first spawn point of the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        private T CreateSlaveItem<T>(SpawnPointType type) where T : PrefabSlaveItem, new()
        {
            var first = pendingPoints.First(x => x.Type == type);
            var item = CreateSlaveItem<T>(first);
            pendingPoints.Remove(first);
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
        /// <param name="item">The slave item.</param>
        /// <param name="spawnPointType">The spawn point type.</param>
        /// <returns>A list of map nodes representing the spawn points.</returns>
        private List<INode> CreateSpawnPointNodes(PrefabSlaveItem item, SpawnPointType spawnPointType)
        {
            var selected = pendingPoints.Where(x => x.Type == spawnPointType).ToList();
            var list = new List<INode>(selected.Count);
            for (int i = 0; i < selected.Count; i++)
            {
                list.Add(CreateSpawnPointNode(item, selected[i]));
                pendingPoints.Remove(selected[i]);
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
        /// Converts a point from model space to map space.
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
        /// Rotates a ppd point around the position of node 0.
        /// The returned point is still in model space.
        /// </summary>
        /// <param name="point">The point to rotate.</param>
        /// <param name="rot">The rotation.</param>
        /// <returns>The rotated point.</returns>
        private Vector3 RotatePointAroundNode0(Vector3 point, Quaternion rot) =>
            MathEx.RotatePointAroundPivot(point, ppd.Nodes[0].Position, rot);
    }
}
