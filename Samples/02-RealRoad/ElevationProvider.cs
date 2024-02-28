using DEM.Net.Core;
using Microsoft.Extensions.DependencyInjection;

namespace RealRoad
{
    internal class ElevationProvider
    {
        private readonly ElevationService elevationService;
        private readonly DEMDataSet dataset = DEMDataSet.SRTM_GL1;

        public static ElevationProvider Create()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDemNetCore();
            serviceCollection.AddTransient<ElevationProvider>();
            var services = serviceCollection.BuildServiceProvider();
            return services.GetService<ElevationProvider>();
        }

        public ElevationProvider(ElevationService elevationService)
        {
            this.elevationService = elevationService;
        }

        public List<GeographicCoordinate> GetElevations(List<GeographicCoordinate> points)
        {
            elevationService.DownloadMissingFiles(dataset, GetBoundingBoxContainingPoints(points));

            var geoPoints = elevationService.GetLineGeometryElevation(
                points.Select(p => new GeoPoint(p.Latitude, p.Longitude)),
                dataset).ToList();
            return geoPoints.Select(
                x => new GeographicCoordinate(x.Latitude, x.Longitude, x.Elevation ?? 0))
                .ToList();
        }

        private static BoundingBox GetBoundingBoxContainingPoints(List<GeographicCoordinate> points)
        {
            double latMin = double.MaxValue, latMax = double.MinValue;
            double lonMin = double.MaxValue, lonMax = double.MinValue;
            for (int i = 0; i < points.Count; i++)
            {
                if (latMin > points[i].Latitude)
                    latMin = points[i].Latitude;
                else if (latMax < points[i].Latitude)
                    latMax = points[i].Latitude;

                if (lonMin > points[i].Longitude)
                    lonMin = points[i].Longitude;
                else if (lonMax < points[i].Longitude)
                    lonMax = points[i].Longitude;
            }
            var bbox = new BoundingBox(lonMin, lonMax, latMin, latMax);
            return bbox;
        }
    }
}
