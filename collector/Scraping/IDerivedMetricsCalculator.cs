using System.Collections.Generic;

namespace VeVantZeData.Collector.Scraping
{
    interface IDerivedMetricsCalculator
    {
        IDictionary<string, DaysOfStocks> CalculateDaysOfStocks(IDictionary<string, Goods> goods, IDictionary<string, Pops> pops);
    }
}