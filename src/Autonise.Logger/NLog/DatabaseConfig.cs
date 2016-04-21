using NLog;
using NLog.Config;

namespace Autonise.Logger.NLog
{
    public class DatabaseConfig
    {
        public static void Configure()
        {
            var config = Configuration.DatabaseLogConfiguration(new LoggingConfiguration());
            LogManager.Configuration = config;
        } 
    }
}