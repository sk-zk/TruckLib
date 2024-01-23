using System;
using System.IO;
using System.Numerics;
using TruckLib;
using TruckLib.ScsMap;
using TruckLib.Model.Ppd;
using TruckLib.HashFs;

namespace Prefabs
{
    class Program
    {
        static void Main(string[] args)
        {
            // This example program demonstrates how to add prefabs to a map.
            // As before, positions are hardcoded for simplicity.

            var map = new Map("example");

            // (Modify this path before running)
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
            var companyDescriptorFile = baseScs.Extract("/prefab2/car_dealer/car_dealer_01_fr.ppd");
            var companyDescriptor = PrefabDescriptor.Load(companyDescriptorFile);

            var companyPos = new Vector3(55, 0, 70);
            var companyRot = Quaternion.CreateFromYawPitchRoll(1.5708f, 0, 0); // 90°

            var company = Prefab.Add(map,
                "dlc_fr_14",        // Unit name
                companyDescriptor,  // .ppd
                companyPos,         // position of node 0
                companyRot          // optional: rotation
                );
            company.Variant = "bhv_fr";
            company.Look = "green_fr";


            // 2)
            // Attach a T-crossing prefab to the entrance of the company.
            var crossingDescriptorFile = baseScs.Extract("/prefab2/cross_temp/fr/fr_r1_x_r1_t_narrow_tmpl.ppd");
            var crossingDescriptor = PrefabDescriptor.Load(crossingDescriptorFile);

            var crossingPos = companyPos;

            var crossing = Prefab.Add(map,
                "387",
                crossingDescriptor,
                crossingPos
                );
            crossing.Variant = "shoul_fr_1";
            crossing.Look = "gray_fr";

            // The T node of this prefab is the origin node, so to connect it
            // to the company prefab's node, which is also red, we'll have to
            // change the origin node of the crossing.
            crossing.ChangeOrigin(1);

            // Now attach it.
            company.Attach(crossing);


            // 3)
            // Finally, let's connect the other ends of the prefab with a road.
            var road = crossing.AppendRoad(map, 1, new Vector3(100, 0, 88), "ger1")
                .Append(new Vector3(100, 0, 52))
                .Append(crossing.Nodes[0].Position);
            crossing.Attach(road);


            // 4)
            // Save the map
            var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var userMapFolder = Path.Combine(documentsFolder, "Euro Truck Simulator 2/mod/user_map/map/");
            map.Save(userMapFolder, true);

            // 5)
            // Remember to recalculate (Map > Recompute map) after loading it in the editor for the first time.
        }
    }
}
