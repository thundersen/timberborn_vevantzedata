using System.Collections.Generic;

namespace VeVantZeData.Collector.Scraping
{
    interface IDistricts
    {
        IDictionary<string, Pops> AllCurrentPopsByDistrict();
    }
}