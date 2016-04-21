using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autonise.Logger.NLog;
using NLog;

namespace Autonise.Logger.WebApi
{

    public abstract class MessageHandler : DelegatingHandler
    {
        static ILogger _logger;
        protected MessageHandler()
        {
            FileConfig.Configure();
            _logger = LogManager.GetCurrentClassLogger();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string corrId = string.Format("{0}{1}", DateTime.Now.Ticks, Thread.CurrentThread.ManagedThreadId);
            var requestInfo = request.RequestUri.ToString();
            string requestMethod = request.Method.Method;
            string currentIpAddress = request.GetClientIpAddress();
            var reqHeaders = "";
            DateTime currentDateTime = DateTime.Now;
            string requestBody = "";
            try
            {

                var header = request.Headers;
                if (header != null)
                {
                    var headers = header.ToArray();
                    for (int i = 0; i < headers.Count(); i++)
                        reqHeaders += string.Format("{0}:{1}, ", headers[i].Key, headers[i].Value.FirstOrDefault());
                }

                byte[] requestBodyByte = null;
                if (request.Content != null)
                    requestBodyByte = await request.Content.ReadAsByteArrayAsync();

                requestBody = requestBodyByte == null ? "" : Encoding.UTF8.GetString(requestBodyByte);
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException ?? ex).Message);
            }
            var response = await base.SendAsync(request, cancellationToken);
            try
            {
                int httpStatusCode = (int)response.StatusCode;
                byte[] responseMessageByte = null;
                if (response.Content != null)
                    responseMessageByte = await response.Content.ReadAsByteArrayAsync();
                string responseMessage = responseMessageByte == null ? "" : Encoding.UTF8.GetString(responseMessageByte);

                byte[] reasonMessageByte = Encoding.UTF8.GetBytes(response.ReasonPhrase);
                string reasonMessage = Encoding.UTF8.GetString(reasonMessageByte);

                DateTime responseTime = DateTime.Now;
                double timeDiff = responseTime.Subtract(currentDateTime).TotalSeconds;

                await
                    OutgoingMessageAsync(corrId, responseTime, timeDiff, responseMessage, httpStatusCode, reasonMessage,
                    currentIpAddress, currentDateTime, requestInfo, requestMethod, reqHeaders, requestBody);

            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException ?? ex).Message);
            }
            return response;

        }

        //private static Task<HttpResponseMessage> SendError(string error, HttpStatusCode code)
        //{
        //    var response = new HttpResponseMessage
        //    {
        //        Content = new StringContent(error),
        //        StatusCode = code
        //    };
        //    return Task<HttpResponseMessage>.Factory.StartNew(() => response);
        //}

        //protected abstract Task IncommingMessageAsync(string clientIp, DateTime requestTime, string correlationId, string requestUri, string requestMethod,
        //    string header, string requestBody);
        protected abstract Task OutgoingMessageAsync(string correlationId, DateTime responseTime, double timeDiff, string responseMessage,
            int statusCode, string statusDescription, string clientIp, DateTime requestTime, string requestUri, string requestMethod,
            string header, string requestBody);
    }

}