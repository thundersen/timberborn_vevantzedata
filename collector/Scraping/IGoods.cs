using System.Collections.Generic;

namespace VeVantZeData.Collector.Collection
{
    interface IGoods
    {
        IDictionary<string, Goods> AllCurrentGoodsByDistrict();
    }
}