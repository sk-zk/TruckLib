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

        public void SetElevations(List<GeographicCoordinate> points)
        {
            var geoPoints = elevationService.GetPointsElevation(
                points.Select(p => new GeoPoint(p.Latitude, p.Longitude)), 
                dataset).ToList();
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                point.Height = geoPoints[i].Elevation ?? 0;
                points[i] = point;
            }
        }
    }
}
