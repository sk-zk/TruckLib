using System;
using System.IO;
using System.Numerics;
using TruckLib.ScsMap;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // This example program places a few road segments
            // and models at hardcoded locations into an empty map
            // and saves the map into the mod folder.
            // You can find a more detailed walkthrough of the code in
            // the documentation.


            // Create an empty map
            var map = new Map("example");

            // Add a road segment:
            // Use the [Item Type].Add method to create a new item
            // and automatically add it to the map.
            var r = Road.Add(map,
                new Vector3(19, 0, 19.5f), // position of backward (red) node
                new Vector3(65, 0, 23),    // position of forward (green) node
                "ger1",  // unit name of the road type
                80, 80   // terrain size on the left and right side
                );


            // Road properties:
            // For roads that don't consist of two separate roadways,
            // various properties introduced with the template system
            // are set for the right side of the road but apply to both
            // sides, and the values on the left side are ignored.
            r.Right.Look = "ger_1";
            r.Right.Variant = "broken_de";
            r.Right.LeftEdge = "ger_sh_15";
            r.Right.RightEdge = "ger_sh_15";

            // The following properties, however, do exist on both sides,
            // and we want to apply them to both.
            foreach (var side in new[]{r.Left, r.Right})
            {
                // Terrain properties:
                // Define what the terrain will look like.
                side.Terrain.QuadData.BrushMaterials[0] = new Material("34"); // "grass_ger_main"
                side.Terrain.Profile = "profile12"; // "hills2"
                side.Terrain.Noise = TerrainNoise.Percent0;
                side.Terrain.Coefficient = 0.5f;

                // Vegetation properties:
                // Let's add some vegetation.
                side.Vegetation[0].Name = "v2_1ger"; // "ger - mixed forest"
                side.Vegetation[0].Density = 200;
                side.Vegetation[0].From = 15;
                side.Vegetation[0].To = 80;

                // Road models:
                // Let's add some bollards as well.
                side.Models[0].Name = "219"; // "reflective post"
                side.Models[0].Distance = 50;
                side.Models[0].Offset = 6;
            }


            // Append some more segments:
            // By default, Append will copy all settings.
            r.Append(new Vector3(98, 0, 43.5f))
                .Append(new Vector3(146.5f, 0, 25))
                .Append(new Vector3(216, 0, 25));
            

            // Finally, let's place two model items:
            var model1 = Model.Add(map,
                new Vector3(103.75f, -0.3f, 31.73f), // position 
                "dlc_no_471", // unit name of "house_01_sc"
                "brick",      // variant
                "default"     // look
                );
            model1.Node.Rotation = Quaternion.CreateFromYawPitchRoll(-2.99f, 0, 0);

            var model2 = Model.Add(map, new Vector3(159.64f, -0.1f, 36.91f), 
                "378", // "wood_heap1"
                "default", "default"); 
            model2.Node.Rotation = Quaternion.CreateFromYawPitchRoll(MathF.PI / 2, 0, 0);


            // Save the map
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var userMap = Path.Combine(documents, "Euro Truck Simulator 2/mod/user_map/map/");
            map.Save(userMap, true);

            // Remember to recalculate (Map > Recompute map) after loading it in the editor for the first time.
        }
    }
}
