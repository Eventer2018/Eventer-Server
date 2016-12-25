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
    public class UserController : ApiController
    {
        private ResponseHandler rh = new ResponseHandler();
        private FacebookHandler fh = new FacebookHandler();


        class SaveUserFriendsThread
        {
            public user _user;
            public FacebookHandler _facebook_handler;

            public void GetAndSave()
            {
                using (eventerEntities de = new eventerEntities())
                {
                    HashSet<user_friends_heap> user_likes = _facebook_handler.GetUserFriendsParsed(_user.access_token, _user.user_id);

                    de.user_friends_heap.AddRange(user_likes);
                    de.SaveChanges();

                    de.add_user_friends(_user.user_id);
                }
            }
        }


        public class user_find_params {
            public string user_name { get; set; }
        
        }

        public class video_event_result : video_event {
            public string date_interval { get; set; }
        }


        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Add(user _user)
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {
                    if (_user.email != null && de.users.Any(e => e.email == _user.email))
                    {
                        return rh.HandleError(HttpStatusCode.Ambiguous, "Email Already Exists");
                    }

                    if (_user.sn_id != null && de.users.Any(e => e.sn_id == _user.sn_id))
                    {
                        return rh.HandleError(HttpStatusCode.Ambiguous, "S.N ID Already Exists");
                    }

                    //Add user
                    _user.status = 1;
                    de.users.Add(_user);
                    de.SaveChanges();

                    return rh.HandleResponse(new { user_id = _user.user_id });
                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }
        
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Login(user _user)
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {
                    
                    user logged_user;

                    if (_user.user_id > 0) {
                        logged_user = de.users.Where(u => u.user_id == _user.user_id).FirstOrDefault();

                        if (logged_user != null)
                        {
                            _user.user_id = logged_user.user_id;
                            logged_user = EntityUpdate.EntityUpdateNotNull<user>(_user, logged_user);
                            de.SaveChanges();

                            return rh.HandleResponse(logged_user);
                        }
                        else {
                            return rh.HandleError("User Not Found");
                        
                        }
                    }

                    if (_user.sn_id != null)
                    {
                        logged_user = de.users.Where(u => u.sn_id == _user.sn_id).FirstOrDefault();

                        if (logged_user != null)
                        {
                            _user.user_id = logged_user.user_id;
                            logged_user = EntityUpdate.EntityUpdateNotNull<user>(_user, logged_user);
                            de.SaveChanges();

                            DataFromFacebook(logged_user);
                            return rh.HandleResponse(logged_user);    
                        }

                    }
                    
                    de.users.Add(_user);
                    de.SaveChanges();

                    logged_user = _user;

                    DataFromFacebook(logged_user);
                    return rh.HandleResponse(logged_user);


                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Get(user _user)
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {
                    user _tmp_user = de.users.FirstOrDefault(u => u.user_id == _user.user_id);
                    return rh.HandleResponse(_tmp_user);
                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }


        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Find(user_find_params _user)
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {
                    List<user> _tmp_user;
                    if (!string.IsNullOrEmpty(_user.user_name))
                    {
                        _tmp_user = de.users.Where(u => (u.first_name.Contains(_user.user_name) || u.last_name.Contains(_user.user_name)) && u.status != 9).OrderByDescending(u => u.user_id).ToList();

                    }
                    else {
                        _tmp_user = de.users.Where(u => u.status != 9).OrderByDescending(u => u.user_id).ToList();
                    }
                        
                    return rh.HandleResponse(_tmp_user);
                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }


        [System.Web.Http.HttpPost]
        public HttpResponseMessage Edit(user _user)
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {
                    Logger.Write(string.Format("User Details: id: {0} name: {1} lname: {2} ", _user.user_id, _user.first_name, _user.last_name));
                    user edited_user = de.users.Find(_user.user_id);

                    if (edited_user != null)
                    {
                        de.Entry(edited_user).CurrentValues.SetValues(EntityUpdate.EntityUpdateNotNull<user>(_user, edited_user));
                        edited_user.last_name = _user.last_name;
                        de.SaveChanges();
                    }
                    return rh.HandleResponse(new { user_id = _user.user_id });
                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Events(user _user)
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {
                    MySqlParameter[] search_params = new MySqlParameter[] { new MySqlParameter("user_id", _user.user_id) };
                    var videos = ((IObjectContextAdapter)de).ObjectContext.ExecuteStoreQuery<video_event_result>("CALL sp_user_events(@user_id)", search_params).ToList<video_event_result>();

                    return rh.HandleResponse(videos);
                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage FacebookFriends(user _user)
        {
            try
            {
                using (eventerEntities de = new eventerEntities())
                {

                    MySqlParameter[] search_params = new MySqlParameter[] { new MySqlParameter("user_id", _user.user_id) };
                    var users = ((IObjectContextAdapter)de).ObjectContext.ExecuteStoreQuery<user>("CALL user_facebook_friends(@user_id)", search_params).ToList<user>();

                    return rh.HandleResponse(users);
                }
            }
            catch (Exception e)
            {
                return rh.HandleError(e.Message);
            }
        }




        private void DataFromFacebook(user _user) {
            //ASYNC - Get and save user like pages from facebook

            SaveUserFriendsThread _sult = new SaveUserFriendsThread();
            _sult._facebook_handler = fh;
            _sult._user = _user;


            //_sult.GetAndSave();
            Thread _lt = new Thread(_sult.GetAndSave);
            _lt.Start();
        
        }





    }
}