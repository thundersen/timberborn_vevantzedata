using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Characters;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.ResourceCountingSystem;
using Timberborn.TimeSystem;
using Timberborn.WeatherSystem;

namespace VeVantZeData.Collector
{
    class Collector
    {
        private readonly HashSet<DistrictCenter> _districtCenters = new HashSet<DistrictCenter>();
        private readonly GlobalPopulation _globalPopulation;
        private readonly WeatherService _weatherService;
        private readonly DayNightCycle _dayNightCycle;
        private readonly IEnumerable<GoodSpecification> _goodSpecs;
        private readonly Func<ResourceCountingService> _resourceCountingService;

        public Collector(GlobalPopulation globalPopulation, WeatherService weatherService, DayNightCycle dayNightCycle, IEnumerable<GoodSpecification> goodSpecs, Func<ResourceCountingService> resourceCountingService)
        {
            _globalPopulation = globalPopulation;
            _weatherService = weatherService;
            _dayNightCycle = dayNightCycle;
            _goodSpecs = goodSpecs;
            _resourceCountingService = resourceCountingService;
        }

        internal void AddDistrictCenter(DistrictCenter districtCenter)
        {
            _districtCenters.Add(districtCenter);
        }

        internal Data Collect()
        {
            var dcPops = CollectDistrictCenterPops();
            var globalPops = CollectGlobalPopulation();
            var time = CollectTime();
            var goods = CollectDistrictStocks();

            return new Data(time, globalPops, dcPops, goods);
        }

        private static readonly DateTime _gameStartDate = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly int _secondsPerDay = 60 * 60 * 24;

        private GameTime CollectTime()
        {
            var secondsSinceGameStart = (_dayNightCycle.PartialDayNumber - 1) * _secondsPerDay;
            var currentGameTime = _gameStartDate.AddSeconds(secondsSinceGameStart);

            Plugin.Log.LogDebug($"partial day: {_dayNightCycle.PartialDayNumber} in seconds: {secondsSinceGameStart} \n  => game time {currentGameTime}");

            return new GameTime(DateTime.Now, currentGameTime, _weatherService.Cycle, _weatherService.CycleDay, _dayNightCycle.DayNumber, _dayNightCycle.DayProgress);
        }

        private IDictionary<String, Pops> CollectDistrictCenterPops()
        {
            return _districtCenters.Select(Pops).ToDictionary(x => x.Key, x => x.Value);
        }

        private KeyValuePair<String, Pops> Pops(DistrictCenter dc)
        {
            var name = dc.DistrictName;
            var adults = dc.DistrictPopulation.NumberOfAdults;
            var children = dc.DistrictPopulation.NumberOfChildren;

            Plugin.Log.LogDebug($"District: {name} | Adults: {adults} | Children: {children}");

            return KeyValuePair.Create(dc.DistrictName, new Pops(dc.DistrictPopulation.NumberOfAdults, dc.DistrictPopulation.NumberOfChildren));
        }

        private Pops CollectGlobalPopulation()
        {
            var adults = _globalPopulation.NumberOfAdults;
            var children = _globalPopulation.NumberOfChildren;

            Plugin.Log.LogDebug($"Global - Adults: {adults} | Children: {children}");

            return new Pops(adults, children);
        }

        private IDictionary<string, Goods> CollectDistrictStocks()
        {
            var countingService = _resourceCountingService.Invoke();

            // ResourceCountingService is a singleton used by the game for displaying the good amounts of the currently selected district in the ui.
            // So it's important to reset it after we abused it.
            var previousDC = countingService.GetInstanceField<ResourceCountingService, DistrictCenter>("_districtCenter");

            var result = new Dictionary<string, Goods>();

            foreach (var dc in _districtCenters)
            {
                countingService.SwitchDistrict(dc);

                var goodCounts = _goodSpecs.ToDictionary(s => s.Id, s => countingService.GetDistrictAmount(s));

                result.Add(dc.DistrictName, new Goods(goodCounts));
            }

            countingService.SwitchDistrict(previousDC);

            return result;
        }
    }
}