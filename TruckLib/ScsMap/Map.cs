using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A SCS map.
    /// </summary>
    public class Map : IItemContainer
    {
        /// <summary>
        /// The name of the map, which is used for file and directory names.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The map's sectors.
        /// </summary>
        public Dictionary<ValueTuple<int, int>, Sector> Sectors { get; set; } 
            = new Dictionary<ValueTuple<int, int>, Sector>();

        /// <summary>
        /// Contains all nodes in this sector.
        /// </summary>
        // I've pulled the node dictionary into Map instead of having one dict
        // per Sector. This is because certain nodes of items that cross 
        // sector boundaries are written to both sectors, and doing this was
        // the best way to prevent two instances of the same node.
        public Dictionary<ulong, Node> Nodes { get; set; } 
            = new Dictionary<ulong, Node>();

        /// <summary>
        /// Scale of the game outside cities.
        /// </summary>
        public float NormalScale { get; set; } = 19;

        /// <summary>
        /// Scale of the game inside cities.
        /// </summary>
        public float CityScale { get; set; } = 3;

        /// <summary>
        /// Editor start position. TODO: Figure out these values
        /// </summary>
        private Vector3 StartPlacementPosition = new Vector3(0, 0, 0);

        /// <summary>
        /// Editor start position. TODO: Figure out these values
        /// </summary>
        private uint StartPlacementSectorOrSomething = 0x800800;

        /// <summary>
        /// Editor start rotation.
        /// </summary>
        private Quaternion StartPlacementRotation = new Quaternion(0, 0, 0, 1);

        /// <summary>
        /// <para>SCS's Europe map UI corrections.</para>
        /// <para>Not sure what it does, but it might have something to do
        /// with the scale of the UK in the official map.</para>
        /// </summary>
        public bool EuropeMapUiCorrections { get; set; } = false;

        // This value is used in both ETS2 and ATS.
        protected uint gameTag = 2998976734; //TODO: What is this?

        /// <summary>
        /// The map's header.
        /// </summary>
        private Header header = new Header();

        /// <summary>
        /// The size of a sector in in-game units (= meters).
        /// </summary>
        public static readonly int SectorSize = 4000;

        /// <summary>
        /// Creates an empty map.
        /// </summary>
        /// <param name="name">The name of the map.</param>
        public Map(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Opens a map.
        /// </summary>
        /// <param name="mbdPath">The mbd file of the map.</param>
        public static Map Open(string mbdPath)
        {
            Trace.WriteLine("Loading map " + mbdPath);
            var name = Path.GetFileNameWithoutExtension(mbdPath);
            var mapDirectory = Directory.GetParent(mbdPath).FullName;
            var sectorDirectory = Path.Combine(mapDirectory, name);

            var map = new Map(name);
            map.ReadMbd(mbdPath);
            Trace.WriteLine("Parsing sectors");
            map.ReadSectors(sectorDirectory);

            Trace.WriteLine("Updating references");
            map.UpdateReferences();

            return map;
        }

        /// <summary>
        /// Creates a new sector.
        /// </summary>
        /// <param name="x">The X coordinate of the sector.</param>
        /// <param name="z">The Z coordinate of the sector.</param>
        /// <returns>The new sector.</returns>
        public Sector AddSector(int x, int z)
        {
            if (Sectors.ContainsKey((x, z)))
            {
                throw new ArgumentOutOfRangeException($"Sector {x}, {z} already exists.");
            }
            var sector = new Sector(x, z, this);
            Sectors.Add((x, z), sector);
            return sector;
        }

        /// <summary>
        /// Creates a new sector.
        /// </summary>
        /// <param name="coordinates">The coordinates of the sector.</param>
        /// <returns>The new sector.</returns>
        public Sector AddSector((int X, int Z) coordinates)
        {
            return AddSector(coordinates.X, coordinates.Z);
        }

        /// <summary>
        /// Creates a new node and adds it to its corresponding sector. 
        /// If that sector does not yet exist, it will be created automatically.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <returns>The new node.</returns>
        public Node AddNode(Vector3 position)
        {
            return AddNode(position, false);
        }

        /// <summary>
        /// Creates a new node and adds it to its corresponding sector. 
        /// If that sector does not yet exist, it will be created automatically.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="isRed">Whether or not the node is red.</param>
        /// <returns>The new node.</returns>
        public Node AddNode(Vector3 position, bool isRed)
        {
            var sectorIdx = GetSectorOfCoordinate(position);
            if(!Sectors.ContainsKey(sectorIdx))
            {
                AddSector(sectorIdx);
            }

            var node = new Node();
            node.Sectors.Add(Sectors[sectorIdx]);
            node.Position = position;
            node.IsRed = isRed;
            Nodes.Add(node.Uid, node);
            return node;
        }

        /// <summary>
        /// Adds an item to the map. This is the final step in the Add() method of an item
        /// and should not be called on its own.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="mainNode">The main node of the item. This will determine which sector
        /// contains the item.</param>
        void IItemContainer.AddItem(MapItem item, Node mainNode)
        {
            mainNode.Sectors[0].MapItems.Add(item.Uid, item);
        }

        /// <summary>
        /// Fills in the Node and Forward/BackwardItem fields in map item and node objects
        /// by searching the Node or Item list for the Uid references.
        /// </summary>
        private void UpdateReferences()
        {
            var allNodes = GetAllNodes();
            var allItems = GetAllItems();

            // first of all, find map items referenced in nodes
            Trace.WriteLine("Updating item references in nodes");
            foreach(var kvp in allNodes)
            {
                kvp.Value.UpdateItemReferences(allItems);
            }

            // then find nodes referenced in map items
            // and map items referenced in map items
            Trace.WriteLine("Updating node & item references in items");
            foreach (var item in allItems)
            {
                item.Value.UpdateNodeReferences(allNodes);
                if(item.Value is IItemReferences hasItemRef)
                {
                    hasItemRef.UpdateItemReferences(allItems);
                }
            }
        }

        /// <summary>
        /// Returns the index of the sector a coordinate is in.
        /// </summary>
        /// <param name="c">The coordinate to check.</param>
        /// <returns>The index of the sector the coordinate is in.</returns>
        public static (int, int) GetSectorOfCoordinate(Vector3 c)
        {
            return (
                (int)Math.Floor(c.X / SectorSize), 
                (int)Math.Floor(c.Z / SectorSize)
                );
        }

        /// <summary>
        /// Returns a list containing all nodes in the entire map.
        /// </summary>
        /// <returns>All nodes in the entire map.</returns>
        public Dictionary<ulong, Node> GetAllNodes()
        {       
            return Nodes;
        }

        /// <summary>
        /// Returns a list containing all map items from all sectors.
        /// </summary>
        /// <returns>All map items in the entire map.</returns>
        public Dictionary<ulong, MapItem> GetAllItems()
        {
            var allItems = new Dictionary<ulong, MapItem>();
            foreach (var sectorKvp in Sectors)
            {
                foreach(var itemKvp in sectorKvp.Value.MapItems)
                {
                    allItems.Add(itemKvp.Value.Uid, itemKvp.Value);
                }
            }
            return allItems;
        }

        /// <summary>
        /// Returns a list containing all items of type T from all sectors.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <returns>All items of this type in the entire map.</returns>
        public List<T> GetAllItems<T>() where T : MapItem
        {
            var allItems = new List<T>();
            foreach (var sectorKvp in Sectors)
            {
                var items = sectorKvp.Value.MapItems.Where(x => x.Value is T);
                foreach(var item in items)
                {
                    allItems.Add((T)item.Value);
                }
            }
            return allItems;
        }

        /// <summary>
        /// Reads the .mbd of a map.
        /// </summary>
        /// <param name="mbdPath">The path to the .mbd file.</param>
        private void ReadMbd(string mbdPath)
        {
            using (var r = new BinaryReader(new FileStream(mbdPath, FileMode.Open)))
            {
                var header = new Header();
                header.ReadFromStream(r);

                // start position
                StartPlacementPosition = r.ReadVector3();
                StartPlacementSectorOrSomething = r.ReadUInt32();

                // start rotation
                StartPlacementRotation.W = r.ReadSingle();
                StartPlacementRotation.X = r.ReadSingle();
                StartPlacementRotation.Y = r.ReadSingle();
                StartPlacementRotation.Z = r.ReadSingle();

                // game tag - TODO: What is this?
                gameTag = r.ReadUInt32();

                // scale
                NormalScale = r.ReadSingle();
                CityScale = r.ReadSingle();

                // SCS's Europe map UI corrections
                EuropeMapUiCorrections = (r.ReadByte() == 1);
            }
        }

        /// <summary>
        /// Reads the sectors of this map.
        /// </summary>
        /// <param name="mapDirectory">The main map directory.</param>
        private void ReadSectors(string mapDirectory)
        {
            var baseFiles = Directory.GetFiles(mapDirectory, "*.base");

            // create itemless instances for all the sectors first;
            // this is so we have references to the sectors
            // before we read them to deal with sectors containing
            // nodes from other sectors
            foreach(var baseFile in baseFiles)
            {
                var sector = new Sector(baseFile, this)
                {
                    Map = this
                };
                Sectors.Add((sector.X, sector.Z), sector);
            }

            var sectorList = Sectors.ToList();
            foreach (var sectorKvp in sectorList)
            {
                Trace.WriteLine($"Reading sector {sectorKvp.Value.ToString()}");
                sectorKvp.Value.Read();
            }
        }

        /// <summary>
        /// Saves the map in binary format.
        /// </summary>
        /// <param name="mapDirectory">The directory the map will be saved in.</param>
        /// <param name="cleanDir">If set to true, the sectors folder will be emptied before saving the map.</param>
        public void Save(string mapDirectory, bool cleanDir = true)
        {
            var sectorDirectory = Path.Combine(mapDirectory, Name);
            Directory.CreateDirectory(sectorDirectory);
            if(cleanDir)
            {
                foreach (var file in new DirectoryInfo(sectorDirectory).GetFiles())
                {
                    file.Delete();
                }
            }

            foreach (var kvp in Sectors)
            {
                kvp.Value.Save(sectorDirectory);
            }

            var mbdPath = Path.Combine(mapDirectory, $"{Name}.mbd");
            SaveMbd(mbdPath);
        }

        /// <summary>
        /// Writes the mbd of this map.
        /// </summary>
        /// <param name="mbdPath">The path of the .mbd file.</param>
        private void SaveMbd(string mbdPath)
        {
            var stream = new FileStream(mbdPath, FileMode.Create);
            using (var w = new BinaryWriter(stream))
            {
                header.WriteToStream(w);

                // start pos.
                w.Write(StartPlacementPosition);
                w.Write(StartPlacementSectorOrSomething);

                // start rotation
                w.Write(StartPlacementRotation.W);
                w.Write(StartPlacementRotation.X);
                w.Write(StartPlacementRotation.Y);
                w.Write(StartPlacementRotation.Z);

                // game tag
                w.Write(gameTag);

                // scale
                w.Write(NormalScale);
                w.Write(CityScale);

                // SCS's Europe map UI corrections
                w.Write(EuropeMapUiCorrections.ToByte());
            }
        }
    }
}
