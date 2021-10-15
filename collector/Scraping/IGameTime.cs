namespace VeVantZeData.Collector.Scraping
{
    interface IGameTime
    {
        float PartialDayNumber { get; }
        int Cycle { get; }
        int CycleDay { get; }
        int DayNumber { get; }
        float DayProgress { get; }
    }
}