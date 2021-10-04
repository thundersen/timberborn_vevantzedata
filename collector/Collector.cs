using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Characters;
using Timberborn.GameDistricts;

namespace VeVantZeData.Collector
{
    class Collector
    {
        private readonly HashSet<DistrictCenter> _districtCenters = new HashSet<DistrictCenter>();
        private GlobalPopulation _globalPopulation;

        public Collector(GlobalPopulation globalPopulation)
        {
            _globalPopulation = globalPopulation;
        }

        internal void AddDistrictCenter(DistrictCenter districtCenter)
        {
            _districtCenters.Add(districtCenter);
        }

        internal Data Collect()
        {
            var dcPops = CollectDistrictCenters();
            var globalPops = CollectGlobalPopulation();

            return new Data(globalPops, dcPops);
        }

        private IDictionary<String, Pops> CollectDistrictCenters()
        {
            return _districtCenters.Select(Pops).ToDictionary(x => x.Key, x => x.Value);
        }

        private KeyValuePair<String, Pops> Pops(DistrictCenter dc)
        {
            var name = dc.DistrictName;
            var adults = dc.DistrictPopulation.NumberOfAdults;
            var children = dc.DistrictPopulation.NumberOfChildren;

            Plugin.Log.LogInfo($"District: {name} | Adults: {adults} | Children: {children}");

            return KeyValuePair.Create(dc.DistrictName, new Pops(dc.DistrictPopulation.NumberOfAdults, dc.DistrictPopulation.NumberOfChildren));
        }

        private Pops CollectGlobalPopulation()
        {
            if (_globalPopulation == null)
            {
                Plugin.Log.LogDebug("Global pop not initialized yet");
                return new Pops(0, 0);
            }

            var adults = _globalPopulation.NumberOfAdults;
            var children = _globalPopulation.NumberOfChildren;

            Plugin.Log.LogInfo($"Global - Adults: {adults} | Children: {children}");

            return new Pops(adults, children);
        }
    }
}