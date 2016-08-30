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
    public class ExperienceController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Experience/
        public ActionResult Index()
        {
            return View(db.Experiences.ToList());
        }

        // GET: /Experience/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Experience experience = db.Experiences.Find(id);
            if (experience == null)
            {
                return HttpNotFound();
            }
            return View(experience);
        }

        // GET: /Experience/Create
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

        // POST: /Experience/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ExperienceId,UserId,ExpDesignation,ExpCompany,ExpYear")] Experience experience)
        {
            experience.UserId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.Experiences.Add(experience);
                db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }

            return View(experience);
        }

        // GET: /Experience/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Experience experience = db.Experiences.Find(id);
            if (experience == null)
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

            return View(experience);
        }

        // POST: /Experience/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ExperienceId,UserId,ExpDesignation,ExpCompany,ExpYear")] Experience experience)
        {
            if (ModelState.IsValid)
            {
                db.Entry(experience).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }
            return View(experience);
        }

        // GET: /Experience/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Experience experience = db.Experiences.Find(id);
            if (experience == null)
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

            return View(experience);
        }

        // POST: /Experience/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Experience experience = db.Experiences.Find(id);
            db.Experiences.Remove(experience);
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
