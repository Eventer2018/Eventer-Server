using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EventerAPI.Handlers
{
    public class ResponseHandler
    {
        const string ok_response_format = "{{ \"status\": \"ok\", \"body\" : {0}}}";
        const string err_response_format = "{{ \"status\": \"error\", \"body\" : {0}}}";

        public HttpResponseMessage HandleResponse()
        {
            return HandleResponse(null);
        }

        public HttpResponseMessage HandleResponse(Object response)
        {
            var res = new HttpResponseMessage(HttpStatusCode.OK);
            res.Content = new StringContent(string.Format(ok_response_format, Serialize(response)), System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        public HttpResponseMessage HandleError(string error_description)
        {
            return HandleError(HttpStatusCode.OK, error_description);
        }

        public HttpResponseMessage HandleError(HttpStatusCode status_code, string error_description)
        {
            var res = new HttpResponseMessage(status_code);
            res.Content = new StringContent(string.Format(err_response_format, Serialize(error_description)), System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        public string Serialize(Object _o)
        {
            return JsonConvert.SerializeObject(_o, Formatting.None, new JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateFormatString = "dd-MM-yyyy HH:mm:ss zzz" });
        }
    }
}