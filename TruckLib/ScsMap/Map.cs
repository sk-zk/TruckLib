using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib;
using TruckLib.HashFs;
using TruckLib.ScsMap.Collections;
using TruckLib.ScsMap.Serialization;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A map for Euro Truck Simulator 2 or American Truck Simulator.
    /// </summary>
    public class Map : IItemContainer
    {
        private string name;
        /// <summary>
        /// The name of the map, which is used for file and directory names.
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(Name),
                        "The map name must not be null or just whitespace.");
                }
                if (value.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    throw new ArgumentException("The map name must not contain characters which are " +
                        "not allowed in filenames", nameof(Name));
                }
                name = value;
            }
        }

        /// <summary>
        /// Metadata of the map's sectors.
        /// </summary>
        public Dictionary<SectorCoordinate, Sector> Sectors { get; set; } = [];

        /// <summary>
        /// Contains the map's nodes.
        /// </summary>
        public NodeDictionary Nodes { get; internal set; } = [];

        /// <summary>
        /// Contains the map's items.
        /// </summary>
        public Dictionary<ulong, MapItem> MapItems { get; internal set; } = [];

        IDictionary<ulong, INode> IItemContainer.Nodes => Nodes;

        /// <summary>
        /// Scale and time compression of the game outside cities.
        /// </summary>
        public float NormalScale { get; set; } = 19;

        /// <summary>
        /// Scale and time compression of the game inside cities.
        /// </summary>
        public float CityScale { get; set; } = 3;

        /// <summary>
        /// Editor start position.
        /// </summary>
        public Vector3 StartPosition { get; set; } = new(0, 0, 0);

        /// <summary>
        /// TODO: Figure out this value
        /// </summary>
        private uint StartSectorOrSomething = 0x4B000800;

        /// <summary>
        /// Editor start rotation.
        /// </summary>
        public Quaternion StartRotation { get; set; } = Quaternion.Identity;

        /// <summary>
        /// <para>Whether SCS's Europe map UI corrections are enabled.</para>
        /// <para>Nobody seems to know definitively what this does, but it might have
        /// something to do with the scale of the UK in <c>europe.mbd.</c></para>
        /// </summary>
        public bool EuropeMapUiCorrections { get; set; } = false;

        public ulong EditorMapId { get; set; }

        // This value is used in both ETS2 and ATS.
        private uint gameTag = 2998976734; //TODO: What is this?

        /// <summary>
        /// The map's header.
        /// </summary>
        private readonly Header header = new();

        /// <summary>
        /// The size of a sector in engine units (= meters).
        /// </summary>
        public const int SectorSize = 4000;

        /// <summary>
        /// EOF marker of .data and .layer files.
        /// </summary>
        internal const ulong EofMarker = ulong.MaxValue;

        /// <summary>
        /// Creates an empty map.
        /// </summary>
        /// <param name="name">The name of the map.</param>
        public Map(string name)
        {
            Name = name;
            EditorMapId = Utils.GenerateUuid();
        }

        /// <summary>
        /// <para>Deserializes a map.</para>
        /// <para>If the given path points to an .mbd file, metadata is read from it and
        /// the sectors are loaded from a sibling subdirectory with the same name.</para>
        /// <para>If the path points to a sector directory, the metadata fields that would
        /// otherwise have been read from the .mbd file, such as <see cref="NormalScale"/>,
        /// are left at their default values.
        /// </para>
        /// </summary>
        /// <param name="path">Path to the .mbd file or sector directory of the map.</param>
        /// <param name="sectors">If set, only the specified sectors will be loaded.</param>
        /// <returns>A Map object.</returns>
        public static Map Open(string path, IList<SectorCoordinate> sectors = null)
        {
            return Open(path, new DiskFileSystem(), sectors);
        }

        /// <summary>
        /// <para>Deserializes a map.</para>
        /// <para>If the given path points to an .mbd file, metadata is read from it and
        /// the sectors are loaded from a sibling subdirectory with the same name.</para>
        /// <para>If the path points to a sector directory, the metadata fields that would
        /// otherwise have been read from the .mbd file, such as <see cref="NormalScale"/>,
        /// are left at their default values.
        /// </para>
        /// </summary>
        /// <param name="path">Path to the .mbd file or sector directory of the map.</param>
        /// <param name="fs">The file system to load the map from. This accepts 
        /// a <see cref="IHashFsReader">HashFS reader</see>.</param>
        /// <param name="sectors">If set, only the specified sectors will be loaded.</param>
        /// <returns>A Map object.</returns>
        public static Map Open(string path, IFileSystem fs, IList<SectorCoordinate> sectors = null)
        {
            path = Path.TrimEndingDirectorySeparator(path);

            Trace.WriteLine("Loading map " + path);

            var loadingFromMbd = fs.FileExists(path);
            var name = Path.GetFileNameWithoutExtension(path);
            var sectorDirectory = loadingFromMbd 
                ? $"{fs.GetParent(path)}{fs.DirectorySeparator}{name}" 
                : path;

            var map = new Map(name);
            if (loadingFromMbd)
            {
                Trace.WriteLine("Parsing .mbd");
                map.ReadMbd(path, fs);
            }

            Trace.WriteLine("Parsing sectors");
            map.ReadSectors(sectorDirectory, fs, sectors);

            Trace.WriteLine("Updating references");
            map.UpdateReferences();

            return map;
        }

        /// <summary>
        /// Creates a new sector.
        /// </summary>
        /// <param name="coord">The coordinate of the sector.</param>
        /// <returns>The new sector.</returns>
        public Sector AddSector(SectorCoordinate coord)
        {
            if (Sectors.TryGetValue(coord, out var existing))
                return existing;
            
             var sector = new Sector(coord, this);
             Sectors.Add(coord, sector);
             return sector;
        }

        /// <summary>
        /// Creates a new sector.
        /// </summary>
        /// <param name="x">The X coordinate of the sector.</param>
        /// <param name="z">The Z coordinate of the sector.</param>
        /// <returns>The new sector.</returns>
        public Sector AddSector(int x, int z) =>
            AddSector(new SectorCoordinate(x, z));

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <returns>The new node.</returns>
        public Node AddNode(Vector3 position)
        {
            return AddNode(position, false);
        }

        /// <summary>
        /// Creates a new node. 
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="isRed">Whether the node is red.</param>
        /// <returns>The new node.</returns>
        public Node AddNode(Vector3 position, bool isRed)
        {
            var coord = GetSectorOfCoordinate(position);
            if (!Sectors.ContainsKey(coord))
            {
                AddSector(coord);
            }

            var node = new Node
            {
                Position = position,
                IsRed = isRed,
                Parent = this,
            };
            Nodes.Add(node.Uid, node);
            return node;
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="isRed">Whether the node is red.</param>
        /// <param name="forwardItem">The forward item which the node will be assigned to.</param>
        /// <returns>The new node.</returns>
        public Node AddNode(Vector3 position, bool isRed, MapItem forwardItem)
        {
            var node = AddNode(position, isRed);
            node.ForwardItem = forwardItem;
            return node;
        }

        /// <summary>
        /// Adds an item to the map. This is the final step in the Add() method of an item
        /// and should not be called on its own.
        /// </summary>
        /// <param name="item">The item.</param>
        void IItemContainer.AddItem(MapItem item)
        {
            AddSector(GetSectorOfCoordinate(item.GetCenter()));
            MapItems.Add(item.Uid, item);
        }

        /// <summary>
        /// Returns the index of the sector the given coordinate falls into.
        /// </summary>
        /// <param name="c">The coordinate to check.</param>
        /// <returns>The index of the sector the coordinate is in.</returns>
        public static SectorCoordinate GetSectorOfCoordinate(Vector3 c) => 
            new((int)Math.Floor(c.X / SectorSize), (int)Math.Floor(c.Z / SectorSize));

        /// <summary>
        /// Deletes an item. Nodes that are only used by this item will also be deleted.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void Delete(MapItem item)
        {
            MapItems.Remove(item.Uid);

            List<IRecalculatable> recalculatables = null;
            List<INode> prefabNodes = null;
            if (item is PolylineItem poly)
            {
                recalculatables = [];
                prefabNodes = [];
                if (poly.BackwardItem is IRecalculatable bwRecalculatable)
                {
                    recalculatables.Add(bwRecalculatable);
                }
                // if the backward item is a prefab, the rotation of that node
                // needs to be reset
                else if (poly.BackwardItem is Prefab)
                {
                    prefabNodes.Add(poly.Node);
                }

                if (poly.ForwardItem is IRecalculatable fwRecalculatable)
                {
                    recalculatables.Add(fwRecalculatable);
                }
            }

            // remove item from its nodes, 
            // and delete them if they're orphaned now
            foreach (var node in item.GetItemNodes())
            {
                if (node.ForwardItem == item)
                {
                    node.ForwardItem = null;
                    node.IsRed = false;
                }
                if (node.BackwardItem == item)
                {
                    node.BackwardItem = null;
                }
                if (node.IsOrphaned())
                {
                    Delete(node);
                }
            }

            for (int i = 0; i < recalculatables?.Count; i++)
                recalculatables[i].Recalculate();

            for (int i = 0; i < prefabNodes?.Count; i++)
                prefabNodes[i].Rotation *= Quaternion.CreateFromYawPitchRoll(MathF.PI, 0, 0);

            // delete dependent items
            if (item is Prefab pf)
            {
                foreach (var slaveItem in pf.SlaveItems)
                {
                    if (slaveItem is MapItem mapItem)
                        Delete(mapItem);
                    // if Unresolved, just ignore it
                }
            }
        }

        /// <summary>
        /// Deletes a node and the items attached to it.
        /// </summary>
        /// <param name="node">The node to delete.</param>
        public void Delete(INode node)
        {
            Nodes.Remove(node.Uid);
            node.Parent = null;
            
            if (node.ForwardItem is MapItem fw)
            {
                node.ForwardItem = null;
                Delete(fw);
            }

            if (node.BackwardItem is MapItem bw)
            {
                node.BackwardItem = null;
                Delete(bw);
            }
        }

        /// <summary>
        /// <para>Creates a <see cref="Compound"/> item and makes it the parent of the given map items.</para>
        /// <para>Note that only <see href="https://github.com/sk-zk/map-docs/wiki/base%2C-aux-and-snd">aux items</see>
        /// are compoundable.</para>
        /// </summary>
        /// <param name="items">The items to compound.</param>
        /// <returns>The newly created compound item.</returns>
        /// <exception cref="InvalidOperationException">Thrown if any of the map items or nodes
        /// are not parented by this map.</exception>
        /// <exception cref="ArgumentException">Thrown if any of the map items is not an aux item.</exception>
        public Compound CompoundItems(IEnumerable<MapItem> items)
        {
            // Make sure everything checks out before moving the first item.
            foreach (var item in items)
            {
                if (item.ItemFile != ItemFile.Aux)
                {
                    throw new ArgumentException($"Item {item.Uid:X} is not an aux item.");
                }

                if (!MapItems.ContainsKey(item.Uid))
                {
                    throw new InvalidOperationException(
                        $"Item {item.Uid:X} is not a child of this map.");
                }

                foreach (var node in item.GetItemNodes())
                {
                    if (!Nodes.ContainsKey(node.Uid))
                    {
                        throw new InvalidOperationException(
                            $"Node {node.Uid:X} of item {item.Uid:X} is not a child of this map.");
                    }
                }
            }

            var compound = Compound.Add(this, Vector3.Zero);
            foreach (var item in items)
            {
                MapItems.Remove(item.Uid);
                compound.MapItems.Add(item.Uid, item);

                foreach (var node in item.GetItemNodes())
                {
                    Nodes.Remove(node.Uid);
                    compound.Nodes.Add(node.Uid, node);
                    node.Parent = compound;
                }
            }
            compound.RecalculateCenter();
            return compound;
        }

        /// <summary>
        /// Deletes the given <see cref="Compound"/> item and adds its child items to the map itself.
        /// </summary>
        /// <param name="compound">The compound item.</param>
        /// <exception cref="InvalidOperationException">Thrown if the compound is not parented
        /// by this map.</exception>
        public void UncompoundItems(Compound compound)
        {
            if (!MapItems.ContainsKey(compound.Uid))
            {
                throw new InvalidOperationException(
                    $"Compound {compound.Uid:X} is not a child of this map.");
            }

            Delete(compound);

            foreach (var (uid, item) in compound.MapItems)
            {
                MapItems.Add(uid, item);
            }
            foreach (var (uid, node) in compound.Nodes)
            {
                Nodes.Add(uid, node);
                node.Parent = this;
            }
        }

        /// <summary>
        /// Imports the contents of a Selection (.sbd) file into this map.
        /// </summary>
        /// <param name="selection">The Selection to import.</param>
        /// <param name="position">The point relative to which the items will be inserted.</param>
        public void Import(Selection selection, Vector3 position)
        {
            // 1) deep cloning everything the lazy way
            var clonedItems = selection.MapItems.Select(x => (x.Key, x.Value.CloneItem()))
                .ToDictionary(k => k.Key, v => v.Item2);
            var clonedNodes = selection.Nodes.Select(x => (x.Key, (x.Value as Node).Clone()))
                .ToDictionary(k => k.Key, v => (INode)v.Item2);

            foreach (var node in clonedNodes.Values)
            {
                node.Position += position - selection.Origin;
                node.UpdateItemReferences(clonedItems);
                var coord = GetSectorOfCoordinate(node.Position);
                AddSector(coord);
            }

            foreach (var item in clonedItems.Values)
            {
                item.UpdateNodeReferences(clonedNodes);
                if (item is IItemReferences itemRefs)
                    itemRefs.UpdateItemReferences(clonedItems);
            }

            // 2) change UIDs (to allow importing the same selection
            // multiple times), then add the nodes and items to the map
            foreach (var node in clonedNodes.Values)
            {
                node.Uid = Utils.GenerateUuid();
                Nodes.Add(node.Uid, node);
            }

            foreach (var item in clonedItems.Values)
            {
                item.Uid = Utils.GenerateUuid();
                (this as IItemContainer).AddItem(item);
            }
        }

        /// <summary>
        /// Reads the .mbd file of a map.
        /// </summary>
        /// <param name="mbdPath">The path to the .mbd file.</param>
        private void ReadMbd(string mbdPath, IFileSystem fs)
        {
            using var fileStream = fs.Open(mbdPath);
            using var r = new BinaryReader(fileStream);

            var header = new Header();
            header.Deserialize(r);

            EditorMapId = r.ReadUInt64();
            StartPosition = r.ReadVector3();
            StartSectorOrSomething = r.ReadUInt32();
            StartRotation = r.ReadQuaternion();
            gameTag = r.ReadUInt32();
            NormalScale = r.ReadSingle();
            CityScale = r.ReadSingle();
            EuropeMapUiCorrections = (r.ReadByte() == 1);
        }

        /// <summary>
        /// Reads the sectors of this map.
        /// </summary>
        /// <param name="mapDirectory">The main map directory.</param>
        /// <param name="sectors">If set, only the given sectors will be loaded.</param>
        private void ReadSectors(string mapDirectory, IFileSystem fs, IList<SectorCoordinate> sectors = null)
        {
            var baseFiles = fs.GetFiles(mapDirectory).Where(f => Path.GetExtension(f) == ".base");

            // create itemless instances for all the sectors first;
            // this is so we have references to the sectors
            // before we read them to deal with sectors containing
            // nodes from other sectors
            foreach (var baseFile in baseFiles)
            {
                var coords = Sector.SectorCoordsFromSectorFilePath(baseFile);
                if (sectors != null && !sectors.Contains(coords))
                    continue;

                var sector = new Sector(coords, this);
                sector.BasePath = baseFile;
                Sectors.Add(coords, sector);
            }

            // now read in the sectors
            foreach (var (_, sector) in Sectors)
            {
                Trace.WriteLine($"Reading sector {sector}");
                sector.ReadDesc(Path.ChangeExtension(sector.BasePath, Sector.DescExtension), fs);
                ReadSector(sector.BasePath, fs);
            }
        }

        /// <summary>
        /// Reads the sector from disk.
        /// </summary>
        /// <param name="basePath">The path to the .base file of the sector. The paths of the other
        /// sector files will be derived automatically.</param>
        /// <param name="fs">The file system to load the sector files from.</param>
        private void ReadSector(string basePath, IFileSystem fs)
        {
            ReadBase(basePath, fs);
            ReadData(Path.ChangeExtension(basePath, Sector.DataExtension), fs);
            ReadAux(Path.ChangeExtension(basePath, Sector.AuxExtenstion), fs);
            ReadSnd(Path.ChangeExtension(basePath, Sector.SndExtension), fs);
            ReadLayer(Path.ChangeExtension(basePath, Sector.LayerExtension), fs);
        }

        /// <summary>
        /// Reads the .base file of the sector.
        /// </summary>
        /// <param name="path">The .base file of the sector.</param>
        /// <param name="fs">The file system to load the file from.</param>
        private void ReadBase(string path, IFileSystem fs)
        {
            using var fileStream = fs.Open(path);
            using var r = new BinaryReader(fileStream);

            var header = new Header();
            header.Deserialize(r);
            ReadItems(r, ItemFile.Base);
            ReadNodes(r);
            ReadVisArea(r);
        }

        /// <summary>
        /// Reads the .aux file of the sector.
        /// </summary>
        /// <param name="path">The .aux file of the sector.</param>
        /// <param name="fs">The file system to load the file from.</param>
        private void ReadAux(string path, IFileSystem fs)
        {
            using var fileStream = fs.Open(path);
            using var r = new BinaryReader(fileStream); 

            var header = new Header();
            header.Deserialize(r);
            ReadItems(r, ItemFile.Aux);
            ReadNodes(r);
            ReadVisArea(r);
        }

        /// <summary>
        /// Reads the .data file of the sector.
        /// </summary>
        /// <param name="path">The .data file of the sector.</param>
        /// <param name="fs">The file system to load the file from.</param>
        private void ReadData(string path, IFileSystem fs)
        {
            using var fileStream = fs.Open(path);
            using var r = new BinaryReader(fileStream);

            // Header
            var header = new Header();
            header.Deserialize(r);

            // Items
            while (r.BaseStream.Position < r.BaseStream.Length)
            {
                var uid = r.ReadUInt64();
                if (uid == EofMarker)
                    break;

                if (!MapItems.TryGetValue(uid, out MapItem item))
                {
                    throw new KeyNotFoundException($"{ToString()}.{Sector.DataExtension} contains " +
                        $"unknown UID {uid:X} - can't continue.");
                }
                var serializer = (IDataPayload)MapItemSerializerFactory.Get(item.ItemType);
                serializer.DeserializeDataPayload(r, item);
            }
        }

        /// <summary>
        /// Reads the .snd file of the sector.
        /// </summary>
        /// <param name="path">The .snd file of the sector.</param>
        /// <param name="fs">The file system to load the file from.</param>
        private void ReadSnd(string path, IFileSystem fs)
        {
            if (!fs.FileExists(path))
                return;

            using var fileStream = fs.Open(path);
            using var r = new BinaryReader(fileStream);

            var header = new Header();
            header.Deserialize(r);
            ReadItems(r, ItemFile.Snd);
            ReadNodes(r);
            ReadVisArea(r);
        }

        /// <summary>
        /// Reads the .layer file of the sector.
        /// </summary>
        /// <param name="path">The .layer file of the sector.</param>
        /// <param name="fs">The file system to load the file from.</param>
        /// <exception cref="KeyNotFoundException"></exception>
        private void ReadLayer(string path, IFileSystem fs)
        {
            if (!fs.FileExists(path))
                return;

            using var fileStream = fs.Open(path);
            using var r = new BinaryReader(fileStream);

            var header = new Header();
            header.Deserialize(r);

            while (r.BaseStream.Position < r.BaseStream.Length)
            {
                var uid = r.ReadUInt64();
                if (uid == EofMarker)
                    break;

                if (!MapItems.TryGetValue(uid, out MapItem item))
                {
                    throw new KeyNotFoundException($"{ToString()}.{Sector.LayerExtension} contains " +
                        $"unknown UID {uid:X} - can't continue.");
                }
                var layer = r.ReadByte();
                item.Layer = layer;
            }
        }

        /// <summary>
        /// Reads items from a .base/.aux/.snd file.
        /// </summary>
        /// <param name="r">A BinaryReader at the start of the item section.</param>
        /// <param name="file">The file which is being read.</param>
        private void ReadItems(BinaryReader r, ItemFile file)
        {
            var itemCount = r.ReadUInt32();
            MapItems.EnsureCapacity(MapItems.Count + (int)itemCount);
            for (int i = 0; i < itemCount; i++)
            {
                var itemType = (ItemType)r.ReadInt32();

                var serializer = MapItemSerializerFactory.Get(itemType);
                var item = serializer.Deserialize(r);

                // deal with signs which can be in aux *and* base
                if (item is Sign sign && file != sign.DefaultItemFile)
                {
                    sign.ItemFile = file;
                }

                MapItems.Add(item.Uid, item);
            }
        }

        /// <summary>
        /// Reads the node section of a .base/.aux/.snd file.
        /// </summary>
        /// <param name="r">A BinaryReader at the start of the node section.</param>
        private void ReadNodes(BinaryReader r)
        {
            var nodeCount = r.ReadUInt32();
            for (int i = 0; i < nodeCount; i++)
            {
                var node = new Node(false);
                node.Deserialize(r);
                node.Parent = this;
                if (!Nodes.ContainsKey(node.Uid))
                {
                    Nodes.Add(node.Uid, node);
                }
            }
        }

        /// <summary>
        /// Reads the VisAreaChild section of a .base/.aux/.snd file.
        /// </summary>
        /// <param name="r">A BinaryReader at the start of the VisAreaChild section.</param>
        private void ReadVisArea(BinaryReader r)
        {
            // I think we can safely ignore this when deserializing
            var visAreaChildCount = r.ReadUInt32();
            for (int i = 0; i < visAreaChildCount; i++)
            {
                r.ReadUInt64();
            }
        }

        /// <summary>
        /// Fills in the Node and Forward/BackwardItem fields in map item and node objects
        /// by searching the Node or Item list for the UID references.
        /// </summary>
        private void UpdateReferences()
        {
            // first of all, find map items referenced in nodes
            Trace.WriteLine("Updating item references in nodes");
            foreach (var (_, node) in Nodes)
            {
                node.UpdateItemReferences(MapItems);
            }

            // then find nodes referenced in map items
            // and map items referenced in map items
            Trace.WriteLine("Updating node & item references in items");
            foreach (var (_, item) in MapItems)
            {
                item.UpdateNodeReferences(Nodes);
                if (item is IItemReferences hasItemRef)
                {
                    hasItemRef.UpdateItemReferences(MapItems);
                }
            }
        }

        /// <summary>
        /// Saves the map in binary format. If the sector directory does not yet exist, it will be created.
        /// </summary>
        /// <param name="mapDirectory">The path of the directory to save the map into.</param>
        /// <param name="cleanSectorDirectory">If true, the sector directory will be emptied
        /// before saving the map.</param>
        public void Save(string mapDirectory, bool cleanSectorDirectory = true)
        {
            var sectorDirectory = Path.Combine(mapDirectory, Name);
            Directory.CreateDirectory(sectorDirectory);
            if (cleanSectorDirectory)
            {
                var files = new DirectoryInfo(sectorDirectory).GetFiles();
                foreach (var file in files)
                    file.Delete();
            }

            var farModelChildren = GetFarModelChildren();
            var allItemsBySector = GetSectorItems(farModelChildren);
            var visAreaShowObjectsChildren = GetVisAreaShowObjectsChildUids();

            foreach (var (sectorCoord, sector) in Sectors)
            {
                if (allItemsBySector.TryGetValue(sectorCoord, out var sectorItems))
                {
                    var sectorNodes = GetSectorNodes(sectorItems, farModelChildren);
                    Trace.WriteLine($"Writing sector {sector}");
                    var sectorVisAreaChildren = sectorItems.BaseItems.Concat(sectorItems.AuxItems)
                        .Where(item => visAreaShowObjectsChildren.Contains(item.Uid))
                        .Select(item => item.Uid);
                    SaveSector(sectorCoord, sectorDirectory, sectorItems, sectorNodes, 
                        sectorVisAreaChildren);
                    sector.WriteDesc(GetSectorFilename(sectorCoord, sectorDirectory, Sector.DescExtension));
                }
            }

            var mbdPath = Path.Combine(mapDirectory, $"{Name}.mbd");
            SaveMbd(mbdPath);
        }

        private static SectorNodes GetSectorNodes(SectorItems sectorItems, HashSet<IMapItem> farModelChildren)
        {
            var sectorNodes = new SectorNodes();

            foreach (var item in sectorItems.BaseItems)
                AddTo(sectorNodes.BaseNodes, item);

            foreach (var item in sectorItems.AuxItems)
            {
                if (farModelChildren.Contains(item))
                    AddTo(sectorNodes.BaseNodes, item);
                else
                    AddTo(sectorNodes.AuxNodes, item);
            }

            foreach (var item in sectorItems.SndItems)
                AddTo(sectorNodes.SndNodes, item);

            return sectorNodes;

            static void AddTo(HashSet<Node> nodeSet, MapItem item)
            {
                nodeSet.UnionWith(
                    item.GetItemNodes()
                    .Where(x => x is not UnresolvedNode)
                    .Cast<Node>());
            }
        }

        private HashSet<ulong> GetVisAreaShowObjectsChildUids()
        {
            var children = new HashSet<ulong>();
            foreach (var (_, visArea) in MapItems.Where(
                x => x.Value is VisibilityArea v &&
                v.Behavior == VisibilityAreaBehavior.ShowObjects))
            {
                foreach (var child in (visArea as VisibilityArea).Children)
                {
                    children.Add(child.Uid);
                }
            }
            return children;
        }

        private Dictionary<SectorCoordinate, SectorItems> GetSectorItems(HashSet<IMapItem> farModelChildren)
        {
            var items = new Dictionary<SectorCoordinate, SectorItems>();

            foreach (var (_, item) in MapItems)
            {
                var center = item.GetCenter();
                var sectorCoord = GetSectorOfCoordinate(center);

                if (!items.TryGetValue(sectorCoord, out SectorItems sectorItems))
                {
                    sectorItems = new SectorItems();
                    items.Add(sectorCoord, sectorItems);
                }

                switch (item.ItemFile)
                {
                    case ItemFile.Base:
                        sectorItems.BaseItems.Add(item);
                        break;
                    case ItemFile.Aux:
                        if (farModelChildren.Contains(item))
                            sectorItems.BaseItems.Add(item);
                        else
                            sectorItems.AuxItems.Add(item);
                        break;
                    case ItemFile.Snd:
                        sectorItems.SndItems.Add(item);
                        break;
                    default:
                        throw new Exception($"Invalid item.ItemFile value {item.ItemFile}");
                }
            }

            return items;
        }

        private HashSet<IMapItem> GetFarModelChildren()
        {
            var farModelChildren = new HashSet<IMapItem>();
            foreach (var fm in MapItems.Where(x => x.Value is FarModel).Select(x => x.Value as FarModel))
            {
                if (fm.UseMapItems)
                    farModelChildren.UnionWith(fm.Children);
            }

            return farModelChildren;
        }

        /// <summary>
        /// Writes the .mbd file of this map.
        /// </summary>
        /// <param name="mbdPath">The path of the .mbd file.</param>
        private void SaveMbd(string mbdPath)
        {
            var stream = new FileStream(mbdPath, FileMode.Create);
            using var w = new BinaryWriter(stream);

            header.Serialize(w);

            w.Write(EditorMapId);
            w.Write(StartPosition);
            w.Write(StartSectorOrSomething);
            w.Write(StartRotation);
            w.Write(gameTag);
            w.Write(NormalScale);
            w.Write(CityScale);
            w.Write(EuropeMapUiCorrections.ToByte());
        }

        /// <summary>
        /// Saves the sector in binary format to the specified directory.
        /// </summary>
        /// <param name="coord">The coordinate of the sector to write.</param>
        /// <param name="sectorDirectory">The sector directory.</param>
        /// <param name="sectorItems">Items which belong to this sector.</param>
        /// <param name="sectorNodes">Nodes which belong to this sector.</param>
        /// <param name="sectorVisAreaChildren">UIDs of VisAreaChildren for this sector.</param>
        private void SaveSector(SectorCoordinate coord, string sectorDirectory, 
            SectorItems sectorItems, SectorNodes sectorNodes,
            IEnumerable<ulong> sectorVisAreaChildren)
        {
            WriteBase(GetSectorFilename(coord, sectorDirectory, Sector.BaseExtension), 
                sectorItems.BaseItems, sectorNodes.BaseNodes, sectorVisAreaChildren);
            WriteData(GetSectorFilename(coord, sectorDirectory, Sector.DataExtension), 
                sectorItems.BaseItems);
            WriteAux(GetSectorFilename(coord, sectorDirectory, Sector.AuxExtenstion), 
                sectorItems.AuxItems, sectorNodes.AuxNodes, sectorVisAreaChildren);
            WriteSnd(GetSectorFilename(coord, sectorDirectory, Sector.SndExtension), 
                sectorItems.SndItems, sectorNodes.SndNodes);
            WriteLayer(GetSectorFilename(coord, sectorDirectory, Sector.LayerExtension),
                sectorItems);
        }

        private static string GetSectorFilename(SectorCoordinate coord, string sectorDirectory, string ext) =>
            Path.Combine(sectorDirectory, $"{Sector.SectorFileNameFromSectorCoords(coord)}.{ext}");

        /// <summary>
        /// Writes the .base part of this sector.
        /// </summary>
        /// <param name="path">The path of the output file.</param>
        /// <param name="baseItems">A list of all items which belong to the base part of this sector.</param>
        /// <param name="baseNodes">A set of all nodes which belong to the base part of this sector.</param>
        /// <param name="sectorVisAreaChildren">UIDs of VisAreaChildren for this sector.</param>
        private void WriteBase(string path, List<MapItem> baseItems,
            HashSet<Node> baseNodes, IEnumerable<ulong> sectorVisAreaChildren)
        {
            using var stream = new FileStream(path, FileMode.Create);
            using var w = new BinaryWriter(stream);
            header.Serialize(w);
            WriteItems(w, baseItems);
            WriteNodes(w, baseNodes);
            WriteVisAreaChildren(w, ItemFile.Base, sectorVisAreaChildren);
        }

        /// <summary>
        /// Writes the .aux part of the sector.
        /// </summary>
        /// <param name="path">The path of the output file.</param>
        /// <param name="auxItems">A list of all items which belong to the aux part of this sector.</param>
        /// <param name="auxNodes">A set of all nodes which belong to the aux part of this sector.</param>
        /// <param name="sectorVisAreaChildren">UIDs of VisAreaChildren for this sector.</param>
        private void WriteAux(string path, List<MapItem> auxItems,
             HashSet<Node> auxNodes, IEnumerable<ulong> sectorVisAreaChildren)
        {
            using var stream = new FileStream(path, FileMode.Create);
            using var w = new BinaryWriter(stream);
            header.Serialize(w);
            WriteItems(w, auxItems);
            WriteNodes(w, auxNodes);
            WriteVisAreaChildren(w, ItemFile.Aux, sectorVisAreaChildren);
        }

        /// <summary>
        /// Writes the .snd part of the sector.
        /// </summary>
        /// <param name="path">The path of the output file.</param>
        /// <param name="sndItems">A list of all items which belong to the snd part of this sector.</param>
        /// <param name="sndNodes">A set of all nodes which belong to the snd part of this sector.</param>
        private void WriteSnd(string path, List<MapItem> sndItems,
            HashSet<Node> sndNodes)
        {
            using var stream = new FileStream(path, FileMode.Create);
            using var w = new BinaryWriter(stream);
            header.Serialize(w);
            WriteItems(w, sndItems);
            WriteNodes(w, sndNodes);
            w.Write(0); // vis_area_child_count
        }

        /// <summary>
        /// Writes the .data part of this sector.
        /// </summary>
        /// <param name="path">The path of the output file.</param>
        /// <param name="baseItems">A list of all base items in this sector.</param>
        private void WriteData(string path, List<MapItem> baseItems)
        {
            using var stream = new FileStream(path, FileMode.Create);
            using var w = new BinaryWriter(stream);

            header.Serialize(w);

            foreach (var item in baseItems.Where(x => x.HasDataPayload))
            {
                w.Write(item.Uid);
                var serializer = (IDataPayload)MapItemSerializerFactory.Get(item.ItemType);
                serializer.SerializeDataPayload(w, item);
            }

            w.Write(EofMarker);
        }

        /// <summary>
        /// Writes the .layer part of the sector.
        /// </summary>
        /// <param name="path">The path of the output file.</param>
        /// <param name="sectorItems">A list of all items in this sector.</param>
        private void WriteLayer(string path, SectorItems sectorItems)
        {
            var itemsWithSetLayers = 
                sectorItems.BaseItems
                .Concat(sectorItems.AuxItems)
                .Concat(sectorItems.SndItems)
                .Where(x => x.Layer != MapItem.DefaultLayer);
            if (!itemsWithSetLayers.Any())
                return;

            using var stream = new FileStream(path, FileMode.Create);
            using var w = new BinaryWriter(stream);

            header.Serialize(w);
            foreach (var item in itemsWithSetLayers)
            {
                w.Write(item.Uid);
                w.Write(item.Layer);
            }
            w.Write(EofMarker);
        }

        /// <summary>
        /// Writes the node part of a .base/.aux/.snd file.
        /// </summary>
        /// <param name="w">The writer.</param>
        /// <param name="nodes">The nodes to write.</param>
        private static void WriteNodes(BinaryWriter w, HashSet<Node> nodes)
        {
            w.Write(nodes.Count);
            foreach (var node in nodes.OrderBy(x => x.Uid))
            {
                node.Serialize(w);
            }
        }

        /// <summary>
        /// Writes .base/.aux/.snd items to a .base/.aux/.snd file.
        /// </summary>
        /// <param name="w">The writer.</param>
        /// <param name="sectorItems">The items to write.</param>
        private static void WriteItems(BinaryWriter w, List<MapItem> items)
        {
            w.Write(items.Count);
            foreach (var item in items.OrderBy(x => x.Uid))
            {
                w.Write((int)item.ItemType);
                var serializer = MapItemSerializerFactory.Get(item.ItemType);
                serializer.Serialize(w, item);
            }
        }

        /// <summary>
        /// Writes the VisAreaChildren part of a .base/.aux/.snd file.
        /// </summary>
        /// <param name="w">A BinaryWriter.</param>
        /// <param name="file">The item file which is being written.</param>
        /// <param name="visAreaShowObjectsChildren">UIDs of the map items which need to be written.</param>
        private void WriteVisAreaChildren(BinaryWriter w, ItemFile file, IEnumerable<ulong> visAreaShowObjectsChildren)
        {
            List<ulong> uidsInFile = visAreaShowObjectsChildren
                .Where(childUid => MapItems.TryGetValue(childUid, out var child) && child.ItemFile == file)
                .Order()
                .ToList();

            w.Write(uidsInFile.Count);
            foreach (var childUid in uidsInFile)
            {
                w.Write(childUid);
            }
        }

        private class SectorItems()
        {
            public List<MapItem> BaseItems { get; } = [];
            public List<MapItem> AuxItems { get; } = [];
            public List<MapItem> SndItems { get; } = [];
        }

        private class SectorNodes()
        {
            public HashSet<Node> BaseNodes { get; } = [];
            public HashSet<Node> AuxNodes { get; } = [];
            public HashSet<Node> SndNodes { get; } = [];
        }
    }
}
