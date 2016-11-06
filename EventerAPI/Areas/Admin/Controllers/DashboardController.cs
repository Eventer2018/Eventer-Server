using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EventerAPI.Areas.Admin.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private eventerEntities de = new eventerEntities();
        
        //
        // GET: /Admin/Dashboard/

        public ActionResult Index()
        {
            ViewData["ActiveUsers"]  = de.users.Count();
            
            return View();
        }

        public ActionResult Users()
        {
            var data = de.users.OrderByDescending(u => u.user_id).ToList<user>();
            return View(data);
        }

        public ActionResult UserInfo(long id)
        {            
            var _tmp_user = de.users.FirstOrDefault(u => u.user_id == id);
            ViewData["UserInfo"] = _tmp_user;
            return View();
        }

        public ActionResult Notifications()
        {
            var data = de.notifications.OrderByDescending(u => u.notification_id).ToList<notification>();
            return View(data);
        }

        public ActionResult SendNotification()
        {
            return View();
        }

        public ActionResult SendNotification_Add(notification n)
        {
            if (string.IsNullOrEmpty(n.text))
            {
                ModelState.AddModelError("Text", "Please Write Some Text");
                return View("SendNotification");

            }
            else 
            {
                de.notifications.Add(n);
                de.SaveChanges();

                List<user> _users = de.users.Where(u => u.device_token_ios != null || u.device_token_android != null).ToList();

                EventerAPI.PushNotifications.Handlers.PushGCMNotification(_users.Where(i => i.device_token_android != null).Select(i => i.device_token_android).ToArray<string>(), n.text, null);
                EventerAPI.PushNotifications.Handlers.PushAPNSNotification(_users.Where(i => i.device_token_ios != null).Select(i => i.device_token_ios).ToArray<string>(), n.text, null);

                return RedirectToAction("Notifications");
            }
        }
    }
}
