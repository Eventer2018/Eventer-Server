using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EventerAPI.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Admin/Login/

        public ActionResult Index()
        {
            return View("Login");
        }

        public ActionResult Login(admin _u)
        {
            if (string.IsNullOrEmpty(_u.user_name))
            {
                ModelState.AddModelError("User", "Please Provide User Name");
            }

            if (string.IsNullOrEmpty(_u.password))
            {
                ModelState.AddModelError("Password", "Please Provide Password");
            }

            using (eventerEntities de = new eventerEntities())
            {
                admin _tmp_user = de.admins.FirstOrDefault(u => u.user_name == _u.user_name);

                if (_tmp_user == null)
                {
                    ModelState.AddModelError("User", "User Not Found");
                }
                else if (_tmp_user.password != _u.password)
                {
                    ModelState.AddModelError("Password", "Wrong Password");
                }

            }

            if (ModelState.IsValid)
            {
                FormsAuthentication.SetAuthCookie(_u.user_name.ToString(),true);
                return RedirectToAction("Index", "Dashboard");
            }
            else 
            {
                return View("Login");
            }
        }

        public ActionResult Logout() {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

    }
}
