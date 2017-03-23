using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebook;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Configuration;

namespace EventerAPI.Handlers
{
    public class FacebookHandler
    {
        private FacebookClient fc = new FacebookClient();

        public class FBLocation
        {
            public long id { get; set; }
            public string name { get; set; }
            public float? longitude { get; set; }
            public float? latitude { get; set; }
        }

        //http://graph.facebook.com/1228068311/picture?width=150&height=150

        public dynamic GetUserWithToken(string token)
        {
            FacebookClient fc = new FacebookClient(token);

            dynamic user = fc.Get("me", new { });


            return user;
        }

        public dynamic GraphData(string token, string graph_path, object parameters)
        {
            dynamic fb_response = null;
            try
            {
                fc.AccessToken = token;
                fb_response = fc.Get(graph_path, parameters);
            }
            catch (Exception e) { }
            return fb_response;
        }

        public FBLocation GetLocation(string token)
        {
            FBLocation fb_location = new FBLocation();

            dynamic user_place = GraphData(token, "me", new { fields = "location" });
            try
            {
                if (user_place != null)
                {
                    if (user_place[0].name == null)
                    {
                        return fb_location;
                    }

                    fb_location.name = user_place[0].name;
                    fb_location.id = long.Parse(user_place[0].id);

                    dynamic location = GraphData(token, user_place[0].id, new { fields = "location" });
                    fb_location.latitude = (float)location.location.latitude;
                    fb_location.longitude = (float)location.location.longitude;
                }
            }
            catch (Exception e) { }
            return fb_location;
        }

        public HashSet<user_friends_heap> GetUserFriendsParsed(string token, long user_id)
        {
            HashSet<user_friends_heap> _result = new HashSet<user_friends_heap>();
            string fb_path = "me/friends";

            while (fb_path != string.Empty)
            {
                dynamic fb_response = GraphData(token, fb_path, new { });

                fb_path = string.Empty;
                if (fb_response != null && fb_response.data != null)
                {
                    foreach (var i in fb_response.data)
                    {
                        _result.Add(new user_friends_heap { sn_id = i.id, sn_name = i.name, user_id = user_id });
                    }

                    if (fb_response.paging != null)
                    {
                        fb_path = fb_response.paging.next;
                    }
                }
            }
            return _result;
        }

        public void PostVideoToWall(string token,string video_title, string video_url, string thumbnail_url, bool post_as_file)
        {
            var client = new FacebookClient(token);
            try
            {
                dynamic parameters = new ExpandoObject();
                //       parameters.method = "video.upload";
                //parameters.description = video_title;//"Video upload using Api Eventer";
                //   parameters.source = mediaObject;
                parameters.title = video_title;
                parameters.picture = thumbnail_url;

                if (post_as_file)
                {
                    string download_path = ConfigurationManager.AppSettings["DownloadPath"];
                    string file_name = Path.GetFileName(video_url);
                    string mime_type = MimeMapping.GetMimeMapping(file_name);

                    using (WebClient wc = new WebClient()) {
                        wc.DownloadFile(video_url, download_path + file_name);

                    }

                    var media_object = new FacebookMediaObject { ContentType = mime_type, FileName = file_name }.SetValue(File.ReadAllBytes(download_path + file_name));
                    parameters.source = media_object;
                    client.Post("/me/videos", parameters);

                    File.Delete(download_path + file_name);
                }
                else
                {
                    parameters.link = video_url;
                    client.Post("/me/feed", parameters);
                }
                
                //client.Post("/me/feed", parameters);
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
        }

    }
}