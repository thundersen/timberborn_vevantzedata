using System.Linq;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;

namespace VeVantZeData.Collector
{
    internal static class EntityComponentExtensions
    {
        internal static bool HasDistrictCenter(this EntityComponent ec) {
            return ec.RegisteredComponents.Any(c => c.GetType() == typeof(DistrictCenter));
        }
    }
}