using BepInEx.Logging;

namespace VeVantZeData.Collector
{
    public interface ILog
    {
        void Log(LogLevel level, object data);
        void Debug(object data);
        void Error(object data);
        void Fatal(object data);
        void Info(object data);
        void Message(object data);
        void Warning(object data);
    }
}