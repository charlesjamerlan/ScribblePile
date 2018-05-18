using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace scribble
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
            "ScribbleAdd",
            "scribble/add",
            new { controller = "Scribble", action = "Add"}
            );

            routes.MapRoute(
                "Scribble",
                "scribble/{id_scribble}/{action}",
                new { controller = "Scribble", action = "Get", id_scribble = (string)null }
                );

            routes.MapRoute(
                "User",
                "user/{id_user}/{action}",
                new { controller = "User", action = "Get", id_scribble = (string)null }
                );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}