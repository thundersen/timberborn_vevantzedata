using System;
using System.Collections.Generic;

namespace VeVantZeData.Collector {
    public record Data(
        GameTime GameTime, 
        Pops GlobalPops, 
        IDictionary<String, Pops> DistrictPops, 
        Goods GlobalStock, 
        IDictionary<String, Goods> DistrictStocks,
        DaysOfStocks DaysOfStocks);

    public record Pops(int Adults, int Children) {
        internal readonly int Total = Adults + Children;
    }

    public record GameTime(DateTime SystemTimeStamp, DateTime GameTimeStamp, int Cycle, int CycleDay, int TotalDay, float DayProgress);

    public record Goods(IDictionary<string, int> Counts);

    public record DaysOfStocks(float Water, float Food);
}