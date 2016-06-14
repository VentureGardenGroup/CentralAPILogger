using NLog;
using NLog.Config;

namespace Autonise.Logger.NLog
{
    public class FileConfig
    {
        public static LoggingConfiguration Configure(LoggingConfiguration logConfig = null, LogLevel level = null)
        {

            var config = Configuration.FileLogConfiguration(logConfig ?? new LoggingConfiguration(), level ?? LogLevel.Info);
            LogManager.Configuration = config;
            return config;
        }

        public static LoggingConfiguration CSVConfigure(LoggingConfiguration logConfig = null, LogLevel level = null)
        {
            var config = Configuration.CsvLogConfiguration(logConfig ?? new LoggingConfiguration(), level ?? LogLevel.Info);
            LogManager.Configuration = config;
            return config;
        }
    }
}