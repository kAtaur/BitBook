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
    public class FriendController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Friend/
        public ActionResult Index()
        {
            return View(db.Friends.ToList());
        }

        // GET: /Friend/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Friend friend = db.Friends.Find(id);
            if (friend == null)
            {
                return HttpNotFound();
            }
            return View(friend);
        }

        // GET: /Friend/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Friend/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,UserId,FriendId,Status")] Friend friend)
        {
            friend.UserId = User.Identity.GetUserId();
            friend.Status = "Pending";

            if (ModelState.IsValid)
            {
                db.Friends.Add(friend);
                db.SaveChanges();
                return RedirectToAction("SearchFreind","Profile");
            }

            return View(friend);
        }

        // GET: /Friend/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Friend friend = db.Friends.Find(id);
            if (friend == null)
            {
                return HttpNotFound();
            }
            return View(friend);
        }

        // POST: /Friend/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,UserId,FriendId,Status")] Friend friend)
        {
            if (ModelState.IsValid)
            {
                db.Entry(friend).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(friend);
        }

        [HttpPost]
        public ActionResult AccreptFriendRequest(string UserId, string FriendId)
        {
            Friend friend = new Friend( );

            //Friend friendId = db.Friends.Find();

            UserId = User.Identity.GetUserId();
            var fId = (from u in db.Friends
                where u.FriendId.Equals(UserId) && u.UserId.Equals(FriendId)
                select new 
                {
                    u.Id
                });

            int id = 0;
            foreach (var v in fId)
            {
                id = v.Id;
            }

            friend = db.Friends.Find(id); 
            
            if (ModelState.IsValid)
            {
                friend.Status = "True";
                db.Entry(friend).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ViewFriendRequest","Profile");
        }

        [HttpPost]
        public ActionResult RejectFriendRequest(string UserId, string FriendId)
        {
            Friend friend = new Friend();

            UserId = User.Identity.GetUserId();
            var fId = (from u in db.Friends
                       where u.FriendId.Equals(UserId) && u.UserId.Equals(FriendId)
                       select new
                       {
                           u.Id
                       });

            int id = 0;
            foreach (var v in fId)
            {
                id = v.Id;
            }

            friend = db.Friends.Find(id);

            db.Friends.Remove(friend);
            db.SaveChanges();

            return RedirectToAction("ViewFriendRequest", "Profile");
        }

        // GET: /Friend/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Friend friend = db.Friends.Find(id);
            if (friend == null)
            {
                return HttpNotFound();
            }
            return View(friend);
        }

        // POST: /Friend/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string UserId, string FriendId)
        {
           
            UserId = User.Identity.GetUserId();
            var fId = (from u in db.Friends
                       where u.FriendId.Equals(UserId) && u.UserId.Equals(FriendId) && u.Status.Equals("True")
                       select new
                       {
                           u.Id
                       });

            var fId2 = (from u in db.Friends
                        where u.FriendId.Equals(FriendId) && u.UserId.Equals(UserId) && u.Status.Equals("True")
                       select new
                       {
                           u.Id
                       });

            int id = 0;
            foreach (var v in fId)
            {
                id = v.Id;
            }

            foreach (var v in fId2)
            {
                id = v.Id;
            }
            
            Friend friend = db.Friends.Find(id); 

            db.Friends.Remove(friend);
            db.SaveChanges();
            return RedirectToAction("ViewFriendList","Profile");
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
