using System;
using System.IO;
using System.Numerics;
using TruckLib.ScsMap;
using TruckLib.Models.Ppd;
using TruckLib.HashFs;

namespace Prefabs
{
    class Program
    {
        static void Main(string[] args)
        {
            // This example program demonstrates how to add prefabs to a map.
            // As before, positions are hardcoded for simplicity.
            // You can find a more detailed walkthrough of the code in
            // the documentation.

            var map = new Map("example");

            // (modify this path before running)
            var gameRoot = @"D:\SteamLibrary\steamapps\common\Euro Truck Simulator 2";


            // 1)
            // Add a company prefab to the map.
            // The most convenient way to do this is to load the .ppd file 
            // of the model and let the library deal with placing the nodes
            // and slave items.

            // Here, we grab the .ppd file straight from base.scs.
            // The game must be closed for this because it locks the file.
            // If you find this inconvenient, you can also load an extracted
            // version with PrefabDescriptor.Open().
            var baseScs = HashFsReader.Open(Path.Combine(gameRoot, "base.scs"));
            var companyDescriptorFile = baseScs.Extract("/prefab2/car_dealer/car_dealer_01_fr.ppd")[0];
            var companyDescriptor = PrefabDescriptor.Load(companyDescriptorFile);

            var company = Prefab.Add(map,
                // position of node 0
                new Vector3(55, 0, 70),
                // Unit name
                "dlc_fr_14",
                // .ppd
                companyDescriptor,
                // rotation (90° in this case)
                Quaternion.CreateFromYawPitchRoll(1.5708f, 0, 0));
            company.Variant = "bhv_fr";
            company.Look = "green_fr";


            // 2)
            // Attach a T junction prefab to the entrance of the company.
            var crossingDescriptorFile = baseScs.Extract("/prefab2/cross_temp/fr/fr_r1_x_r1_t_narrow_tmpl.ppd")[0];
            var crossingDescriptor = PrefabDescriptor.Load(crossingDescriptorFile);

            var crossing = Prefab.Add(map, company.Nodes[0].Position, "387", 
                crossingDescriptor, Quaternion.CreateFromYawPitchRoll(-1.5708f, 0, 0));
            crossing.Variant = "shoul_fr_1";
            crossing.Look = "gray_fr";
            crossing.AdditionalParts.Add("_midlines");

            // The T stem node of this prefab is the origin node, so to connect it
            // to the company prefab's node, which is also the origin, we'll have to
            // change the origin of the crossing.
            crossing.ChangeOrigin(1);

            // Now attach it.
            company.Attach(crossing);


            // 3)
            // Finally, let's connect the other ends of the prefab with a road.
            var road = crossing.AppendRoad(1, new Vector3(100, 0, 88), "template22");
            road.Right.Variant = "broken";
            road.Right.LeftEdge = "fr_sh_15";
            road.Right.RightEdge = "fr_sh_15";
            road = road.Append(new Vector3(100, 0, 52)).Append(crossing.Nodes[0].Position);
            crossing.Attach(road);


            // 4)
            // Save the map
            var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var userMapFolder = Path.Combine(documentsFolder, "Euro Truck Simulator 2/mod/user_map/map/");
            map.Save(userMapFolder, true);


            // Remember to recalculate (Map > Recompute map) after loading it in the editor for the first time.
        }
    }
}
