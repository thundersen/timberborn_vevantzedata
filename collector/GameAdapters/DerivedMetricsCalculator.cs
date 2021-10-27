namespace VeVantZeData.Collector.Scraping
{
    class DerivedMetricsCalculator : IDerivedMetricsCalculator
    {
        public DaysOfStocks CalculateDaysOfStocks()
        {
            return new DaysOfStocks(0, 0);
        }
    }
}