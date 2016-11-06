using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using EventerAPI.Handlers;
using EventerAPI.General;
using MySql.Data.MySqlClient;

namespace EventerAPI.Controllers
{
    public class EventController : ApiController
    {
        private ResponseHandler rh = new ResponseHandler();
        private FacebookHandler fh = new FacebookHandler();

        public class feed_result : video_event {
            public string date_interval { get; set; }
            public string user_name { get; set; }
            public string profile_pic_url { get; set; }
        }

        public class new_event : video_event {
            public bool share { get; set; }
        }


        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Add(new_event _event)
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {                    
                    _event.status = 1;

                    video_event _event_tmp = new video_event();
                    EntityUpdate.EntityUpdateNotNull<video_event>(_event, _event_tmp);

                    de.video_event.Add(_event_tmp);
                    de.SaveChanges();

                    user event_user = de.users.Where(u => u.user_id == _event.user_id).FirstOrDefault();

                    MySqlParameter[] search_params = new MySqlParameter[] { new MySqlParameter("user_id", _event.user_id) };
                    var users = ((IObjectContextAdapter)de).ObjectContext.ExecuteStoreQuery<user>("CALL user_facebook_friends(@user_id)", search_params).ToList<user>();

                    
                    string alert = string.Format("{0} has created a new event", event_user.first_name);
                    
                    foreach(var item in users){
                        if (item.device_token_ios != null) {
                            PushNotifications.Handlers.PushAPNSNotification(new string[] { item.device_token_ios }, alert, null);
                        };

                        if (item.device_token_android != null)
                        {
                            PushNotifications.Handlers.PushGCMNotification(new string[] { item.device_token_android }, alert, null);
                        };
                    }

                    if (_event.share) {
                        fh.PostVideoToWall(event_user.access_token, _event.movie_url, _event.thumbnail_url);
                    }

                    dynamic event_result = new 
                    {
                        event_id = _event.event_id,
                        user_id = _event.user_id,
                        movie_url = _event.movie_url,
                        thumbnail_url = _event.thumbnail_url,
                        date_created = _event.date_created,
                        date_interval = DateIntreval(_event.date_created),
                        first_name = event_user.first_name,
                        profile_pic_url = event_user.profile_pic_url
                    };

                    return rh.HandleResponse(event_result);
                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Edit(video_event _event)
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {
                    video_event edited_event = de.video_event.Find(_event.event_id);

                    if (edited_event != null)
                    {
                        de.Entry(edited_event).CurrentValues.SetValues(EntityUpdate.EntityUpdateNotNull<video_event>(_event, edited_event));
                        de.SaveChanges();
                    }

                    user event_user = de.users.Where(u => u.user_id == _event.user_id).FirstOrDefault();
                    dynamic event_result = new
                    {
                        event_id = edited_event.event_id,
                        user_id = edited_event.user_id,
                        movie_url = edited_event.movie_url,
                        thumbnail_url = edited_event.thumbnail_url,
                        date_created = edited_event.date_created,
                        date_interval = DateIntreval(edited_event.date_created),
                        first_name = event_user.first_name,
                        profile_pic_url = event_user.profile_pic_url
                    };

                    return rh.HandleResponse(event_result);
                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Delete(video_event _event)
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {
                    video_event edited_event = de.video_event.Find(_event.event_id);

                    if (edited_event != null)
                    {
                        edited_event.status = 9;
                        de.SaveChanges();
                    }
                    return rh.HandleResponse();
                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Feed()
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {

                    MySqlParameter[] search_params = new MySqlParameter[] {};

                    var videos = ((IObjectContextAdapter)de).ObjectContext.ExecuteStoreQuery<feed_result>("CALL sp_feed()", search_params).ToList<feed_result>();


                    return rh.HandleResponse(videos);
                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }

        public static string DateIntreval(DateTime? date)
        {
            if (date == null) return "";

            double delta = DateTime.Now.Subtract(date ?? DateTime.Now).TotalSeconds;

            if (delta < 60) return "now";
            if (delta < 120) return "1m";
            if (delta < 60 * 60) return Math.Floor(delta / 60) + "m";
            if (delta < 60 * 60 * 2) return "1h";
            if (delta < 60 * 60 * 24) return Math.Floor(delta / (60 * 60)) + "h";
            if (delta < 60 * 60 * 24 * 2) return "1d";
            if (delta < 60 * 60 * 24 * 30) return Math.Floor(delta / (60 * 60 * 24)) + "d";
            if (delta < 60 * 60 * 24 * 30 * 2) return "1M";
            if (delta < 60 * 60 * 24 * 365) return Math.Floor(delta / (60 * 60 * 24 * 30)) + "M";
            if (delta < 60 * 60 * 24 * 365 * 2) return "1y";
            return Math.Floor(delta / (60 * 60 * 24 * 365)) + "y";
        }

    }
}
