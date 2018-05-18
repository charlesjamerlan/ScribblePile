using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using scribble.Models;
using scribble.Common;
using System.IO;
using System.Web.Script.Serialization;

namespace scribble.Controllers
{
    public class ScribbleController : Controller
    {
        public class ScribbleControllerModel
        {
            public Scribble scribble { get; set; }
            public User user_loggedin { get; set; }
            public string disqus_sso_payload { get; set; }
            public bool is_owner { get; set; }
            public bool is_liked { get; set; }

            public ScribbleControllerModel()
            {
                scribble = null;
                user_loggedin = null;
                disqus_sso_payload = string.Empty;
                is_owner = false;
            }
        }

        public ActionResult Liked(string id_scribble)
        {
            scribble.Models.Like.SaveLike(int.Parse(id_scribble), Utilities.GetLoggedInUserID());
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Unlike(string id_scribble)
        {
            scribble.Models.Like.RemoveLike(int.Parse(id_scribble), Utilities.GetLoggedInUserID());
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            //System.Drawing.Image Img = System.Drawing.Image.FromFile(Server.MapPath("~/Scribbles/bd142bad-ffae-4a58-8127-608d9d05a02c_Rogue_hodges.jpg"));            
            //Utilities.ScaleBySize(Img, 100).Save(Server.MapPath("~/Scribbles/scaled.jpg"));
            //System.Drawing.Image Img2 = System.Drawing.Image.FromFile(Server.MapPath("~/Scribbles/scaled.jpg"));
            //Utilities.CropImage(Img2, new System.Drawing.Rectangle(0, 0, 100, 100)).Save(Server.MapPath("~/Scribbles/cropped.jpg"));
            ////CropImage(ScaleBySize(Img,100), new Rectangle(0, 0, 100, 100))

            return View();
        }

        public ActionResult AddVersion(string id_scribble)
        {            
            Scribble s = Scribble.Get(id_scribble);
            return View("addversion",null, s);
        }

        [HttpPost]
        public ActionResult AddVersion(string id_scribble, FormCollection form)
        {
            string title = form["title"];
            string description = form["description"];

            if (int.Parse(id_scribble) > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var fileName = guid + "_" + Path.GetFileName(file.FileName);
                    //Common.Utilities.ResizeImage(file.InputStream, fileName);

                    var path = Path.Combine(Server.MapPath("~/Scribbles/"), fileName);
                    file.SaveAs(path);

                    //resize image for thumbnails
                    Utilities.ResizeImage(fileName, Server.MapPath("~/Scribbles/"));

                    Models.Version version = new Models.Version();
                    version.description = description;
                    version.filename = fileName;
                    version.scribble_id = int.Parse(id_scribble);
                    version.Add();
                }
            }

            return Redirect("/scribble/" + id_scribble.ToString());
        }

        [HttpPost]
        public ActionResult Add(FormCollection form)
        {
            string title = form["title"];
            string description = form["description"];

            Scribble scribble = new Scribble();
            scribble.title = title;
            scribble.description = description;
            scribble.id_author = Utilities.GetLoggedInUserID();

            int id_scribble = scribble.Add();
            if (id_scribble > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var fileName = guid + "_" + Path.GetFileName(file.FileName);

                    //MemoryStream destination = new MemoryStream();
                    //file.InputStream.CopyTo(destination);
                    //Common.Utilities.ResizeImage(destination, fileName);
                    
                    var path = Path.Combine(Server.MapPath("~/Scribbles/"), fileName);
                    file.SaveAs(path);
                    
                    //resize image for thumbnails
                    Utilities.ResizeImage(fileName, Server.MapPath("~/Scribbles/"));

                    Models.Version version = new Models.Version();
                    version.description = description;
                    version.filename = fileName;
                    version.scribble_id = id_scribble;
                    version.Add();
                }
            }

            return Redirect("/scribble/" + id_scribble.ToString());
            
        }

        public ActionResult Get(string id_scribble)
        {
            //Track View 
            View v = new View();
            v.id_scribble = int.Parse(id_scribble);
            v.ip = Request.UserHostAddress;
            v.useragent = Request.UserAgent;
            v.sessionid = Session.SessionID;
            v.Save();

            ScribbleControllerModel scm = new ScribbleControllerModel();
            Scribble s = Scribble.Get(id_scribble);
            scm.scribble = s;

            //authenticate if logged in
            User u = Utilities.GetLoggedInUser();
            scm.user_loggedin = new User();
            if (u.id > 0)
            {
                scm.user_loggedin = u;
                scm.is_owner = (u.id == s.author.id) ? true : false;
                scm.is_liked = Like.IsLiked(s.id, u.id);
                var obj = new
                {
                    id = u.id,
                    username = u.username,
                    email = u.email
                };
                scm.disqus_sso_payload = Utilities.GenerateDisqusSSOPayload(obj);
            }
            return View("scribble",scm);
        }

    }
}
