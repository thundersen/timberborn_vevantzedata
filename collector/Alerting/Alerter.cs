
using System.Collections.Generic;

namespace VeVantZeData.Collector.Alerting
{
    class Alerter : IAlerter
    {
        private readonly List<IAlerter> _alerters;

        internal static IAlerter Create(VeVantZeDataConfig config, Playthrough playthrough)
        {
            var alerts = new Alerter(new List<IAlerter>(){new InfluxDBAlerter(config, playthrough)});
            alerts.Init();
            return alerts;
        }

        private Alerter(List<IAlerter> alerters)
        {
            _alerters = alerters;
        }

        public void Init()
        {
            _alerters.ForEach(a => a.Init());
        }

        public void CleanUp()
        {
            _alerters.ForEach(a => a.CleanUp());
        }

        public void UpdateAlertStatus()
        {
            _alerters.ForEach(a => a.UpdateAlertStatus());
        }
    }
}