using System;
using System.Linq;
using System.Collections.Generic;
using TruckLib;
using System.IO;
using TruckLib.ScsMap;
using System.Numerics;
using TruckLib.Model.Ppd;

namespace Prefabs
{
    class Program
    {
        static void Main(string[] args)
        {
            // In this example, I'm going to demonstrate how placing prefabs
            // in a map currently works.
            // As before, positions of items are hardcoded for simplicity.

            var map = new Map("example");

            // (Modify this path before running)
            var extractedRoot = @"D:\SteamLibrary\steamapps\common\Euro Truck Simulator 2\_extracted";

            // 1) Add a company prefab to the map.
            // The most convenient way to do this is to load the .ppd file 
            // of the model and let the library deal with placing the nodes
            // and slave items.

            var companyPpdPath = Path.Combine(extractedRoot,
                @"prefab2/car_dealer/car_dealer_01_fr.ppd");
            var companyPpd = PrefabDescriptor.Open(companyPpdPath);
         
            var companyPos = new Vector3(55, 0, 70);
            var companyRot = Quaternion.CreateFromYawPitchRoll(1.5708f, 0, 0); // 90°                    
            
            var company = Prefab.Add(map,
                "dlc_fr_14", // Unit name
                "bhv_fr",    // Variant
                "green_fr",  // Look
                companyPpd,  // ppd
                companyPos,  // position of node 0
                companyRot   // optional: rotation
                );
                         
            // 2) Attach a T-crossing prefab to the entrance of the company.
  
            var crossingPpdPath = Path.Combine(extractedRoot,
                @"prefab2/cross_temp/fr/fr_r1_x_r1_t_narrow_tmpl.ppd");
            var crossingPpd = PrefabDescriptor.Open(crossingPpdPath);

            var crossingPos = companyPos;
            
            var crossing = Prefab.Add(map,
                "387",
                "shoul_fr_1",
                "gray_fr", 
                crossingPpd,
                crossingPos
                );

            // The T node of this prefab is the origin node, so to connect it
            // to the company prefab's node, which is also red, we'll have to
            // change the origin node of the crossing.
            crossing.ChangeOrigin(1);

            // Now attach it.
            company.Attach(crossing);
            
            // 3) Finally, let's connect the other ends of the prefab with a road.
            var road = crossing.AppendRoad(map, 1, new Vector3(100, 0, 88), "ger1");
            road = road.Append(new Vector3(100, 0, 52));
            road = road.Append(crossing.Nodes[0].Position);
            crossing.Attach(road);
            
            // Save the map
            var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var modFolder = Path.Combine(documentsFolder, "Euro Truck Simulator 2/mod");
            var modName = "example";
            map.Save(Path.Combine(modFolder, modName, "map"), true);

            // Don't forget to hit F8 after loading it in the editor for the first time.
        }
    }
}
