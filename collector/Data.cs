using System;
using System.Collections.Generic;

namespace VeVantZeData.Collector {
    record Data(Pops GlobalPops, IDictionary<String, Pops> DistrictPops);

    record Pops(int Adults, int Children) {
        internal readonly int Total = Adults + Children;
    }
}