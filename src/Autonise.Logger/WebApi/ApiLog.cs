using System;

namespace Autonise.Logger.WebApi
{
    public class ApiLog
    {
        public virtual long Id { get; set; }
        public virtual string IPAddress { get; set; }
        public virtual string CorrelationId { get; set; }
        public virtual string RequestMethod { get; set; }
        public virtual string Response { get; set; }
        public virtual DateTime RequestTime { get; set; }
        public virtual string ApplicationName { get; set; }
        public virtual string RequestURI { get; set; }
        public virtual string RequestBody { get; set; }
        public virtual string Header { get; set; }

    }
}