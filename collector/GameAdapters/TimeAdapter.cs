using Timberborn.TimeSystem;
using Timberborn.WeatherSystem;
using VeVantZeData.Collector.Scraping;

namespace VeVantZeData.Collector.GameAdapters
{
    class TimeAdapter : IGameTime
    {
        private readonly WeatherService _weatherService;
        private readonly DayNightCycle _dayNightCycle;

        public TimeAdapter(WeatherService weatherService, DayNightCycle dayNightCycle)
        {
            _weatherService = weatherService;
            _dayNightCycle = dayNightCycle;
        }

        public float PartialDayNumber => _dayNightCycle.PartialDayNumber;
        public int Cycle =>  _weatherService.Cycle;
        public int CycleDay => _weatherService.CycleDay;
        public int DayNumber => _dayNightCycle.DayNumber;
        public float DayProgress => _dayNightCycle.DayProgress;
    }
}