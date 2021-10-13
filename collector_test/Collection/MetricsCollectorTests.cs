using Moq;
using NUnit.Framework;
using Timberborn.Characters;
using VeVantZeData.Collector.GameAdapters;

namespace VeVantZeData.Collector.Collection
{
    [TestFixture]
    public class MetricsCollectorTests
    {
        private MetricsCollector _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new MetricsCollector(
                new Mock<IDistricts>().Object, 
                new GlobalPopulation(null), 
                new Mock<IGameTime>().Object,
                new Mock<IGoods>().Object);
        }

        [Test]
        public void CanConstructMetricsCollector()
        {
            Assert.IsNotNull(_sut);
        }


        // global pops
    }
}
