namespace VeVantZeData.Collector.Alerting
{
    interface IAlerter
    {
        void Init();
        void CleanUp();
        void UpdateAlertStatus();
    }
}