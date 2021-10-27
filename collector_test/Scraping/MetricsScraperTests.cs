using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace VeVantZeData.Collector.Scraping
{
    [TestFixture]
    public class MetricsScraperTests
    {
        private IGoods _defaultgoods = Mock.Of<IGoods>(g => g.AllCurrentGoodsByDistrict() == new Dictionary<string, Goods>() { { "a", new Goods(new Dictionary<string, int>()) } });
        private IGameTime _defaultGameTime = Mock.Of<IGameTime>();
        private IDerivedMetricsCalculator _derivedMetrics = Mock.Of<IDerivedMetricsCalculator>();

        [OneTimeSetUp]
        public void BeforeAll()
        {
            MetricsScraper.Log = new Mock<ILog>().Object;
        }

        [TestCase("1,1", "1,1")]
        [TestCase("1,1 2,1", "3,2")]
        [TestCase("99,199 101,1801 100,1000", "300,3000")]
        public void CountsGlobalPopsFromDistricts(string pops, string expected)
        {
            var districtPops = pops.Split(" ").Select(Pops).ToArray();
            var sut = SutWith(DistrictsWith(districtPops));
            Assert.That(sut.Scrape().GlobalPops, Is.EqualTo(Pops(expected)));
        }

        [Test]
        public void CountsZeroGlobalPopsWhenThereAreNoDistricts()
        {
            var sut = SutWith(DistrictsWith());
            Assert.That(sut.Scrape().GlobalPops, Is.EqualTo(new Pops(0, 0)));
        }

        [Test]
        public void AddsDerivedMetrics()
        {
            var expected = new Dictionary<string, DaysOfStocks>();
            var derivedMetrics = Mock.Of<IDerivedMetricsCalculator>(g => g.CalculateDaysOfStocks(
                It.IsAny<IDictionary<string, Goods>>(), 
                It.IsAny<IDictionary<string, Pops>>()) 
                == expected);
            var sut = SutWith(derivedMetrics);
            Assert.That(sut.Scrape().DistrictDaysOfStocks, Is.SameAs(expected));
        }

        private Pops Pops(string p)
        {
            var split = p.Split(",");
            return new Pops(int.Parse(split[0]), int.Parse(split[1]));
        }

        private MetricsScraper SutWith(IDistricts districts)
        {
            return new MetricsScraper(districts, _defaultGameTime, _defaultgoods, _derivedMetrics);
        }

        private MetricsScraper SutWith(IDerivedMetricsCalculator derivedMetrics)
        {
            return new MetricsScraper(DistrictsWith(), _defaultGameTime, _defaultgoods, derivedMetrics);
        }

        private IDistricts DistrictsWith(params Pops[] pops)
        {
            return Mock.Of<IDistricts>(d => d.AllCurrentPopsByDistrict() == pops.ToDictionary(p => "d_" + p.GetHashCode(), p => p));
        }
    }
}
