using ScsReader.ScsMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an empty map
            var map = new Map("example");

            // Add a road segment:
            // Use the [Item Type].Add method to add a new item to the map.
            var r = Road.Add(map,
                new Vector3(19, 0, 19.5f), // position of backward node
                new Vector3(65, 0, 23),    // position of forward node
                "ger1",  // unit name of the road
                80, 80   // terrain size on the left and right side
                );

            // Road properties:
            // For roads that don't consist of two seperate roads
            // (highways etc.), various values introduced with the template
            // system are set on the right side of the road, but apply to both
            // sides, and the values on the left side are ignored.
            r.Right.Look = "ger_1";
            r.Right.Variant = "broken_de";
            r.Right.LeftEdge = "ger_sh_15";
            r.Right.RightEdge = "ger_sh_15";

            // Terrain properties:
            // Define what the terrain will look like.
            // We'll set all of these values on the right side only
            // and then clone it for the left side.
            r.Right.Terrain.QuadData.Material = "34"; // "grass_ger_main"
            r.Right.Terrain.Profile = "profile12"; // "hills2"
            r.Right.Terrain.Noise = TerrainNoise.Percent0;
            r.Right.Terrain.Coefficient = 0.5f;

            // Vegetation properties:
            // Let's add some vegetation.
            r.Right.Vegetation[0].Name = "v2_1ger"; // "ger - mixed forest"
            r.Right.Vegetation[0].Density = 200;
            r.Right.Vegetation[0].From = 15;
            r.Right.Vegetation[0].To = 80;

            // Models:
            // Let's add some reflector posts as well.
            r.Right.Models[0].Name = "219"; // "reflective post"
            r.Right.Models[0].Distance = 50;
            r.Right.Models[0].Offset = 6;

            // Copy all of these properties to the right side.
            r.Left = r.Right.Clone();

            // Append some more segments:
            // By default, Append will copy all settings.
            r.Append(new Vector3(98, 0, 43.5f))
                .Append(new Vector3(146.5f, 0, 25))
                .Append(new Vector3(216, 0, 25));

            // Finally, let's place two models:
            Model.Add(map,
                new Vector3(103.75f, -0.3f, 31.73f), // position 
                -2.99f, // Y rotation in radians
                "dlc_no_471", // unit name of "house_01_sc"
                "brick", // variant
                "default" // look
                );
            Model.Add(map, new Vector3(159.64f, -0.1f, 36.91f), (float)Math.PI / 2,
                "378", "default", "default"); // wood_heap1

            // Save the map
            var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var gameFolder = Path.Combine(documentsFolder, "Euro Truck Simulator 2/mod");
            var modName = "example";
            map.Save(Path.Combine(gameFolder, modName, "map"), true);

            // Don't forget to hit F8 after loading it in the editor for the first time.
        }
    }
}
