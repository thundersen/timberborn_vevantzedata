using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace VeVantZeData.Collector.Scraping
{
    [TestFixture]
    public class DerivedMetricsCalculatorTests
    {
        private DerivedMetricsCalculator _sut = new DerivedMetricsCalculator();

        [Test]
        public void ReturnsEmptyWithNoDistricts()
        {
            var result = _sut.CalculateDaysOfStocks(new Dictionary<string, Goods>(), new Dictionary<string, Pops>());
            Assert.That(result, Is.Empty);
        }

        //this is important to ensure that it doesn't throw when the game is run with another language than english
        [Test]
        public void ReturnsZeroesIfItCantFindGoodsInDistrict()
        {
            var goods = new Dictionary<string, Goods>(){
                {"d1", new Goods(new Dictionary<string, int>(){{"Wasser", 1}})}
            };
            var pops = new Dictionary<string, Pops>(){
                {"d1", new Pops(1, 0)}
            };

            CollectionAssert.AreEquivalent(
                _sut.CalculateDaysOfStocks(goods, pops),
                new Dictionary<string, DaysOfStocks>() { { "d1", new DaysOfStocks(0, 0) } });
        }

        [TestCase("d1:0,0,0,0,0", "d1:10", "d1:0,0")]
        [TestCase("d1:20,0,0,0,0", "d1:10", "d1:1,0")]
        [TestCase("d1:10,0,0,0,0", "d1:10", "d1:0.5,0")]
        [TestCase("d1:200,0,0,0,0", "d1:10", "d1:10,0")]
        [TestCase("d1:10,0,0,0,0 d2:20,0,0,0,0", "d1:10 d2:10", "d1:0.5,0 d2:1,0")]
        public void CalculatesDaysOfWaterAssuming2WaterPerBeaverAndDay(string goodsString, string popsString, string expectedString)
        {
            var goods = GoodsFrom(goodsString);
            var pops = PopsFrom(popsString);
            var expected = ExpectedFrom(expectedString);
            CollectionAssert.AreEquivalent(_sut.CalculateDaysOfStocks(goods, pops), expected);
        }

        [TestCase("d1:0,0,0,0,0", "d1:10", "d1:0,0")]
        [TestCase("d1:0,24,0,0,0", "d1:10", "d1:0,1")]
        [TestCase("d1:0,6,6,6,6", "d1:10", "d1:0,1")]
        [TestCase("d1:0,6,6,0,0", "d1:10", "d1:0,0.5")]
        [TestCase("d1:0,60,60,60,60", "d1:10", "d1:0,10")]
        [TestCase("d1:0,6,6,6,6 d2:0,120,120,240,0", "d1:10 d2:10", "d1:0,1 d2:0,20")]
        public void CalculatesDaysOFoodAssuming2Point4PerBeaverAndDay(string goodsString, string popsString, string expectedString)
        {
            var goods = GoodsFrom(goodsString);
            var pops = PopsFrom(popsString);
            var expected = ExpectedFrom(expectedString);
            CollectionAssert.AreEquivalent(_sut.CalculateDaysOfStocks(goods, pops), expected);
        }

        private static IDictionary<string, Goods> GoodsFrom(string goodsString)
        {
            var goods = new Dictionary<string, Goods>();
            foreach (var districtGoodsString in goodsString.Split(" "))
            {
                var goodsSplit = districtGoodsString.Split(":");
                var foodSplit = goodsSplit[1].Split(",");
                var districtGoods = new Dictionary<string, int>();
                districtGoods.Add(DerivedMetricsCalculator.Water, Int32.Parse(foodSplit[0]));
                districtGoods.Add(DerivedMetricsCalculator.Berries, Int32.Parse(foodSplit[1]));
                districtGoods.Add(DerivedMetricsCalculator.Carrots, Int32.Parse(foodSplit[2]));
                districtGoods.Add(DerivedMetricsCalculator.GrilledPotatoes, Int32.Parse(foodSplit[3]));
                districtGoods.Add(DerivedMetricsCalculator.Bread, Int32.Parse(foodSplit[4]));

                goods.Add(goodsSplit[0], new Goods(districtGoods));
            }
            return goods;
        }

        private static IDictionary<string, Pops> PopsFrom(string popsString)
        {
            var pops = new Dictionary<string, Pops>();
            foreach (var districtPopsString in popsString.Split(" "))
            {
                var popsSplit = districtPopsString.Split(":");
                pops.Add(popsSplit[0], new Pops(Int32.Parse(popsSplit[1]), 0));
            }
            return pops;
        }

        private static IDictionary<string, DaysOfStocks> ExpectedFrom(string expectedString)
        {
            var expected = new Dictionary<string, DaysOfStocks>();
            foreach (var districtExpected in expectedString.Split(" "))
            {
                var expectedSplit = districtExpected.Split(":");
                var waterAndFood = expectedSplit[1].Split(",");
                expected.Add(expectedSplit[0], new DaysOfStocks(float.Parse(waterAndFood[0]), float.Parse(waterAndFood[1])));
            }
            return expected;
        }
    }
}