using System.Collections.Generic;
using System.Linq;

namespace VeVantZeData.Collector.Scraping
{
    class DerivedMetricsCalculator : IDerivedMetricsCalculator
    {
        // assuming english goods names. TODO: use translated good names
        internal const string Water = "Water";
        internal const string Berries = "Berries";
        internal const string Carrots = "Carrot";
        internal const string GrilledPotatoes = "GrilledPotato";
        internal const string Bread = "Bread";

        // assuming constants for normal and hard difficulty. TODO: determine values at runtime
        private const int _waterPerDay = 2;
        private const float _foodPerDay = 2.4f;

        public IDictionary<string, DaysOfStocks> CalculateDaysOfStocks(IDictionary<string, Goods> goods, IDictionary<string, Pops> pops)
        {
            return goods.Keys.ToDictionary(d => d, d => CalculateFor(goods[d], pops[d]));
        }

        private static DaysOfStocks CalculateFor(Goods goods, Pops pops)
        {
            var daysOfWater = TotalCountFor(goods.Counts, Water) / pops.Total / _waterPerDay;

            var totalFood = TotalCountFor(goods.Counts, Berries, Carrots, GrilledPotatoes, Bread);

            var daysOfFood = totalFood / pops.Total / _foodPerDay;

            return new DaysOfStocks(daysOfWater, daysOfFood);
        }

        private static float TotalCountFor(IDictionary<string, int> counts, params string[] goods)
        {
            return goods.Select(g => CountFor(counts, g)).Sum();
        }

        // returns 0, if good can't be found. that will the case for non-english language settings
        private static int CountFor(IDictionary<string, int> counts, string good)
        {
            return counts.ContainsKey(good) ? counts[good] : 0;
        }
    }
}