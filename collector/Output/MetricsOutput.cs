using System.Collections.Generic;
using Timberborn.SingletonSystem;

namespace VeVantZeData.Collector.Output
{
    class MetricsOutput : IMetricsOutput
    {
        private List<IMetricsOutput> _writers;

        internal static IMetricsOutput Create(VeVantZeDataConfig config, Playthrough playthrough, EventBus eventBus)
        {
            var writers = new List<IMetricsOutput>();

            if (config.CsvWriterEnabled)
                writers.Add(new CsvWriter(playthrough));

            if (config.LogWriterEnabled)
                writers.Add(new LogWriter());

            if (config.InfluxDBEnabled)
                writers.Add(new InfluxDBWriter(config, playthrough));

            if (config.EventPublisherEnabled)
                writers.Add(new EventPublisher(eventBus));
            
            return new MetricsOutput(writers);
        }

        private MetricsOutput(List<IMetricsOutput> writers)
        {
            _writers = writers;
        }

        public void Write(Data data)
        {
            _writers.ForEach(w => w.Write(data));
        }
    }
}