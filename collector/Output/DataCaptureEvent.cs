namespace VeVantZeData.Collector.Output
{
    public class DataCapturedEvent
    {
        public DataCapturedEvent(Data data)
        {
            Data = data;
        }

        public Data Data { get; }
    }
}