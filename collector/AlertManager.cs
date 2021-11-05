using VeVantZeData.Collector.Alerting;

namespace VeVantZeData.Collector
{
    class AlertManager
    {
        private readonly VeVantZeDataConfig _config;
        private IAlerter _alerts;

        public AlertManager(VeVantZeDataConfig config)
        {
            TimberbornGame.AddGameStartCallback(OnGameStart);
            _config = config;
        }


        internal void Update()
        {
        }

        private void OnGameStart()
        {
            _alerts = Alerter.Create(_config, TimberbornGame.Playthrough);
        }
    }
}