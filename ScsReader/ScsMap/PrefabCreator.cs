using ScsReader.Model.Ppd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    internal class PrefabCreator
    {
        private PpdFile ppd;
        private IItemContainer map;
        private Vector3 prefabPos;
        private Prefab prefab;

        public Prefab FromPpd(IItemContainer map, string unitName, string variant, string look, PpdFile ppd, Vector3 prefabPos)
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

            // create slave items
            if (IsCompany())
            {
                CreateCompany();
            }

            map.AddItem(prefab, prefab.Nodes[0]);
            // TODO: Which node determines the sector? 
            // Nodes[0] or Nodes[origin]?
            return prefab;
        }

        /// <summary>
        /// Creates a Company item for company prefabs.
        /// </summary>
        private void CreateCompany()
        {
            var node0Pos = ppd.Nodes[0].Position;

            // create company item
            var companyPoint = ppd.SpawnPoints.First(x => x.Type == SpawnPointType.CompanyPoint);
            Vector3 companyMapPos = GetAbsolutePosition(companyPoint.Position, node0Pos);

            var company = Company.Add(map, prefab, companyMapPos);
            company.Node.Rotation = companyPoint.Rotation;
            company.Node.ForwardItem = company;

            // set unloading points
            company.UnloadPointsEasy = CreateSpawnPointNodes(company, SpawnPointType.UnloadEasy, node0Pos);
            company.UnloadPointsMedium = CreateSpawnPointNodes(company, SpawnPointType.UnloadMedium, node0Pos);
            company.UnloadPointsHard = CreateSpawnPointNodes(company, SpawnPointType.UnloadHard, node0Pos);

            // set trailer spawn points
            company.TrailerSpawnPoints = CreateSpawnPointNodes(company, SpawnPointType.Trailer, node0Pos);
        }

        /// <summary>
        /// Creates map nodes for all spawn points of the given type.
        /// </summary>
        /// <param name="company"></param>
        /// <param name="spawnPointType"></param>
        /// <param name="node0Pos"></param>
        /// <returns></returns>
        private List<Node> CreateSpawnPointNodes(Company company, SpawnPointType spawnPointType, Vector3 node0Pos)
        {
            var list = new List<Node>();
            foreach (var spawnPoint in ppd.SpawnPoints.Where(x => x.Type == spawnPointType))
            {
                var spawnPos = GetAbsolutePosition(spawnPoint.Position, node0Pos);
                var spawnNode = map.AddNode(spawnPos, false);
                spawnNode.Rotation = spawnPoint.Rotation;
                spawnNode.ForwardItem = company;
                list.Add(spawnNode);
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
                // ??? sometimes points have to be rotated to match the default ingame rotation ???
                // ppdNodePos = RotateNode(ppdNodePos, node0Pos);

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

        private bool IsCompany()
        {
            return ppd.SpawnPoints.Any(x => x.Type == SpawnPointType.CompanyPoint);
        }
    }
}
