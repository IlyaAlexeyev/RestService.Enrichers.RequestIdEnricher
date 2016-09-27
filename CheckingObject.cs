using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace RestService
{
    public class CheckingObject : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            String message = "";
            CheckingObject check = new CheckingObject();

            if (!check.IsInputObjectCorrect())
            {
                message = "Sorry! Requested object is incorrect!";
            }
            else if (!check.IsValueOfFieldCorrect("UserId", "Guid"))
            {
                message = "Sorry! UserId is incorrect!";
            }
            else if (!check.IsValueOfFieldCorrect("RequestId", "Guid"))
            {
                message = "Sorry! RequestId is incorrect!";
            }
            else if (!check.IsValueOfFieldCorrect("AdvertisingOptIn", "bool"))
            {
                message = "Sorry! AdvertisingOptIn is incorrect!";
            }
            else if (!check.IsValueOfFieldCorrect("DateModified", "DateTime"))
            {
                message = "Error! ISO country code does not exist!";
            }

            if (String.IsNullOrEmpty(message)) 
                actionContext.Response = actionContext.Request.CreateResponse(message);
        }

        public bool IsInputObjectCorrect()
        {
            bool isCorrect = true;

            //HttpContext httpContext = Request.Properties["MS_HttpContext"] as HttpContext;

            HttpContext.Current.Request.InputStream.Position = 0;
            using (StreamReader inputStream = new StreamReader(HttpContext.Current.Request.InputStream))
            {
                string jsonString = inputStream.ReadToEnd();

                if (!jsonString.Contains("UserId") || !jsonString.Contains("RequestId") || !jsonString.Contains("AdvertisingOptIn") || !jsonString.Contains("CountryIsoCode") || !jsonString.Contains("DateModified") || !jsonString.Contains("Locale"))
                {
                    isCorrect = false;
                }
            }
            return isCorrect;
        }

        public bool IsValueOfFieldCorrect(string fieldName, string datatype)
        {
            bool isCorrect = false;

            HttpContext.Current.Request.InputStream.Position = 0;
            using (StreamReader inputStream = new StreamReader(HttpContext.Current.Request.InputStream))
            {
                string jsonString = inputStream.ReadToEnd();
                throw new Exception(jsonString);
                if (jsonString.Contains(fieldName))
                {
                    throw new Exception();
                    JToken token = JObject.Parse(jsonString);
                    string ajaxRequestData = (string)token.SelectToken(fieldName);

                    if (datatype == "Guid")
                    {
                        Guid guidResult;
                        if (Guid.TryParse(ajaxRequestData, out guidResult)) isCorrect = true;
                    }
                    if (datatype == "bool")
                    {
                        bool boolResult;
                        if (Boolean.TryParse(ajaxRequestData, out boolResult)) isCorrect = true;
                    }
                    if (datatype == "DateTime")
                    {
                        DateTime dateTimeResult;
                        if (DateTime.TryParse(ajaxRequestData, out dateTimeResult)) isCorrect = true;
                    }
                }
            }
            return isCorrect;
        }
    }
}
 