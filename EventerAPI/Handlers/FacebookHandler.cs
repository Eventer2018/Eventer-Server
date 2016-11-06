using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebook;
using System.Dynamic;

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

        public void PostVideoToWall(string token, string video_url, string thumbnail_url)
        {
            var client = new FacebookClient(token);
            try
            {
                dynamic parameters = new ExpandoObject();
                //       parameters.method = "video.upload";
                parameters.description = "Video upload using Api Eventer";
                //   parameters.source = mediaObject;
                parameters.title = "video bala";
                parameters.link = video_url;
                parameters.picture = thumbnail_url;
                client.Post("/me/feed", parameters);
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
        }

    }
}