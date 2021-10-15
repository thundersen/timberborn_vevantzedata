using System.Collections.Generic;

namespace VeVantZeData.Collector.Scraping
{
    interface IGoods
    {
        IDictionary<string, Goods> AllCurrentGoodsByDistrict();
    }
}