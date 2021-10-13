using System.Text;

namespace VeVantZeData.Collector.Output
{
    class LogWriter : IMetricsOutput
    {
        public void Write(Data data)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"collected metrics for game day {data.GameTime.GameTimeStamp.ToString("o")} (cycle/day {data.GameTime.Cycle}/{data.GameTime.CycleDay})");
            sb.AppendLine($"  global: {data.GlobalPops.ToString()}");
            sb.AppendLine($"  got pops for districts: {string.Join(", ", data.DistrictPops.Keys)}");
            sb.AppendLine($"  got global stock for {data.GlobalStock.Counts.Count} goods");
            sb.AppendLine($"  got stocks for districts: {string.Join(", ", data.DistrictStocks.Keys)}");

            Plugin.Log.LogInfo(sb.ToString());
        }
    }
}