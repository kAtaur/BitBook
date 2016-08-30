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
    public class UpdateNotificationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /UpdateNotification/
        public ActionResult Index()
        {
            return View(db.UpdateNotifications.ToList());
        }

        // GET: /UpdateNotification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UpdateNotification updatenotification = db.UpdateNotifications.Find(id);
            if (updatenotification == null)
            {
                return HttpNotFound();
            }
            return View(updatenotification);
        }

        // GET: /UpdateNotification/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /UpdateNotification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="UpdateNotificationId,UserId,Content,ContentDateTime")] UpdateNotification updatenotification)
        {
            if (ModelState.IsValid)
            {
                db.UpdateNotifications.Add(updatenotification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(updatenotification);
        }

        // GET: /UpdateNotification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UpdateNotification updatenotification = db.UpdateNotifications.Find(id);
            if (updatenotification == null)
            {
                return HttpNotFound();
            }
            return View(updatenotification);
        }

        // POST: /UpdateNotification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="UpdateNotificationId,UserId,Content,ContentDateTime")] UpdateNotification updatenotification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(updatenotification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(updatenotification);
        }

        // GET: /UpdateNotification/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UpdateNotification updatenotification = db.UpdateNotifications.Find(id);
            if (updatenotification == null)
            {
                return HttpNotFound();
            }
            return View(updatenotification);
        }

        // POST: /UpdateNotification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UpdateNotification updatenotification = db.UpdateNotifications.Find(id);
            db.UpdateNotifications.Remove(updatenotification);
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
