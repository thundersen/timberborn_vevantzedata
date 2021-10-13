using BepInEx.Logging;

namespace VeVantZeData.Collector
{
    internal class LogWrapper : ILog
    {
        private ManualLogSource _log;

        public static LogWrapper Default()
        {
            return new LogWrapper(Plugin.Log);
        }

        public LogWrapper(ManualLogSource log)
        {
            _log = log;
        }

        public void Debug(object data)
        {
            _log.LogDebug(data);
        }

        public void Error(object data)
        {
            _log.LogError(data);
        }

        public void Fatal(object data)
        {
            _log.LogFatal(data);
        }

        public void Info(object data)
        {
            _log.LogInfo(data);
        }

        public void Message(object data)
        {
            _log.LogMessage(data);
        }

        public void Warning(object data)
        {
            _log.LogWarning(data);
        }

        void ILog.Log(LogLevel level, object data)
        {
            _log.Log(level, data);
        }
    }
}