using System.Collections.Generic;
using Timberborn.Characters;
using Timberborn.GameDistricts;
using Prometheus;

namespace VeVantZeData.Collector
{
    class Collector
    {
        private static readonly Gauge _beaversGauge = Metrics.CreateGauge("beaver_count", "All beavers...",
            new GaugeConfiguration
            {
                LabelNames = new[] { "district", "lifephase" }
            });

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
            Plugin.Log.LogInfo($"Currently have {_districtCenters.Count} district centers");

            foreach (var dc in _districtCenters)
            {
                var name = dc.DistrictName;
                var adults = dc.DistrictPopulation.NumberOfAdults;
                var children = dc.DistrictPopulation.NumberOfChildren;

                _beaversGauge.WithLabels(name, "adults").Set(adults);
                _beaversGauge.WithLabels(name, "children").Set(children);

                Plugin.Log.LogInfo($"label values so far: {_beaversGauge.GetAllLabelValues()}");

                Plugin.Log.LogInfo($"District: {name} | Adults: {adults} | Children: {children}");
            }
        }

        private void CollectGlobalPopulation()
        {
            if (_globalPopulation == null)
            {
                Plugin.Log.LogDebug("Global pop not initialized yet");
                return;
            }

            var adults = _globalPopulation.NumberOfAdults;
            var children = _globalPopulation.NumberOfChildren;

            _beaversGauge.Set(adults + children);

            Plugin.Log.LogInfo($"Global - Adults: {adults} | Children: {children}");
        }
    }
}