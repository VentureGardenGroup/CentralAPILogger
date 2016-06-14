using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web.Configuration;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using StackifyLib.nLog;

namespace Autonise.Logger.NLog
{
    public class Configuration
    {
        public static LoggingConfiguration FileLogConfiguration(LoggingConfiguration config, LogLevel level = null)
        {
            LoggingConfiguration logconfig = config;
            var logPath = WebConfigurationManager.AppSettings["logPath"] ?? "C:/CentralLogs"; //@"${basedir}/logs";

            var fileTarget = new FileTarget()
            {
                FileName = string.Format("{0}/Applications/${{appsetting:name=AppName:default=Unknown\\: set AppName in appSettings}}_${{date:format=yyyy-MMM-dd}}.log", logPath),
                //"${basedir}/logs/${appsetting:name=AppName:default=Unknown\\: set AppName in appSettings}_${date:format=yyyy-MMM-dd}.log",
                Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} ${logger} ${callsite} ${level} ${message}",
                ArchiveDateFormat = "yyyyMMddHHmm",
                ArchiveFileName = string.Format("{0}/Archives/Applications (fileLogs)/${{appsetting:name=AppName:default=Unknown\\: set AppName in appSettings}}_${{date:format=yyyy-MMM-dd}}.{{#}}.zip", logPath),
                //ArchiveAboveSize = 80240,
                EnableArchiveFileCompression = true,
                ArchiveEvery = FileArchivePeriod.Month,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                ConcurrentWrites = true,
                KeepFileOpen = false,
                Encoding = Encoding.UTF8,
                Name = "FileLogger"
            };

            logconfig.LoggingRules.Add(new LoggingRule("*", level ?? LogLevel.Info, fileTarget));
            return logconfig;
        }

        public static LoggingConfiguration StackifyConfiguration(LoggingConfiguration config, LogLevel level = null)
        {
            LoggingConfiguration logconfig = config;
            var target = new StackifyTarget
            {
                Layout = "${longdate} ${logger} ${callsite} ${level} ${message}",
                logMethodNames = true,
                logAllParams = true,
                apiKey = "4Rg1Yv3Kr3To3Lq3Tf9Yj3Qw7Ka7Re9Is6Bb8Hh"
            };

            logconfig.LoggingRules.Add(new LoggingRule("*", level ?? LogLevel.Info, target));
            return logconfig;
        }


        public static LoggingConfiguration CsvLogConfiguration(LoggingConfiguration config, LogLevel level = null)
        {
            LoggingConfiguration logconfig = config;
            var logPath = WebConfigurationManager.AppSettings["logPath"] ?? @"C:/CentralLogs";

            var fileTarget = new FileTarget
            {

                //FileName = //"CSVLogger",
                //"${basedir}/logs/${appsetting:name=AppName:default=Unknown\\: set AppName in appSettings}_${date:format=yyyy-MMM-dd}.csv",
                FileName = string.Format("{0}/APIs/${{appsetting:name=AppName:default=Unknown\\: set AppName in appSettings}}_${{date:format=yyyy-MMM-dd}}.csv", logPath),
                //ArchiveOldFileOnStartup = true,
                Layout = new CsvLayout
                {
                    Columns = {
                        new CsvColumn("LogTime", "${date:format=yyyy-MM-dd HH\\:mm\\:ss}"),
                        new CsvColumn("Level", "${level}"),
                        new CsvColumn("ApplicationName", "${appsetting:name=AppName:default=Unknown\\: set AppName in appSettings}"),
                        //new CsvColumn("CallSite", "${callsite}"), 
                        //new CsvColumn("Logger", "${logger}"), 
                        new CsvColumn("ServerHost", "${event-context:item=ServerHost}"), 
                        new CsvColumn("ServerIP", "${event-context:item=ServerIP}"), 
                        new CsvColumn("Message", "${message}"), 
                        new CsvColumn("CorrelationId", "${event-context:item=CorrelationId}"), 
                        new CsvColumn("RequestIP", "${event-context:item=IpAddress}"), 
                        new CsvColumn("RequestURI", "${event-context:item=RequestURI}"), 
                        new CsvColumn("RequestBody", "${event-context:item=RequestBody}"), 
                        new CsvColumn("FullRequestBody", "${event-context:item=Request}"), 
                        new CsvColumn("RequestMethod", "${event-context:item=RequestMethod}"), 
                        new CsvColumn("RequestTime", "${event-context:item=RequestTime}"), 
                        new CsvColumn("ResponseTime", "${event-context:item=ResponseTime}"),
                        new CsvColumn("Response", "${event-context:item=Response}"),
                        new CsvColumn("FullResponse", "${event-context:item=FullResponse}"),
                        new CsvColumn("StatusCode", "${event-context:item=StatusCode}"), 
                        new CsvColumn("StatusText", "${event-context:item=StatusText}"), 
                        new CsvColumn("TimeDiff", "${event-context:item=TimeDiff}"),
                        new CsvColumn("Headers", "${event-context:item=Headers}"), 

                    },
                    Delimiter = CsvColumnDelimiterMode.Comma
                },

                ArchiveDateFormat = "yyyyMMddHHmm",
                ArchiveFileName = string.Format("{0}/Archives/APIs (csvLogs)/${{appsetting:name=AppName:default=Unknown\\: set AppName in appSettings}}_${{date:format=yyyy-MMM-dd}}.{{#}}.zip", logPath),
                //ArchiveAboveSize = 80240,
                ArchiveEvery = FileArchivePeriod.Month,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                EnableArchiveFileCompression = true,
                ConcurrentWrites = true,
                KeepFileOpen = false,
                Encoding = Encoding.UTF8,
                Name = "CSVLogger"
            };

            logconfig.LoggingRules.Add(new LoggingRule("*", level ?? LogLevel.Info, fileTarget));

            return logconfig;
        }

        public static LoggingConfiguration DatabaseLogConfiguration(LoggingConfiguration config, LogLevel level = null)
        {
            LoggingConfiguration logconfig = config;

            var target = new DatabaseTarget
            {
                ConnectionStringName = "autoniseLogger",
                DBProvider = "System.Data.SqlClient",
                CommandType = CommandType.Text,
                Parameters =
                {
                    new DatabaseParameterInfo("@application",
                        "${appsetting:name=AppName:default=Unknown\\: set AppName in appSettings}"),
                        new DatabaseParameterInfo("@logged", "${date}"),
                        new DatabaseParameterInfo("@level", "${level}"),
                        new DatabaseParameterInfo("@message", "${message}"),
                        new DatabaseParameterInfo("@username", "${identity}"),
                        //new DatabaseParameterInfo("@serverName", "${aspnet-request:serverVariable=SERVER_NAME}"),
                        //new DatabaseParameterInfo("@port", "${aspnet-request:serverVariable=SERVER_PORT}"),
                        //new DatabaseParameterInfo("@url", "${aspnet-request:serverVariable=HTTP_URL}"),
                        //new DatabaseParameterInfo("@https",
                        //    "${when:inner=1:when='${aspnet-request:serverVariable=HTTPS}' == 'on'}${when:inner=0:when='${aspnet-       request:serverVariable=HTTPS}' != 'on'}"),
                        //new DatabaseParameterInfo("@serverAddress", "${aspnet-request:serverVariable=LOCAL_ADDR}"),
                        //new DatabaseParameterInfo("@remoteAddress",
                        //    "${aspnet-request:serverVariable=REMOTE_ADDR}:${aspnet-request:serverVariable=REMOTE_PORT}"),
                            new DatabaseParameterInfo("@serverName", ""),
                        new DatabaseParameterInfo("@port", ""),
                        new DatabaseParameterInfo("@url", ""),
                        new DatabaseParameterInfo("@https",
                            ""),
                        new DatabaseParameterInfo("@serverAddress", ""),
                        new DatabaseParameterInfo("@remoteAddress",
                            ""),
                        new DatabaseParameterInfo("@logger", "${logger}"),
                        new DatabaseParameterInfo("@callSite", "${callsite}"),
                        new DatabaseParameterInfo("@exception", "${exception:tostring}"),
                },

                CommandText = "insert into dbo.Log (Application, Logged, Level, Message,Username,ServerName," +
                              " Port, Url, Https,ServerAddress, RemoteAddress," +
                              "Logger, CallSite, Exception) " +
                              "values (@Application, @Logged, @Level, @Message,@Username,@ServerName, @Port, @Url," +
                              " @Https,@ServerAddress, @RemoteAddress," + "@Logger, @Callsite, @Exception);"
            };

            logconfig.LoggingRules.Add(new LoggingRule("*", level ?? LogLevel.Info, target));

            //if (Debugger.IsAttached)
            //{
            //    logconfig = ConsoleLogConfiguration(logconfig);
            //}
            return logconfig;
        }





        private static LoggingConfiguration ConsoleLogConfiguration(LoggingConfiguration logconfig, LogLevel level = null)
        {
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget
             {
                 Layout = "${level}|${logger}|${message}${onexception:${newline}${exception:format=tostring}}"
             };

            logconfig.LoggingRules.Add(new LoggingRule("*", level ?? LogLevel.Info, consoleTarget));

            return logconfig;
        }


        //======================Database Schema Script==============================================================================  
        //SET ANSI_NULLS ON
        //SET QUOTED_IDENTIFIER ON
        //CREATE TABLE [dbo].[Log] (
        //    [Id] [int] IDENTITY(1,1) NOT NULL,
        //    [Application] [nvarchar](50) NOT NULL,
        //    [Logged] [datetime] NOT NULL,
        //    [Level] [nvarchar](50) NOT NULL,
        //    [Message] [nvarchar](max) NOT NULL,
        //    [UserName] [nvarchar](250) NULL,
        //    [ServerName] [nvarchar](max) NULL,
        //    [Port] [nvarchar](max) NULL,
        //    [Url] [nvarchar](max) NULL,
        //    [Https] [bit] NULL,
        //    [ServerAddress] [nvarchar](100) NULL,
        //    [RemoteAddress] [nvarchar](100) NULL,
        //    [Logger] [nvarchar](250) NULL,
        //    [Callsite] [nvarchar](max) NULL,
        //    [Exception] [nvarchar](max) NULL,
        //  CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED ([Id] ASC)
        //    WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
        //) ON [PRIMARY]


    }
}