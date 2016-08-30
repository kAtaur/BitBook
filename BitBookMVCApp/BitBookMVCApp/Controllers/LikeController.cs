using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BitBookMVCApp.Models;
using Microsoft.AspNet.Identity;

namespace BitBookMVCApp.Controllers
{
    public class LikeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Like/


        //public ActionResult Like(int id)
        //{

        //}


        public ActionResult Index()
        {

            return View();
        }


        public string Test()
        {

            return "Hello";
        }


        // GET: /Like/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Like like = db.Likes.Find(id);
            if (like == null)
            {
                return HttpNotFound();
            }
            return View(like);
        }

        //// GET: /Like/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: /Like/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="Id,UserId,PostId,Status")] Like like)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        db.Likes.Add(like);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(like);
        //}

        public string LikeExist(int postId)
        {
            var likeList = db.Likes.ToList();

            var likeWithUser =
                       (from like in likeList
                        where like.UserId == User.Identity.GetUserId() && like.PostId == postId
                        select new { like.PostId, like.UserId }).ToList();

            var likesWithoutUser =
                      (from like in likeList
                       where like.UserId != User.Identity.GetUserId() && like.PostId == postId
                       select new { like.PostId, like.UserId }).ToList();

            var userId = User.Identity.GetUserId();

            int likeNumber;

            //var likeNumber = db.Likes.Where(d => d.UserId == userId).Select(d => d.Status).Count();

            if (likeWithUser == null && likesWithoutUser == null)
            {
                return "";
            }
            else
                if (likeWithUser == null)
                {
                    likeNumber = Convert.ToInt32(likesWithoutUser.ToString());
                    return likeNumber + " users like this post";
                }
                else
                {
                    likeNumber = Convert.ToInt32(likesWithoutUser.ToString()) - 1;
                    return "You & " + likeNumber + " users like this post";
                }

            //return "1";
        }

        [HttpPost]
        public string Create(int postIdString)
        {
            var postId = Convert.ToInt32(postIdString);
            var likeList = db.Likes.ToList();

            var likeWithUser =
                       (from like in likeList
                        where like.UserId == User.Identity.GetUserId() && like.PostId == postId
                        select new { like.PostId, like.UserId }).ToList();

            var likesWithoutUser =
                      (from like in likeList
                       where like.UserId != User.Identity.GetUserId() && like.PostId == postId
                       select new { like.PostId, like.UserId }).ToList();

            int likeNumber = 0;

            if (likeWithUser.Count == 0)
            {
                Like aLike = new Like();
                aLike.UserId = User.Identity.GetUserId();
                aLike.PostId = Convert.ToInt32(postId);
                aLike.Status = 1;
                db.Likes.Add(aLike);
                db.SaveChanges();

                var friendIdList1 = new List<string>();
                var userId = User.Identity.GetUserId();
                var friendIdQuery1 = db.Friends.Where(d => d.UserId == userId && d.Status == "True").Select(d => d.FriendId);
                friendIdList1.AddRange(friendIdQuery1);

                var friendIdList2 = new List<string>();
                var friendIdQuery2 = db.Friends.Where(d => d.FriendId == userId && d.Status == "True").Select(d => d.UserId);
                friendIdList2.AddRange(friendIdQuery2);

                var friendList = new List<string>();

                foreach (var VARIABLE in friendIdList1)
                {
                    friendList.Add(VARIABLE);
                }

                foreach (var VARIABLE in friendIdList2)
                {
                    friendList.Add(VARIABLE);
                }

                var name1 = (from u in db.Users
                             where u.Id == userId
                             select new
                             {
                                 u.Name
                             }).ToList();

                string name = "";
                foreach (var VARIABLE in name1)
                {
                    name = VARIABLE.Name;
                }

                ContentNotification request = new ContentNotification();
                Post post = db.Posts.Find(postId);
                request.TextPost = post.TextPost;
                request.ImagePost = post.ImgPost;

                foreach (var item in friendList)
                {
                    request.FriendId = item;
                    request.UserName = name;
                    request.UserId = User.Identity.GetUserId();
                    request.PostId = postId.ToString();
                    request.Notification = "Post " + request.TextPost + " Liked by " + request.UserName;
                    request.RequestDateTime = DateTime.Now;
                    db.ContentNotifications.Add(request);
                    db.SaveChanges();
                }

                likeNumber = Convert.ToInt32(likesWithoutUser.Count().ToString()) + 1;
                return likeNumber.ToString();
            }

            likeNumber = Convert.ToInt32(likesWithoutUser.Count().ToString()) + 1;
            return likeNumber.ToString();
        }

        public ActionResult BrowseLikers(int? id)
        {
            // Find UserIds for likers by postId
            var likers =
                        (from like in db.Likes
                         where like.PostId == id
                         select new { like.UserId }).ToList();

            // Find Users for likers 
            var users = (from u in db.Users
                         join p in db.ProfilePhotoes
                         on u.Id equals p.UserId into uGroup
                         from p in uGroup.DefaultIfEmpty()
                         select new
                         {
                             u.Id,
                             u.Name,
                             p.ProfileImage
                         }).ToList();

            //Create List for Post Likers

            List<SearchFriend> LikeUserList = new List<SearchFriend>();

            // For each likers
            foreach (var item1 in likers)
            {
                // Relative information
                foreach (var item2 in users)
                {
                    if (item1.UserId == item2.Id)
                    {
                        SearchFriend sf = new SearchFriend();
                        sf.UserId = item2.Id;
                        sf.UserName = item2.Name;
                        sf.ProfilePhoto = item2.ProfileImage;
                        LikeUserList.Add(sf);
                    }
                }
            }

            // Pass to myModel in Profile Controller for view
            dynamic mymodel = new ExpandoObject();
            mymodel.LikeUsers = LikeUserList;

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

            return  View(mymodel);
        }

        [HttpPost]
        public ActionResult CreateComment(int postId, string comment)
        {
            var commentList = db.PostComments.ToList();

            // Create & save an user passed comment
            PostComment aComment = new PostComment();
            aComment.UserId = User.Identity.GetUserId();
            aComment.PostId = Convert.ToInt32(postId);
            aComment.UserComment = comment;
            db.PostComments.Add(aComment);
            db.SaveChanges();

            // Find friends from 1st column UserId
            var friendIdList1 = new List<string>();
            var userId = User.Identity.GetUserId();
            var friendIdQuery1 = db.Friends.Where(d => d.UserId == userId && d.Status=="True").Select(d => d.FriendId);
            friendIdList1.AddRange(friendIdQuery1);

            // Find friends from 2nd column FriendId
            var friendIdList2 = new List<string>();
            var friendIdQuery2 = db.Friends.Where(d => d.FriendId == userId && d.Status == "True").Select(d => d.UserId);
            friendIdList2.AddRange(friendIdQuery2);

            var friendList = new List<string>();

            // Prepare friend list by
            foreach (var VARIABLE in friendIdList1)
            {
                friendList.Add(VARIABLE);
            }

            // Prepare friend list by id
            foreach (var VARIABLE in friendIdList2)
            {
                friendList.Add(VARIABLE);
            }

            var name1 = (from u in db.Users
                         where u.Id == userId
                         select new
                         {
                             u.Name
                         }).ToList();

            string name = "";

            // Add names to friendList
            foreach (var VARIABLE in name1)
            {
                name = VARIABLE.Name;
            }

            // Create notification request
            ContentNotification request = new ContentNotification();
            Post post = db.Posts.Find(postId);
            request.TextPost = post.TextPost;
            request.ImagePost = post.ImgPost;

            foreach (var item in friendList)
            {
                request.FriendId = item;
                request.UserId = User.Identity.GetUserId();
                request.UserName = name;
                request.PostId = postId.ToString();
                request.Notification = request.TextPost + " Commented by " + request.UserName;
                request.RequestDateTime = DateTime.Now;
                db.ContentNotifications.Add(request);
                db.SaveChanges();
            }

            //int commentNumber = 0;

            //commentNumber = db.PostComments.ToList().Count;
            //return commentNumber.ToString();

            return RedirectToAction("Index", "Post");
        }
        public ActionResult CreateCommentFromTimeline(int postId, string comment)
        {
            var commentList = db.PostComments.ToList();

            // Create & save an user passed comment
            PostComment aComment = new PostComment();
            aComment.UserId = User.Identity.GetUserId();
            aComment.PostId = Convert.ToInt32(postId);
            aComment.UserComment = comment;
            db.PostComments.Add(aComment);
            db.SaveChanges();

            // Find friends from 1st column UserId
            var friendIdList1 = new List<string>();
            var userId = User.Identity.GetUserId();
            var friendIdQuery1 = db.Friends.Where(d => d.UserId == userId && d.Status == "True").Select(d => d.FriendId);
            friendIdList1.AddRange(friendIdQuery1);

            // Find friends from 2nd column FriendId
            var friendIdList2 = new List<string>();
            var friendIdQuery2 = db.Friends.Where(d => d.FriendId == userId && d.Status == "True").Select(d => d.UserId);
            friendIdList2.AddRange(friendIdQuery2);

            var friendList = new List<string>();

            // Prepare friend list by
            foreach (var VARIABLE in friendIdList1)
            {
                friendList.Add(VARIABLE);
            }

            // Prepare friend list by id
            foreach (var VARIABLE in friendIdList2)
            {
                friendList.Add(VARIABLE);
            }

            var name1 = (from u in db.Users
                         where u.Id == userId
                         select new
                         {
                             u.Name
                         }).ToList();

            string name = "";

            // Add names to friendList
            foreach (var VARIABLE in name1)
            {
                name = VARIABLE.Name;
            }

            // Create notification request
            ContentNotification request = new ContentNotification();
            Post post = db.Posts.Find(postId);
            request.TextPost = post.TextPost;
            request.ImagePost = post.ImgPost;

            foreach (var item in friendList)
            {
                request.FriendId = item;
                request.UserId = User.Identity.GetUserId();
                request.UserName = name;
                request.PostId = postId.ToString();
                request.Notification = request.TextPost + " Commented by " + request.UserName;
                request.RequestDateTime = DateTime.Now;
                db.ContentNotifications.Add(request);
                db.SaveChanges();
            }

            //int commentNumber = 0;

            //commentNumber = db.PostComments.ToList().Count;
            //return commentNumber.ToString();

            return RedirectToAction("Timeline", "Profile");
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(int postId)
        //{
        //    Like like = new Like();
        //    like.UserId = User.Identity.GetUserId();
        //    like.PostId = postId;
        //    like.Status = 1;

        //    if (ModelState.IsValid)
        //    {
        //        db.Likes.Add(like);
        //        db.SaveChanges();

        //        return RedirectToAction("Index","Post");
        //    }

        //    return View(like);
        //}




        // GET: /Like/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Like like = db.Likes.Find(id);
            if (like == null)
            {
                return HttpNotFound();
            }
            return View(like);
        }

        // POST: /Like/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,PostId,Status")] Like like)
        {
            if (ModelState.IsValid)
            {
                db.Entry(like).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(like);
        }

        // GET: /Like/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Like like = db.Likes.Find(id);
            if (like == null)
            {
                return HttpNotFound();
            }
            return View(like);
        }

        // POST: /Like/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Like like = db.Likes.Find(id);
            db.Likes.Remove(like);
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
