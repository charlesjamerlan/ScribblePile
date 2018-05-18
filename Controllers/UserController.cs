using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using scribble.Models;

namespace scribble.Controllers
{
    public class UserController : Controller
    {
        public class UserControllerModel
        {
            public User user { get; set; }
            public IList<Scribble> scribbles { get; set; }

            public UserControllerModel()
            {
                user = null;
                scribbles = null;
            }
        }

        public ActionResult Get(int id_user)
        {
            UserControllerModel ucm = new UserControllerModel();
            ucm.user = scribble.Models.User.Get(id_user);
            ucm.scribbles = scribble.Models.Scribble.GetScribblesByUser(id_user);

            return View("profile", ucm);
        }

        public ActionResult Edit(int id_user)
        {
            User u = scribble.Models.User.Get(id_user);
            return View("edit", u);
        }

        [HttpPost]
        public ActionResult Edit(int id_user, FormCollection form)
        {
            string firstname = form["firstname"];
            string lastname = form["lastname"];
            string email = form["email"];
            string biography = form["biography"];

            if (id_user > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var fileName = guid + "_" + Path.GetFileName(file.FileName);
                    fileName = id_user.ToString() + ".jpg";
                    var path = Path.Combine(Server.MapPath("~/Users/"), fileName);
                    file.SaveAs(path);

                }
                Models.User user = Models.User.Get(id_user);
                user.firstname = firstname;
                user.lastname = lastname;
                user.email = email;
                user.biography = biography;
                user.UpdateProfile();

            }

            return Redirect("/user/" + id_user.ToString());
        }
    }
}
