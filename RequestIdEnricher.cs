using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Serilog.Core;
using Serilog.Events;
using System.IO;
using Newtonsoft.Json.Linq;

namespace RestService.Enrichers
{
    public class RequestIdEnricher : ILogEventEnricher
    {
        public const string HttpRequestIdPropertyName = "HttpRequestId";

        static readonly string RequestIdItemName = typeof(RequestIdEnricher).Name + "+RequestId";

        private string requestIdParameterName = "RequestId";

        //Request parameter which contains Request Id is called RequestId
        public RequestIdEnricher () { }

        //Set a name of the request parameter which contains Request Id (optional if the parameter has a name "RequestId")
        public RequestIdEnricher (string requestIdParameterName)
        {
            this.requestIdParameterName = requestIdParameterName;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null) throw new ArgumentNullException("logEvent");

            if (HttpContext.Current == null)
                return;

            if (HttpContext.Current.Request == null)
                return;

            Guid requestId;

            //Request has a JSON object which has Request Id
            string ajaxRequestData = "";
            if (HttpContext.Current.Request.ContentType.Contains("application/json"))
            {
                string jsonString = "";
                HttpContext.Current.Request.InputStream.Position = 0;
                using (StreamReader inputStream = new StreamReader(HttpContext.Current.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();

                    if (jsonString.Contains(requestIdParameterName)) {
                        JToken token = JObject.Parse(jsonString);
                        ajaxRequestData = (string)token.SelectToken(requestIdParameterName);
                    }
                }
            }

            //Request has Request Id as its parameter
            string parametricRequest = HttpContext.Current.Request.Params[requestIdParameterName];

            //Request which has Request Id will be logged with its RequestId
            var requestIdItem = HttpContext.Current.Items[RequestIdItemName];
            Guid guidResult;

            if (requestIdItem == null)
                if (!String.IsNullOrEmpty(ajaxRequestData) && Guid.TryParse(ajaxRequestData, out guidResult))
                    HttpContext.Current.Items[RequestIdItemName] = requestId = new Guid(ajaxRequestData);
                else if (!String.IsNullOrEmpty(parametricRequest) && Guid.TryParse(ajaxRequestData, out guidResult))
                    HttpContext.Current.Items[RequestIdItemName] = requestId = new Guid(parametricRequest);
                else
                    HttpContext.Current.Items[RequestIdItemName] = requestId =  Guid.NewGuid();
            else
                requestId = (Guid)requestIdItem;

            LogEventProperty requestIdProperty = new LogEventProperty(HttpRequestIdPropertyName, new ScalarValue(requestId));
            logEvent.AddPropertyIfAbsent(requestIdProperty);
        }
    }
}