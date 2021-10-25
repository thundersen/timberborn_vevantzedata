using UnityEngine;
using VeVantZeData.Collector.GameAdapters;
using VeVantZeData.Collector.Output;
using VeVantZeData.Collector.Scraping;

namespace VeVantZeData.Collector
{
    class Collector
    {
        private const float intervalSeconds = 10.0f;
        private float secondsSinceLastInterval;
        private TimeAdapter _timeAdapter;
        private GoodsAdapter _goodsAdapter;
        private static DistrictsAdapter _districtsAdapter;
        private IMetricsOutput _output;
        private MetricsScraper _scraper;
        private VeVantZeDataConfig _config;

        internal Collector(VeVantZeDataConfig config)
        {
            _config = config;
            TimberbornGame.AddGameStartCallback(OnGameStart);
        }

        internal void Update()
        {
            secondsSinceLastInterval += Time.deltaTime;
            if (secondsSinceLastInterval > intervalSeconds)
            {
                CollectIfInitialized();
                secondsSinceLastInterval = 0.0f;
            }
        }

        void CollectIfInitialized()
        {
            if (!TimberbornGame.IsRunning())
                return;

            var data = _scraper.Scrape();
            _output.Write(data);
        }

        private void OnGameStart()
        {
            EntityListener.Init(TimberbornGame.EventBus);
            _districtsAdapter = new DistrictsAdapter();
            DistrictCenterListener.DistrictsAdapter = _districtsAdapter;
            _timeAdapter = new TimeAdapter(TimberbornGame.WeatherService, TimberbornGame.DayNightCycle);
            _goodsAdapter = new GoodsAdapter(_districtsAdapter, TimberbornGame.GoodSpecs, () => TimberbornGame.ResourceCountingService);
            _scraper = new MetricsScraper(_districtsAdapter, _timeAdapter, _goodsAdapter);

            _output = MetricsOutput.Create(_config, TimberbornGame.Playthrough, TimberbornGame.EventBus);

            Plugin.Log.LogDebug("Reset after game start.");
        }
    }
}