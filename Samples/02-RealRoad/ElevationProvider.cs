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
            var geoPoints = elevationService.GetLineGeometryElevation(
                points.Select(p => new GeoPoint(p.Latitude, p.Longitude)), 
                dataset).ToList();
            return geoPoints.Select(
                x => new GeographicCoordinate(x.Latitude, x.Longitude, x.Elevation ?? 0))
                .ToList();
        }
    }
}
