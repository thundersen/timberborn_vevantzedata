using System.Collections.Generic;

namespace VeVantZeData.Collector.Output
{
    class MetricsOutput : IMetricsOutput
    {
        private List<IMetricsOutput> _writers;

        internal static IMetricsOutput Create(VeVantZeDataConfig config, Playthrough playthrough)
        {
            var writers = new List<IMetricsOutput>() { new CsvWriter(playthrough) };

            if (config.InfluxDBEnabled)
                writers.Add(new InfluxDBWriter(config, playthrough));
            
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