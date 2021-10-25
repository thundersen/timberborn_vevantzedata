using Timberborn.SingletonSystem;

namespace VeVantZeData.Collector.Output
{
    class EventPublisher : IMetricsOutput
    {
        private readonly EventBus _eventBus;

        public EventPublisher(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Write(Data data)
        {
            _eventBus.Post(new DataCapturedEvent(data));
        }
    }
}