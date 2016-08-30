using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BitBookMVCApp.Models;
using Microsoft.AspNet.Identity;

namespace BitBookMVCApp.Controllers
{
    public class CoverPhotoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /CoverPhoto/
        public ActionResult Index()
        {
            return View(db.CoverPhotoes.ToList());
        }

        // GET: /CoverPhoto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoverPhoto coverphoto = db.CoverPhotoes.Find(id);
            if (coverphoto == null)
            {
                return HttpNotFound();
            }
            return View(coverphoto);
        }

        // GET: /CoverPhoto/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: /CoverPhoto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file)
        {


            return View();
        }

        // GET: /CoverPhoto/Edit/5
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file)
        {
            CoverPhoto coverphoto = new CoverPhoto();

            string path = null;

            if (file != null && file.ContentLength > 0)
                try
                {
                    path = Path.Combine(Server.MapPath("~/Images/CoverPic"),
                                              Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            coverphoto.UserId = User.Identity.GetUserId();

            if (file != null && file.ContentLength > 0)
                try
                {
                    coverphoto.CoverImage = "~/Images/CoverPic/" + Path.GetFileName(file.FileName);

                    string UserId = User.Identity.GetUserId();
                    var coverphotoFind = db.CoverPhotoes.Where(s => s.UserId.Equals(UserId)).ToList();

                    if (coverphotoFind.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            db.CoverPhotoes.Add(coverphoto);
                            db.SaveChanges();

                            var friendIdLst = new List<string>();
                            var userIds = User.Identity.GetUserId();
                            var friendIdQry = db.Friends.Where(d => d.UserId == userIds && d.Status == "True").Select(d => d.FriendId);
                            friendIdLst.AddRange(friendIdQry);

                            var friendIdLst2 = new List<string>();
                            //var userIds2 = User.Identity.GetUserId();
                            var friendIdQry2 = db.Friends.Where(d => d.FriendId == userIds && d.Status == "True").Select(d => d.UserId);
                            friendIdLst2.AddRange(friendIdQry2);

                            var friendList = new List<string>();

                            foreach (var VARIABLE in friendIdLst)
                            {
                                friendList.Add(VARIABLE);
                            }

                            foreach (var v in friendIdLst2)
                            {
                                friendList.Add(v);
                            }

                            var name1 = (from u in db.Users
                                         where u.Id == userIds
                                         select new
                                         {
                                             u.Name
                                         }).ToList();

                            string name = "";
                            foreach (var VARIABLE in name1)
                            {
                                name = VARIABLE.Name;
                            }

                            //foreach (var f in friendList)
                            //{
                            //    ContentNotification cn = new ContentNotification();
                            //    cn.UserId = User.Identity.GetUserId();
                            //    cn.FriendId = f;
                            //    cn.Content = coverphoto.CoverImage = "~/Images/CoverPic/" + Path.GetFileName(file.FileName);
                            //    cn.RequestDateTime = DateTime.Now;

                            //    db.ContentNotifications.Add(cn);
                            //    db.SaveChanges();
                            //}


                            // profilePicture save as a Post in Post Table

                            Post aPost = new Post();
                            if (file != null && file.ContentLength > 0)
                                try
                                {
                                    path = Path.Combine(Server.MapPath("~/Images/PostPic"),
                                                              Path.GetFileName(file.FileName));
                                    file.SaveAs(path);
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                                }
                            if (file != null && file.ContentLength > 0)
                                try
                                {
                                    aPost.ImgPost = "~/Images/PostPic/" + Path.GetFileName(file.FileName);
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                                }

                            string userId = User.Identity.GetUserId();
                            var user = (from u in db.Users
                                        where u.Id == userId
                                        select new { u.Name }).ToList();
                            string userName = "";
                            foreach (var v in user)
                            {
                                userName = v.Name;
                            }

                            aPost.UserId = userId;
                            aPost.UserName = userName;
                            aPost.PostDate = DateTime.Now;
                            if (ModelState.IsValid)
                            {
                                db.Posts.Add(aPost);
                                db.SaveChanges();
                            }

                            var photoById = (from pid in db.Posts
                                             where pid.ImgPost == aPost.ImgPost && pid.UserId == userIds
                                             select new { pid.PostId }).ToList();

                            int photoPostId = 0;

                            foreach (var VARIABLE in photoById)
                            {
                                photoPostId = VARIABLE.PostId;
                            }

                            ContentNotification request = new ContentNotification();

                            foreach (var item in friendList)
                            {
                                request.FriendId = item;
                                request.UserName = name;
                                request.UserId = User.Identity.GetUserId();
                                request.PostId = photoPostId.ToString();
                                request.Notification = "Cover Photo Uploaded by " + request.UserName;
                                request.RequestDateTime = DateTime.Now;
                                db.ContentNotifications.Add(request);
                                db.SaveChanges();
                            }


                            // ---- end ---------- //

                            //return RedirectToAction("Index", "Profile");


                        }
                    }

                    else
                    {
                        //var findId = from s in db.CoverPhotoes
                        //   where s.UserId.fi == UserId
                        //   select s.CoverPhotoId;

                        var item = db.CoverPhotoes.FirstOrDefault(i => i.UserId == UserId);

                        coverphoto = db.CoverPhotoes.Find(item.CoverPhotoId);


                        if (ModelState.IsValid)
                        {

                            if (file != null && file.ContentLength > 0)
                                try
                                {
                                    coverphoto.CoverImage = "~/Images/CoverPic/" + Path.GetFileName(file.FileName);
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                                }

                            db.Entry(coverphoto).State = EntityState.Modified;
                            db.SaveChanges();


                            var friendIdLst = new List<string>();
                            var userIds = User.Identity.GetUserId();
                            var friendIdQry = db.Friends.Where(d => d.UserId == userIds).Select(d => d.FriendId);
                            friendIdLst.AddRange(friendIdQry);

                            var friendIdLst2 = new List<string>();
                            var userIds2 = User.Identity.GetUserId();
                            var friendIdQry2 = db.Friends.Where(d => d.FriendId == userIds2).Select(d => d.FriendId);
                            friendIdLst2.AddRange(friendIdQry2);

                            var friendList = new List<string>();

                            foreach (var VARIABLE in friendIdLst)
                            {
                                friendList.Add(VARIABLE);
                            }

                            foreach (var v in friendIdLst2)
                            {
                                friendList.Add(v);
                            }

                            //foreach (var f in friendList)
                            //{
                            //    ContentNotification cn = new ContentNotification();
                            //    cn.UserId = User.Identity.GetUserId();
                            //    cn.FriendId = f;
                            //    cn.Content = coverphoto.CoverImage = "~/Images/CoverPic/" + Path.GetFileName(file.FileName);
                            //    cn.RequestDateTime = DateTime.Now;

                            //    db.ContentNotifications.Add(cn);
                            //    db.SaveChanges();
                            //}

                            // profilePicture save as a Post in Post Table

                            Post aPost = new Post();
                            if (file != null && file.ContentLength > 0)
                                try
                                {
                                    path = Path.Combine(Server.MapPath("~/Images/PostPic"),
                                                              Path.GetFileName(file.FileName));
                                    file.SaveAs(path);
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                                }
                            if (file != null && file.ContentLength > 0)
                                try
                                {
                                    aPost.ImgPost = "~/Images/PostPic/" + Path.GetFileName(file.FileName);
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                                }

                            string userId = User.Identity.GetUserId();
                            var user = (from u in db.Users
                                        where u.Id == userId
                                        select new { u.Name }).ToList();
                            string userName = "";
                            foreach (var v in user)
                            {
                                userName = v.Name;
                            }

                            aPost.UserId = userId;
                            aPost.UserName = userName;
                            aPost.PostDate = DateTime.Now;
                            if (ModelState.IsValid)
                            {
                                db.Posts.Add(aPost);
                                db.SaveChanges();
                            }

                            var photoById = (from pid in db.Posts
                                             where pid.ImgPost == aPost.ImgPost && pid.UserId == userIds
                                             select new { pid.PostId }).ToList();

                            int photoPostId = 0;

                            foreach (var VARIABLE in photoById)
                            {
                                photoPostId = VARIABLE.PostId;
                            }

                            ContentNotification request = new ContentNotification();

                            foreach (var item1 in friendList)
                            {
                                request.FriendId = item1;
                                request.UserName = userName;
                                request.UserId = User.Identity.GetUserId();
                                request.PostId = photoPostId.ToString();
                                request.Notification = "Cover Photo Uploaded by " + request.UserName;
                                request.RequestDateTime = DateTime.Now;
                                db.ContentNotifications.Add(request);
                                db.SaveChanges();
                            }

                            // ---- end ---------- //

                            //RedirectToAction("Index", "Profile");
                        }
                    }


                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }




            return RedirectToAction("Index", "Profile");
        }

        // POST: /CoverPhoto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="CoverPhotoId,UserId,CoverImage")] CoverPhoto coverphoto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(coverphoto).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(coverphoto);
        //}

        // GET: /CoverPhoto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoverPhoto coverphoto = db.CoverPhotoes.Find(id);
            if (coverphoto == null)
            {
                return HttpNotFound();
            }
            return View(coverphoto);
        }

        // POST: /CoverPhoto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CoverPhoto coverphoto = db.CoverPhotoes.Find(id);
            db.CoverPhotoes.Remove(coverphoto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
