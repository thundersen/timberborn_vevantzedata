using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Characters;
using Timberborn.GameDistricts;
using Timberborn.TimeSystem;
using Timberborn.WeatherSystem;

namespace VeVantZeData.Collector
{
    class Collector
    {
        private readonly HashSet<DistrictCenter> _districtCenters = new HashSet<DistrictCenter>();
        private GlobalPopulation _globalPopulation;
        private WeatherService _weatherService;
        private DayNightCycle _dayNightCycle;

        public Collector(GlobalPopulation globalPopulation, WeatherService weatherService, DayNightCycle dayNightCycle)
        {
            _globalPopulation = globalPopulation;
            _weatherService = weatherService;
            _dayNightCycle = dayNightCycle;
        }

        internal void AddDistrictCenter(DistrictCenter districtCenter)
        {
            _districtCenters.Add(districtCenter);
        }

        internal Data Collect()
        {
            var dcPops = CollectDistrictCenters();
            var globalPops = CollectGlobalPopulation();
            var time = CollectTime();

            return new Data(time, globalPops, dcPops);
        }

        private static readonly DateTime _gameStartDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly int _secondsPerDay = 60 * 60 * 24;

        private GameTime CollectTime()
        {
            var secondsSinceGameStart = (_dayNightCycle.PartialDayNumber - 1) * _secondsPerDay;
            var currentGameTime = _gameStartDate.AddSeconds(secondsSinceGameStart);

            Plugin.Log.LogDebug($"partial day: {_dayNightCycle.PartialDayNumber} in seconds: {secondsSinceGameStart} \n  => game time {currentGameTime}");

            return new GameTime(DateTime.Now, currentGameTime, _weatherService.Cycle, _weatherService.CycleDay, _dayNightCycle.DayNumber, _dayNightCycle.DayProgress);
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
    }
}