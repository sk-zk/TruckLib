using DotSpatial.Projections;
using System.Globalization;
using System.Numerics;
using TruckLib.ScsMap;

namespace RealRoad
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // This sample will show one possible approach to import
            // real data into a map for 1:1 mapping purposes.
            // You can find a more detailed walkthrough of the code in
            // the documentation.


            // 1)
            // Load our coordinates.
            // In this case, it's a CSV file containing the course of a road
            // in Germany (© OpenStreetMap contributors).
            var coordinates = LoadCoordinates();


            // 2)
            // Fetch height above mean sea level for each point.
            var elevationProvider = ElevationProvider.Create();
            elevationProvider.SetElevations(coordinates);


            // 3)
            // Project the coordinates.
            // The game world is flat, so our coordinates need to be projected.
            // In this sample, we'll use a transverse Mercator; more specifically,
            // UTM zone 32N, which is the zone most of Germany falls into.
            var sourceCrs = KnownCoordinateSystems.Geographic.World.WGS1984;
            var destCrs = KnownCoordinateSystems.Projected.UtmWgs1984.WGS1984UTMZone32N;
            var points = Project(coordinates, sourceCrs, destCrs);

            // We also project a center point which will be subtracted
            // from all of our road points.
            var center = Project(new[] { new GeographicCoordinate(54.744101, 9.799639) },
                sourceCrs, destCrs)[0];


            // 4)
            // Create or open a map.
            var map = new Map("example");


            // 5)
            // Add the road to the map.
            // We start by creating the first road item explicitly with
            // the first two points of the road, and then append the rest in a loop.
            var road = Road.Add(map,
                PointToNodePosition(points[0], center),
                PointToNodePosition(points[1], center),
                "ger1");

            for (int i = 2; i < points.Count; i++)
            {
                road = road.Append(PointToNodePosition(points[i], center));
            }


            // 6)
            // Save the map.
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var userMap = Path.Combine(documents, "Euro Truck Simulator 2/mod/user_map/map/");
            map.Save(userMap, true);


            // Remember to recalculate (Map > Recompute map) after loading it in the editor for the first time.
        }

        static Vector3 PointToNodePosition(ProjectedCoordinate point, ProjectedCoordinate center)
        {
            return new Vector3((float)(point.X - center.X), 
                (float)point.Height, 
                -(float)(point.Y - center.Y));
        }

        static List<GeographicCoordinate> LoadCoordinates()
        {
            var coordinates = new List<GeographicCoordinate>();
            using var reader = new StreamReader("road.csv");
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine()!.Split(",");
                var latitude = double.Parse(line[0], CultureInfo.InvariantCulture);
                var longitude = double.Parse(line[1], CultureInfo.InvariantCulture);
                coordinates.Add(new GeographicCoordinate(latitude, longitude));
            }
            return coordinates;
        }

        static List<ProjectedCoordinate> Project(IList<GeographicCoordinate> coordinates, 
            ProjectionInfo source, ProjectionInfo dest)
        {
            // Convert our list into the flat array expected by the library
            var xy = new double[coordinates.Count * 2];
            for (int i = 0; i < coordinates.Count; i++)
            {
                xy[i * 2] = coordinates[i].Longitude;
                xy[(i * 2) + 1] = coordinates[i].Latitude;
            }

            // Project the points in-place
            Reproject.ReprojectPoints(xy, null, source, dest, 0, coordinates.Count);

            // And convert it back into a list of ProjectedCoordinates
            var points = new List<ProjectedCoordinate>(coordinates.Count);
            for (int i = 0; i < coordinates.Count; i++)
            {
                var easting = xy[i * 2];
                var northing = xy[(i * 2) + 1];
                points.Add(new ProjectedCoordinate(easting, northing, coordinates[i].Height));
            }
            return points;
        }
    }

    struct GeographicCoordinate
    {
        public double Latitude;
        public double Longitude;
        public double Height;
        public GeographicCoordinate(double latitude, double longitude, double height = 0)
        {
            Latitude = latitude;
            Longitude = longitude;
            Height = height;
        }
    }

    struct ProjectedCoordinate
    {
        public double X;
        public double Y;
        public double Height;
        public ProjectedCoordinate(double x, double y, double height = 0)
        {
            X = x;
            Y = y;
            Height = height;
        }
    }
}
