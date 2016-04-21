using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Autonise.Logger.NLog;
using NLog;

namespace Autonise.Logger.WebApi
{
    public class AutoniseApiLogHandler : MessageHandler
    {
        static ILogger _logger;


        public AutoniseApiLogHandler()
        {
            FileConfig.CSVConfigure();
            _logger = LogManager.GetCurrentClassLogger();
        }


        //protected override async  Task IncommingMessageAsync(string clientIp, DateTime requestTime, string correlationId, string requestUri, string requestMethod,
        //    string header, string requestBody)
        //{
        //    await Task.Run(() => { });
        //}


        protected override async Task OutgoingMessageAsync(string correlationId, DateTime responseTime, double timeDiff, string responseMessage,
            int statusCode, string statusDescription, string clientIp, DateTime requestTime, string requestUri, string requestMethod,
            string header, string requestBody)
        {
            await Task.Run(() =>
                Save(correlationId, responseMessage, statusCode, statusDescription, responseTime,
                timeDiff, clientIp, requestTime, requestUri, requestMethod, header, requestBody));
        }


        protected void Save(string correlationId, string response, int statusCode, string statusText, DateTime responseTime,
            double timeDiff, string clientIp, DateTime requestTime, string requestUri, string requestMethod, string header, string requestBody)
        {
            try
            {
                var res = response.Replace("\n", "").Replace("\r", "");
                var req = requestBody.Replace("\n", "").Replace("\r", "");
                var serverIp = ServerIp(ServerHost);
                LogEventInfo eventInfo = new LogEventInfo(LogLevel.Info, "CSVLogger", "Autonise API Logging was successful");
                eventInfo.Properties["StatusCode"] = statusCode;
                eventInfo.Properties["Response"] = (res.Length > 20) ? res.Substring(0, 20) : res;
                eventInfo.Properties["FullResponse"] = res;
                eventInfo.Properties["StatusText"] = statusText.Replace("\n", "").Replace("\r", "");
                eventInfo.Properties["CorrelationId"] = correlationId;
                eventInfo.Properties["ResponseTime"] = responseTime;
                eventInfo.Properties["TimeDiff"] = timeDiff;
                eventInfo.Properties["ServerHost"] = ServerHost.HostName ?? "N/A";
                eventInfo.Properties["ServerIP"] = serverIp;

                eventInfo.Properties["IpAddress"] = clientIp == "::1" ? serverIp : clientIp;
                eventInfo.Properties["RequestURI"] = requestUri.Replace("\n", "").Replace("\r", "");
                eventInfo.Properties["Request"] = req.Length > 20 ? req.Substring(0, 20) : req;
                eventInfo.Properties["RequestBody"] = req;
                eventInfo.Properties["RequestMethod"] = requestMethod.Replace("\n", "").Replace("\r", "");
                eventInfo.Properties["Headers"] = header.Replace("\n", "").Replace("\r", "");
                eventInfo.Properties["ServerHost"] = ServerHost.HostName ?? "N/A";
                eventInfo.Properties["ServerIP"] = serverIp;
                eventInfo.Properties["RequestTime"] = requestTime;
                _logger.Log(eventInfo);
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException ?? ex).Message);
            }
        }

        private static IPHostEntry ServerHost
        {
            get
            {
                IPHostEntry serverHost = Dns.GetHostEntry(Dns.GetHostName());
                return serverHost;
            }
        }

        private static string ServerIp(IPHostEntry serverHost)
        {
            IPHostEntry ipHostInfo = serverHost;
            foreach (IPAddress address in ipHostInfo.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                    return address.ToString();
            }
            return string.Empty;
        }
    }

}