using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.ResourceCountingSystem;
using VeVantZeData.Collector.Scraping;

namespace VeVantZeData.Collector.GameAdapters
{
    class GoodsAdapter : IGoods
    {
        private readonly DistrictsAdapter _districts;
        private readonly IEnumerable<GoodSpecification> _goodSpecs;
        private readonly Func<ResourceCountingService> _resourceCountingService;

        internal GoodsAdapter(DistrictsAdapter districts,  IEnumerable<GoodSpecification> goodSpecs, Func<ResourceCountingService> resourceCountingService)
        {
            _districts = districts;
            _goodSpecs = goodSpecs;
            _resourceCountingService = resourceCountingService;
        }

        public IDictionary<string, Goods> AllCurrentGoodsByDistrict()
        {
            var countingService = _resourceCountingService.Invoke();

            // ResourceCountingService is a singleton used by the game for displaying the good amounts of the currently selected district in the ui.
            // So it's important to reset it after we abused it.
            var previousDC = countingService.GetInstanceField<ResourceCountingService, DistrictCenter>("_districtCenter");

            var result = new Dictionary<string, Goods>();

            foreach (var dc in _districts.DistrictCenters)
            {
                countingService.SwitchDistrict(dc);

                var goodCounts = _goodSpecs.ToDictionary(s => s.Id, s => countingService.GetDistrictAmount(s));

                result.Add(dc.DistrictName, new Goods(goodCounts));
            }

            countingService.SwitchDistrict(previousDC);

            return result;
        }
    }
}