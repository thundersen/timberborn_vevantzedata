using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.GameDistricts;
using VeVantZeData.Collector.GameAdapters;

namespace VeVantZeData.Collector.Collection
{
    class MetricsCollector
    {
        internal static ILog Log = LogWrapper.Default();

        private static readonly DateTime _gameStartDate = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly int _secondsPerDay = 60 * 60 * 24;

        private readonly HashSet<DistrictCenter> _districtCenters = new HashSet<DistrictCenter>();
        private readonly IDistricts _districts;
        private readonly IGameTime _time;
        private readonly IGoods _goods;

        internal MetricsCollector(IDistricts districts, IGameTime time, IGoods goods)
        {
            _districts = districts;
            _time = time;
            _goods = goods;
        }

        internal Data Collect()
        {
            var time = CollectTime();
            var dcPops = _districts.AllCurrentPopsByDistrict();
            var globalPops = GlobalPopsFrom(dcPops);
            var dcStocks = _goods.AllCurrentGoodsByDistrict();
            var globalStocks = GlobalStocksFrom(dcStocks);

            return new Data(time, globalPops, dcPops, globalStocks, dcStocks);
        }

        private GameTime CollectTime()
        {
            var secondsSinceGameStart = (_time.PartialDayNumber - 1) * _secondsPerDay;
            var currentGameTime = _gameStartDate.AddSeconds(secondsSinceGameStart);

            return new GameTime(DateTime.Now, currentGameTime, _time.Cycle, _time.CycleDay, _time.DayNumber, _time.DayProgress);
        }

        private Pops GlobalPopsFrom(IDictionary<string, Pops> dcPops)
        {
            var values = dcPops.Values;

            return new Pops(values.Sum(p => p.Adults), values.Sum(p => p.Children));
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