using System.Collections.Generic;
using System.Linq;
using Timberborn.GameDistricts;

namespace VeVantZeData.Collector.GameAdapters
{
    class DistrictsAdapter : IDistricts
    {
        private readonly HashSet<DistrictCenter> _districtCenters = new HashSet<DistrictCenter>();
        internal IEnumerable<DistrictCenter> DistrictCenters { get => _districtCenters; }

        internal void Add(DistrictCenter dc)
        {
            Plugin.Log.LogDebug($"adding dc {(dc).DistrictName}");
            _districtCenters.Add(dc);
        }
        internal void Remove(DistrictCenter dc)
        {
            Plugin.Log.LogDebug($"removing dc {(dc).DistrictName}");
            _districtCenters.Remove(dc);
        }

        public IDictionary<string, Pops> AllCurrentPopsByDistrict()
        {
            return _districtCenters.Select(Pops).ToDictionary(x => x.Key, x => x.Value);
        }

        private KeyValuePair<string, Pops> Pops(DistrictCenter dc)
        {
            var name = dc.DistrictName;
            var adults = dc.DistrictPopulation.NumberOfAdults;
            var children = dc.DistrictPopulation.NumberOfChildren;

            //TODO: move to log writer
            Plugin.Log.LogDebug($"District: {name} | Adults: {adults} | Children: {children}");

            return KeyValuePair.Create(dc.DistrictName, new Pops(dc.DistrictPopulation.NumberOfAdults, dc.DistrictPopulation.NumberOfChildren));
        }

    }
}