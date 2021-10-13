using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Characters;
using Timberborn.GameDistricts;
using VeVantZeData.Collector.GameAdapters;

namespace VeVantZeData.Collector.Collection
{
    class MetricsCollector
    {
        private readonly HashSet<DistrictCenter> _districtCenters = new HashSet<DistrictCenter>();
        private readonly IDistricts _districts;
        private readonly GlobalPopulation _globalPopulation;
        private readonly IGameTime _time;
        private readonly IGoods _goods;

        internal MetricsCollector(IDistricts districts, GlobalPopulation globalPopulation, IGameTime time, IGoods goods)
        {
            _districts = districts;
            _globalPopulation = globalPopulation;
            _time = time;
            _goods = goods;
        }

        internal Data Collect()
        {
            var time = CollectTime();
            var dcPops = _districts.AllCurrentPopsByDistrict();
            var globalPops = CollectGlobalPopulation();
            var dcStocks = _goods.AllCurrentGoodsByDistrict();
            var globalStocks = GlobalStocksFrom(dcStocks);

            return new Data(time, globalPops, dcPops, globalStocks, dcStocks);
        }

        private static readonly DateTime _gameStartDate = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly int _secondsPerDay = 60 * 60 * 24;

        private GameTime CollectTime()
        {
            var secondsSinceGameStart = (_time.PartialDayNumber - 1) * _secondsPerDay;
            var currentGameTime = _gameStartDate.AddSeconds(secondsSinceGameStart);

            Plugin.Log.LogDebug($"partial day: {_time.PartialDayNumber} in seconds: {secondsSinceGameStart} \n  => game time {currentGameTime}");

            return new GameTime(DateTime.Now, currentGameTime, _time.Cycle, _time.CycleDay, _time.DayNumber, _time.DayProgress);
        }

        private Pops CollectGlobalPopulation()
        {
            //TODO: sum up district counts instead of depending on globalpopulation class
            var adults = _globalPopulation.NumberOfAdults;
            var children = _globalPopulation.NumberOfChildren;

            Plugin.Log.LogDebug($"Global - Adults: {adults} | Children: {children}");

            return new Pops(adults, children);
        }

        private Goods GlobalStocksFrom(IDictionary<string, Goods> districtStocks)
        {
            var goods = districtStocks.Values.First().Counts.Keys;

            var globalCounts = new Dictionary<string, int>();
            foreach (var districtCounts in districtStocks.Values.Select(g => g.Counts))
            {
                foreach (var good in goods)
                {
                    if (!globalCounts.ContainsKey(good))
                        globalCounts[good] = 0;

                    globalCounts[good] += districtCounts[good];
                }
            }
            return new Goods(globalCounts);
        }
    }
}