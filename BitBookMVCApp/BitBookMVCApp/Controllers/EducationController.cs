using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BitBookMVCApp.Models;
using Microsoft.AspNet.Identity;

namespace BitBookMVCApp.Controllers
{
    public class EducationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Education/
        public ActionResult Index()
        {
            return View(db.Educations.ToList());
        }

        // GET: /Education/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Education education = db.Educations.Find(id);
            if (education == null)
            {
                return HttpNotFound();
            }
            return View(education);
        }

        // GET: /Education/Create
        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();
            var coverPhoto = db.CoverPhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            var profilePhoto = db.ProfilePhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            ViewBag.coverPhoto = coverPhoto;
            ViewBag.profilePhoto = profilePhoto;

            var userName = (from u in db.Users
                            where u.Id == userId
                            select new
                            {
                                u.Name
                            }).ToList();
            string name = "";
            foreach (var v in userName)
            {
                name = v.Name;
            }

            ViewBag.name = name;

            //Friend Request Count
            var friendIdList = new List<string>();
            var userIdss = User.Identity.GetUserId();
            var friendIdQrys = db.Friends.Where(d => d.FriendId == userIdss && d.Status == "Pending").Select(d => d.UserId);
            friendIdList.AddRange(friendIdQrys);

            //Notification Count
            string userIdNot = User.Identity.GetUserId();
            var notifications = db.ContentNotifications.Where(d => d.FriendId == userIdNot).ToList();
            ViewBag.NotificationCount = notifications.Count();

            var requestCount = friendIdList.Count();
            ViewBag.RequestCount = requestCount;

            return View();
        }

        // POST: /Education/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="EducationId,UserId,EduTitle,EduInstitute,EduYear")] Education education)
        {
            education.UserId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.Educations.Add(education);
                db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }

            return View(education);
        }

        // GET: /Education/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Education education = db.Educations.Find(id);
            if (education == null)
            {
                return HttpNotFound();
            }

            string userId = User.Identity.GetUserId();
            var coverPhoto = db.CoverPhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            var profilePhoto = db.ProfilePhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            ViewBag.coverPhoto = coverPhoto;
            ViewBag.profilePhoto = profilePhoto;

            var userName = (from u in db.Users
                            where u.Id == userId
                            select new
                            {
                                u.Name
                            }).ToList();
            string name = "";
            foreach (var v in userName)
            {
                name = v.Name;
            }

            ViewBag.name = name;

            //Friend Request Count
            var friendIdList = new List<string>();
            var userIdss = User.Identity.GetUserId();
            var friendIdQrys = db.Friends.Where(d => d.FriendId == userIdss && d.Status == "Pending").Select(d => d.UserId);
            friendIdList.AddRange(friendIdQrys);

            //Notification Count
            string userIdNot = User.Identity.GetUserId();
            var notifications = db.ContentNotifications.Where(d => d.FriendId == userIdNot).ToList();
            ViewBag.NotificationCount = notifications.Count();

            var requestCount = friendIdList.Count();
            ViewBag.RequestCount = requestCount;

            return View(education);
        }

        // POST: /Education/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="EducationId,UserId,EduTitle,EduInstitute,EduYear")] Education education)
        {
            if (ModelState.IsValid)
            {
                db.Entry(education).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }
            return View(education);
        }

        // GET: /Education/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Education education = db.Educations.Find(id);
            if (education == null)
            {
                return HttpNotFound();
            }

            string userId = User.Identity.GetUserId();
            var coverPhoto = db.CoverPhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            var profilePhoto = db.ProfilePhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            ViewBag.coverPhoto = coverPhoto;
            ViewBag.profilePhoto = profilePhoto;

            var userName = (from u in db.Users
                            where u.Id == userId
                            select new
                            {
                                u.Name
                            }).ToList();
            string name = "";
            foreach (var v in userName)
            {
                name = v.Name;
            }

            ViewBag.name = name;

            //Friend Request Count
            var friendIdList = new List<string>();
            var userIdss = User.Identity.GetUserId();
            var friendIdQrys = db.Friends.Where(d => d.FriendId == userIdss && d.Status == "Pending").Select(d => d.UserId);
            friendIdList.AddRange(friendIdQrys);

            //Notification Count
            string userIdNot = User.Identity.GetUserId();
            var notifications = db.ContentNotifications.Where(d => d.FriendId == userIdNot).ToList();
            ViewBag.NotificationCount = notifications.Count();

            var requestCount = friendIdList.Count();
            ViewBag.RequestCount = requestCount;

            return View(education);
        }

        // POST: /Education/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Education education = db.Educations.Find(id);
            db.Educations.Remove(education);
            db.SaveChanges();
            return RedirectToAction("Index", "Profile");
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
