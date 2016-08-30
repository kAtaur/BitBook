using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BitBookMVCApp.Models;

namespace BitBookMVCApp.Controllers
{
    public class ContentNotificationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /ContentNotification/
        public ActionResult Index()
        {
            return View(db.ContentNotifications.ToList()); 
        }

        // GET: /ContentNotification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentNotification contentnotification = db.ContentNotifications.Find(id);
            if (contentnotification == null)
            {
                return HttpNotFound();
            }
            return View(contentnotification);
        }

        // GET: /ContentNotification/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ContentNotification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ContentNotificationId,UserId,FriendId,Content,RequestDateTime")] ContentNotification contentnotification)
        {
            if (ModelState.IsValid)
            {
                db.ContentNotifications.Add(contentnotification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contentnotification);
        }

        // GET: /ContentNotification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentNotification contentnotification = db.ContentNotifications.Find(id);
            if (contentnotification == null)
            {
                return HttpNotFound();
            }
            return View(contentnotification);
        }

        // POST: /ContentNotification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ContentNotificationId,UserId,FriendId,Content,RequestDateTime")] ContentNotification contentnotification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contentnotification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contentnotification);
        }

        // GET: /ContentNotification/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentNotification contentnotification = db.ContentNotifications.Find(id);
            if (contentnotification == null)
            {
                return HttpNotFound();
            }
            return View(contentnotification);
        }

        // POST: /ContentNotification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContentNotification contentnotification = db.ContentNotifications.Find(id);
            db.ContentNotifications.Remove(contentnotification);
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
