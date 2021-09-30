using System.Collections.Generic;
using Timberborn.Characters;
using Timberborn.GameDistricts;

namespace VeVantZeData.Collector
{
    class Collector
    {
        private static HashSet<DistrictCenter> _districtCenters = new HashSet<DistrictCenter>();
        private static GlobalPopulation _globalPopulation;

        internal static void AddDistrictCenter(DistrictCenter districtCenter)
        {
            _districtCenters.Add(districtCenter);
        }

        internal static void SetGlobalPopulation(GlobalPopulation globalPopulation)
        {
            _globalPopulation = globalPopulation;
        }

        internal void Collect()
        {
            CollectDistrictCenters();
            CollectGlobalPopulation();
        }

        private void CollectDistrictCenters()
        {
            foreach (var dc in _districtCenters)
            {
                var name = dc.DistrictName;
                var adults = dc.DistrictPopulation.NumberOfAdults;
                var children = dc.DistrictPopulation.NumberOfChildren;

                Plugin.Log.LogInfo($"District: {name} | Adults: {adults} | Children: {children}");
            }
        }

        private void CollectGlobalPopulation()
        {
            var adults = _globalPopulation.NumberOfAdults;
            var children = _globalPopulation.NumberOfChildren;

            Plugin.Log.LogInfo($"Global - Adults: {adults} | Children: {children}");
        }
    }
}