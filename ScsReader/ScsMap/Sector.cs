using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// A map sector.
    /// </summary>
    public class Sector
    {
        /// <summary>
        /// The X coordinate of this sector. 
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The Z coordinate of this sector.
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// The map the sector belongs to.
        /// </summary>
        public Map Map { get; set; }

        /// <summary>
        /// The path of the .base file of this sector.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// A list of all map items in this sector.
        /// </summary>
        public Dictionary<ulong, MapItem> MapItems { get; set; } 
            = new Dictionary<ulong, MapItem>();

        /// <summary>
        /// The climate of this sector.
        /// </summary>
        public Token Climate { get; set; } = "default";

        /// <summary>
        /// The header of this sector.
        /// </summary>
        private Header header = new Header();

        // Always 2 in both ETS2 and ATS.
        private uint SectorDescVersion = 2;

        public Vector2 MinBoundary { get; set; } = new Vector2(0, 0);

        public Vector2 MaxBoundary { get; set; } = new Vector2(4000, 4000);

        private BitArray Flags = new BitArray(32);

        /// <summary>
        /// EOF marker of a .data file.
        /// </summary>
        private const ulong dataEof = ulong.MaxValue;

        public Sector()
        {
        }

        /// <summary></summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <param name="map">The map this sector belongs to.</param>
        public Sector(int x, int z, Map map)
        {
            X = x;
            Z = z;
            Map = map;
        }

        /// <summary></summary>
        /// <param name="basePath">The path of the .base file of this sector.</param>
        /// <param name="map">The map this sector belongs to.</param>
        public Sector(string basePath, Map map)
        {
            BasePath = basePath;
            GetSectorCoordsFromBasePath(basePath);
            Map = map;
        }
        
        /// <summary>
        /// Reads the sector from the specified .base path.
        /// </summary>
        public void Read()
        {
            Open(BasePath);
        }

        /// <summary>
        /// Reads the sector.
        /// </summary>
        /// <param name="basePath">The .base file of the sector.</param>
        public void Open(string basePath)
        {
            BasePath = basePath;

            GetSectorCoordsFromBasePath(basePath);

            ReadBase(basePath);
            ReadData(Path.ChangeExtension(basePath, ".data"));
            ReadAux(Path.ChangeExtension(basePath, ".aux"));
            ReadDesc(Path.ChangeExtension(basePath, ".desc"));
        }

        internal void GetSectorCoordsFromBasePath(string basePath)
        {
            var sectorName = Path.GetFileNameWithoutExtension(basePath);
            X = int.Parse(sectorName.Substring(3, 5));
            Z = int.Parse(sectorName.Substring(8, 5));
        }

        /// <summary>
        /// Reads the .base file of the sector.
        /// </summary>
        /// <param name="path">The .base file of the sector.</param>
        private void ReadBase(string path)
        {
            using (var r = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                header = new Header();
                header.ReadFromStream(r);
                ReadMapItems(r, ItemFile.Base);
                ReadNodes(r);
            }
        }

        /// <summary>
        /// Reads the .aux file of the sector.
        /// </summary>
        /// <param name="path">The .aux file of the sector.</param>
        private void ReadAux(string path)
        {
            using (var r = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                var auxHeader = new Header();
                auxHeader.ReadFromStream(r);
                ReadMapItems(r, ItemFile.Aux);
                ReadNodes(r);
            }
        }

        /// <summary>
        /// Reads the .data file of the sector.
        /// </summary>
        /// <param name="path">The .data file of the sector.</param>
        private void ReadData(string path)
        {
            using (var r = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                // Read the header
                var dataHeader = new Header();
                dataHeader.ReadFromStream(r);

                // Read the items
                while (r.BaseStream.Position < r.BaseStream.Length)
                {
                    var uid = r.ReadUInt64();
                    if (uid == dataEof) break;
                    var item = (MapItems.First(x => x.Value.Uid == uid).Value as IDataPart);
                    item.ReadDataPart(r);
                }
            }
        }

        private const float boundaryFactor = 256f;

        /// <summary>
        /// Reads the .desc file of the sector.
        /// </summary>
        /// <param name="path">The .desc file of the sector.</param>
        private void ReadDesc(string path)
        {
            // TODO: 
            // - figure out if there are any desc flags (ets2 & ats 
            //   base maps don't have any)
            // - figure out what exactly the boundaries are and how
            //   they work, because they seem to relate to items
            //   at the borders of the sector

            using (var r = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                // sector desc version
                SectorDescVersion = r.ReadUInt32();

                // boundaries
                MinBoundary = new Vector2(
                    r.ReadInt32() / boundaryFactor,
                    r.ReadInt32() / boundaryFactor
                    );
                MaxBoundary = new Vector2(
                    r.ReadInt32() / boundaryFactor,
                    r.ReadInt32() / boundaryFactor
                    );

                // flags - unknown
                Flags = new BitArray(r.ReadBytes(4));

                // climate profile
                Climate = r.ReadToken();
            }
        }

        /// <summary>
        /// Reads items from a .base or .aux file.
        /// </summary>
        /// <param name="r">The reader.</param>
        /// <param name="file">The file which is being read.</param>
        private void ReadMapItems(BinaryReader r, ItemFile file)
        {
            var itemCount = r.ReadUInt32();
            for (int i = 0; i < itemCount; i++)
            {
                var itemType = (ItemType)r.ReadInt32();

                var item = MapItemFactory.Create(itemType);
                item.ReadFromStream(r);

                // deal with signs which can be in aux *and* base
                if(item is Sign sign && file != sign.DefaultItemFile)
                {
                    sign.ItemFile = file;
                }
                else if(item.DefaultItemFile != file)
                {
                    Trace.WriteLine($"{itemType} in {file}?");
                }

                MapItems.Add(item.Uid, item);
            }
        }

        /// <summary>
        /// Reads the node section of a .base / .aux file.
        /// </summary>
        /// <param name="r">The reader. (Position must be the start of the footer)</param>
        private void ReadNodes(BinaryReader r)
        {
            var nodeCount = r.ReadInt32();
            for (int i = 0; i < nodeCount; i++)
            {
                var node = new Node();
                node.ReadFromStream(this, r);
                if(Map.Nodes.ContainsKey(node.Uid))
                {
                    Map.Nodes[node.Uid].Sectors.Add(this);
                }
                else
                {
                    Map.Nodes.Add(node.Uid, node);
                }
            }
        }
    
        /// <summary>
        /// Saves the sector as binary files to the specified directory.
        /// </summary>
        /// <param name="sectorDirectory">The sector directory.</param>
        public void Save(string sectorDirectory)
        {
            WriteBase(GetFilename("base"), MapItems);
            WriteData(GetFilename("data"), MapItems);
            WriteAux(GetFilename("aux"), MapItems);
            WriteDesc(GetFilename("desc"));

            string GetFilename(string ext)
            {
                return Path.Combine(sectorDirectory, $"{ToString()}.{ext}");
            }
        }

        /// <summary>
        /// Writes the .base part of this sector.
        /// </summary>
        /// <param name="baseFilename">The path of the output file.</param>
        /// <param name="allItems">A list of all items in the sector.</param>
        private void WriteBase(string baseFilename, Dictionary<ulong, MapItem> allItems)
        {
            var stream = new FileStream(baseFilename, FileMode.Create);
            using (var w = new BinaryWriter(stream))
            {
                header.WriteToStream(w);
                WriteItems(allItems, ItemFile.Base, w);
                WriteNodes(w, ItemFile.Base);
            }
        }

        /// <summary>
        /// Writes the .aux part of the sector.
        /// </summary>
        /// <param name="auxFilename">The path of the output file.</param>
        /// <param name="allItems">A list of all items in the sector.</param>
        private void WriteAux(string auxFilename, Dictionary<ulong, MapItem> allItems)
        {
            var stream = new FileStream(auxFilename, FileMode.Create);
            using (var w = new BinaryWriter(stream))
            {
                header.WriteToStream(w);
                WriteItems(allItems, ItemFile.Aux, w);
                WriteNodes(w, ItemFile.Aux);
            }
        }

        /// <summary>
        /// Writes the .data part of this sector.
        /// </summary>
        /// <param name="dataFilename">The path of the output file.</param>
        /// <param name="allItems">A list of all items in the sector.</param>
        private void WriteData(string dataFilename, Dictionary<ulong, MapItem> allItems)
        {
            var stream = new FileStream(dataFilename, FileMode.Create);
            using (var w = new BinaryWriter(stream))
            {
                header.WriteToStream(w);

                foreach(var itemKvp in allItems.Where(x => x.Value is IDataPart))
                {
                    var item = (IDataPart)itemKvp.Value;
                    w.Write((item as MapItem).Uid);
                    item.WriteDataPart(w);
                }

                w.Write(dataEof);
            }
        }

        /// <summary>
        /// Writes the .desc part of the sector.
        /// </summary>
        /// <param name="descFilename">The path of the output file.</param>
        private void WriteDesc(string descFilename)
        {
            var stream = new FileStream(descFilename, FileMode.Create);
            using (var w = new BinaryWriter(stream))
            {
                // SectorDescVersion
                w.Write(SectorDescVersion);

                // boundaries
                w.Write((int)(MinBoundary.X * boundaryFactor));
                w.Write((int)(MinBoundary.Y * boundaryFactor));
                w.Write((int)(MaxBoundary.X * boundaryFactor));
                w.Write((int)(MaxBoundary.Y * boundaryFactor));

                // flags - always 0?
                w.Write(Flags.ToUInt());

                // climate profile
                w.Write(Climate);
            }
        }

        /// <summary>
        /// Writes the node part of a .base / .aux file.
        /// </summary>
        /// <param name="w">The writer.</param>
        private void WriteNodes(BinaryWriter w, ItemFile file)
        {
            // get base nodes only || get aux nodes only
            List<Node> nodes = new List<Node>();
            foreach (var kvp in Map.Nodes.Where(x => x.Value.Sectors.Contains(this)))
            {
                var node = kvp.Value;
                if (!(node.ForwardItem is UnresolvedItem) && node.ForwardItem is MapItem fwItem && fwItem.ItemFile == file)
                {
                    nodes.Add(node);
                }
                else if (!(node.BackwardItem is UnresolvedItem) && node.BackwardItem is MapItem bwItem && bwItem.ItemFile == file)
                {
                    nodes.Add(node);
                }
            }

            w.Write(nodes.Count());
            foreach (var node in nodes)
            {
                node.WriteToStream(w);
            }
        }

        /// <summary>
        /// Writes base / aux items to a base / aux file.
        /// </summary>
        /// <typeparam name="T">Either IBaseItem or IAuxItem, defining which items to write.</typeparam>
        /// <param name="allItems">All items in the sector.</param>
        /// <param name="w">The writer.</param>
        private void WriteItems(Dictionary<ulong, MapItem> allItems, ItemFile file, BinaryWriter w)
        {
            var items = allItems.Where(x => x.Value.ItemFile == file);

            w.Write(items.Count());
            foreach (var item in items)
            {
                var itemType = (int)item.Value.ItemType;
                w.Write(itemType);
                item.Value.WriteToStream(w);
            }
        }

        /// <summary>
        /// Returns the name of this sector as used in filenames and the editor's map overlay.
        /// </summary>
        /// <returns>The name of this sector.</returns>
        public override string ToString()
        {
            return $"sec{X:+0000;-0000;+0000}{Z:+0000;-0000;+0000}";
        }

    }
}
