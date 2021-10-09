using System;
using System.Collections.Generic;

namespace VeVantZeData.Collector {
    record Data(GameTime GameTime, Pops GlobalPops, IDictionary<String, Pops> DistrictPops);

    record Pops(int Adults, int Children) {
        internal readonly int Total = Adults + Children;
    }

    record GameTime(DateTime SystemTimeStamp, DateTime GameTimeStamp, int Cycle, int CycleDay, int TotalDay, float DayProgress);
}