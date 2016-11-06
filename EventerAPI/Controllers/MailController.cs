using EventerAPI.Handlers;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace EventerAPI.Controllers
{
    class MailController : ApiController
    {
        private ResponseHandler rh = new ResponseHandler();
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage SendEmail(string to, string subject, string body)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", "key-5ec3b92f291420550b4c5aa87b124557"); // change to Eventer-api-domain
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "skideash.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "dev@moveo.co.il");
            request.AddParameter("to", to);
            request.AddParameter("subject", subject);
            request.AddParameter("text", body);
            request.Method = Method.POST;

            IRestResponse response = client.Execute(request);

            return rh.HandleResponse(response);
        }
    }
}
