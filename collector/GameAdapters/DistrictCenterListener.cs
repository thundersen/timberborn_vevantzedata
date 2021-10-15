using System.Linq;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;

namespace VeVantZeData.Collector.GameAdapters
{
    class DistrictCenterListener
    {
        internal static DistrictsAdapter DistrictsAdapter { private get; set; }

        internal static void CaptureCreatedDistrictCenter(EntityComponent ec)
        {
            var dc = TryExtractingDistrictCenter(ec);

            if (!dc)
                return;

            DistrictsAdapter.Add(dc);
        }

        internal static void CaptureDestroyedDistrictCenter(EntityComponent ec)
        {
            var dc = TryExtractingDistrictCenter(ec);

            if (!dc)
                return;

            DistrictsAdapter.Remove(dc);
        }

        private static DistrictCenter TryExtractingDistrictCenter(EntityComponent ec)
        {
            return (DistrictCenter)ec.RegisteredComponents.FirstOrDefault(c => c.GetType() == typeof(DistrictCenter));
        }
    }
}