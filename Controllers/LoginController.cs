using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using scribble.Models;
using scribble.Common;

namespace scribble.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            if (Utilities.IsAuthenticated())
            {
                if ((Request.QueryString["f"] == "sso") && (Request.QueryString["success"] == "true"))
                {
                    Response.Write("<script>window.close();</script>");
                }
                else
                {
                    return Redirect(Request.QueryString["r"]);
                }                
            }
            return View("login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            return Redirect("/login");
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            string username = form["username"];
            string password = form["password"];
            string redirect = Request.QueryString["r"];
            string from = Request.QueryString["f"];

            if (string.IsNullOrEmpty(redirect))
            {
                redirect = "/scribble/1";
            }

            int id_user = Models.User.Login(username, password);
            if (id_user > 0)
            {
                FormsAuthentication.SetAuthCookie(id_user.ToString(), false);
                //determine redirect
                if (from == "sso")
                {
                    redirect = "/login?success=true&f=sso";
                }
                return Redirect(redirect);
            }
            else
            {
                return Redirect("/login?success=false&f="+from+"&r="+redirect);
            }
        }

    }
}
