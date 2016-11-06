using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using EventerAPI.Handlers;
using EventerAPI.General;
using System.IO;

namespace EventerAPI.Controllers
{
    public class UploadController : ApiController
    {
        private ResponseHandler rh = new ResponseHandler();
        private AmazonHandler ah = new AmazonHandler();
        private FFMpegHandler ffh = new FFMpegHandler();

        public async Task<HttpResponseMessage> UploadVideo()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = "C:\\Temp\\";
            var provider = new CustomMultipartFormDataStreamProvider(root);
           
            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (string key in provider.FormData.AllKeys) {
                    Logger.Write(key + ":" + provider.FormData[key]);
                }


                string file_name = provider.FormData.AllKeys.Contains("file_name") ? provider.FormData["file_name"].ToString() : " ";
                string text_1 = provider.FormData.AllKeys.Contains("text_1") ? provider.FormData["text_1"].ToString() : " ";
                string user_name = provider.FormData.AllKeys.Contains("user_name") ? provider.FormData["user_name"].ToString() : " ";
                string headline = provider.FormData.AllKeys.Contains("headline") ? provider.FormData["headline"].ToString() : " ";
                string profile_pic = provider.FormData.AllKeys.Contains("profile_pic") ? provider.FormData["profile_pic"].ToString() : " ";
                int duration = provider.FormData.AllKeys.Contains("duration") ? int.Parse(provider.FormData["duration"].ToString()) : 0;

                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);

                    ffh.Convert(Path.GetFileNameWithoutExtension(file.LocalFileName), text_1, " ", user_name, headline, profile_pic, duration);

                    //ffh.Convert(Path.GetFileNameWithoutExtension(file.LocalFileName), "text 1", "name 1 ", "headline ", "http://www.partypacks.co.uk/images/products/product_93556_1_orig.jpg");

                    string new_file_name = ah.PutFile(@"C:\VidOut\" + Path.GetFileName(file.LocalFileName), file_name);

                    return rh.HandleResponse(new_file_name);
                }

                

                return rh.HandleResponse();
            }
            catch (System.Exception e)
            {
                Logger.Write(e.Message);
                Logger.Write(e.StackTrace);
                return rh.HandleError(e.Message);
            }
        }

        // We implement MultipartFormDataStreamProvider to override the filename of File which
        // will be stored on server, or else the default name will be of the format like Body-
        // Part_{GUID}. In the following implementation we simply get the FileName from 
        // ContentDisposition Header of the Request Body.
        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }
    }
}
