using System.Collections.Generic;

namespace VeVantZeData.Collector.GameAdapters
{
    interface IDistricts
    {
        IDictionary<string, Pops> AllCurrentPopsByDistrict();
    }
}