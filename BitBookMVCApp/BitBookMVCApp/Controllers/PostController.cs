using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using BitBookMVCApp.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace BitBookMVCApp.Controllers
{
    public class PostController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Post/
        public ActionResult Index()
        {
            ViewBag.currentUser = User.Identity.GetUserId();

            if (User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var posts = db.Posts.ToList();

            List<Post> myPosts = new List<Post>();
            myPosts = posts.Where(t => t.UserId.Contains(User.Identity.GetUserId())).ToList();

            var friendIdLst = new List<string>();
            var userIds = User.Identity.GetUserId();//my id
            var friendIdQry = db.Friends.Where(d => d.UserId == userIds && d.Status == "True").Select(d => d.FriendId);
            friendIdLst.AddRange(friendIdQry);

            var friendPosts = new List<Post>();

            foreach (string fi in friendIdLst)
            {
                List<Post> fPosts = posts.Where(t => t.UserId.Contains(fi)).OrderBy(d => d.PostDate).ToList();
                foreach (var VARIABLE in fPosts)
                {
                    friendPosts.Add(VARIABLE);
                }
            }

            foreach (var VARIABLE in friendPosts)
            {
                myPosts.Add(VARIABLE);
            }

            var friendIdLst2 = new List<string>();

            var userIds2 = User.Identity.GetUserId();//my id
            var friendIdQry2 = db.Friends.Where(d => d.FriendId == userIds2 && d.Status == "True").Select(d => d.UserId);
            friendIdLst2.AddRange(friendIdQry2);

            var friendPosts2 = new List<Post>();

            foreach (string fi in friendIdLst2)
            {
                List<Post> fPosts2 = posts.Where(t => t.UserId.Contains(fi)).OrderBy(d => d.PostDate).ToList();
                foreach (var VARIABLE in fPosts2)
                {
                    friendPosts2.Add(VARIABLE);
                }
            }

            foreach (var VARIABLE in friendPosts2)
            {
                myPosts.Add(VARIABLE);
            }

            var allPost = myPosts.OrderByDescending(d => d.PostDate).Distinct();

            var postIdList = db.Likes.Select(d => d.PostId).Distinct().ToList();
            List<PostLikeComment> likeList = new List<PostLikeComment>();

            foreach (var VARIABLE in postIdList)
            {
                //var user1 = db.Likes.Where(d => d.PostId == VARIABLE).Select new{(d => d.UserId).ToList();}
                var user = (from l in db.Likes
                            where l.PostId == VARIABLE
                            select new
                            {
                                l.PostId,
                                l.UserId
                            }).ToList();

                // Total Count
                int postIdCount = user.Count;
                int countWithUser = 0;
                int countWithoutUser = 0;

                foreach (var v in user)
                {
                    bool find = v.UserId.Equals(User.Identity.GetUserId());
                    if (find)
                    {
                        countWithUser = 1;
                    }
                }

                countWithoutUser = postIdCount - countWithUser;

                foreach (var v in user)
                {
                    PostLikeComment aLike = new PostLikeComment();
                    aLike.PostId = v.PostId;
                    aLike.TotalLikeCount = postIdCount;
                    aLike.LikeStatus = countWithUser;
                    likeList.Add(aLike);
                }
            }
            //End of PostLikeComment

            List<PostLikeComment> PostWithLike = new List<PostLikeComment>();

            foreach (var item in allPost)
            {
                PostLikeComment aPostLikeComment = new PostLikeComment();

                aPostLikeComment.PostId = item.PostId;
                aPostLikeComment.UserId = item.UserId;
                aPostLikeComment.UserName = item.UserName;
                aPostLikeComment.TextPost = item.TextPost;
                aPostLikeComment.ImgPost = item.ImgPost;
                aPostLikeComment.PostDate = item.PostDate;

                var aLikes = (from l in db.Likes
                              where l.PostId == item.PostId
                              select new
                              {
                                  l.Status
                              }).ToList();

                foreach (var v in aLikes)
                {
                    aPostLikeComment.TotalLikeCount = aLikes.Count;
                }

                string user = User.Identity.GetUserId();

                var aLikess = (from l in db.Likes
                               where l.UserId == user && l.PostId == item.PostId
                               select new
                               {
                                   l.Status
                               }).ToList();

                foreach (var v in aLikes)
                {
                    aPostLikeComment.LikeStatus = aLikess.Count;
                }

                PostWithLike.Add(aPostLikeComment);
            }

            string userId = User.Identity.GetUserId();
            var profilePhoto = db.ProfilePhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            dynamic mymodel = new ExpandoObject();
            mymodel.PostWithLike = PostWithLike;
            mymodel.profilePhoto = profilePhoto;

            //Load Post Comment
            var postIds = db.PostComments.Select(d => d.PostId).Distinct().ToList();

            var postIdCommentList = db.PostComments.ToList();

            List<ViewPostComment> CommentList = new List<ViewPostComment>();

            foreach (var VARIABLE in postIds)
            {
                // Total Comment Count           

                var comments = (from l in db.PostComments
                                where l.PostId == VARIABLE
                                select new
                                {
                                    l.UserComment,
                                    l.UserId
                                }).ToList();

                // End of Loading

                ViewPostComment aComment = new ViewPostComment();

                // Passing Post Comments to View myModel in Profile Controller
                aComment.PostId = VARIABLE;
                List<ViewCommentwithUser> comment = new List<ViewCommentwithUser>();
                foreach (var v1 in comments)
                {
                    var userName = (from u in db.Users
                                    where u.Id == v1.UserId
                                    select new { u.Name, u.Id }).ToList();


                    string userNames = "";
                    foreach (var v2 in userName)
                    {

                        userNames = v2.Name;
                    }

                    ViewCommentwithUser commentUser = new ViewCommentwithUser();
                    commentUser.UserName = userNames;
                    commentUser.CommentatorId = v1.UserId;
                    commentUser.Comment = v1.UserComment;
                    comment.Add(commentUser);
                }

                aComment.Ucomment = comment;
                CommentList.Add(aComment);
            }

            mymodel.PostWithComment = CommentList;

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

            if (TempData["Msg"] == null)
            {
                
            }
            else
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
           
            return View(mymodel);
        }

        // GET: /Post/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts.Find(id);
            dynamic mymodel = new ExpandoObject();

            if (post == null)
            {
                //Friend Request Count
                var friendIdList = new List<string>();
                var userIdss = User.Identity.GetUserId();
                var friendIdQrys = db.Friends.Where(d => d.FriendId == userIdss && d.Status == "Pending").Select(d => d.UserId);
                friendIdList.AddRange(friendIdQrys);

                var requestCount = friendIdList.Count();
                ViewBag.RequestCount = requestCount;

                //Notification Count
                string userIdNot = User.Identity.GetUserId();
                var notifications = db.ContentNotifications.Where(d => d.FriendId == userIdNot).ToList();
                ViewBag.NotificationCount = notifications.Count();

                ViewBag.Message = "This post is no longer available";

                return View(mymodel);
            }

            else
            {
                var posts = db.Posts.ToList();

                List<Post> myPosts = new List<Post>();
                myPosts = posts.Where(t => t.UserId.Contains(User.Identity.GetUserId())).ToList();

                var friendIdLst = new List<string>();
                var userIds = User.Identity.GetUserId();//my id
                var friendIdQry = db.Friends.Where(d => d.UserId == userIds && d.Status == "True").Select(d => d.FriendId);
                friendIdLst.AddRange(friendIdQry);

                var friendPosts = new List<Post>();

                foreach (string fi in friendIdLst)
                {
                    List<Post> fPosts = posts.Where(t => t.UserId.Contains(fi)).OrderBy(d => d.PostDate).ToList();
                    foreach (var VARIABLE in fPosts)
                    {
                        friendPosts.Add(VARIABLE);
                    }
                }

                foreach (var VARIABLE in friendPosts)
                {
                    myPosts.Add(VARIABLE);
                }

                var friendIdLst2 = new List<string>();

                var userIds2 = User.Identity.GetUserId();//my id
                var friendIdQry2 = db.Friends.Where(d => d.FriendId == userIds2 && d.Status == "True").Select(d => d.UserId);
                friendIdLst2.AddRange(friendIdQry2);

                var friendPosts2 = new List<Post>();

                foreach (string fi in friendIdLst2)
                {
                    List<Post> fPosts2 = posts.Where(t => t.UserId.Contains(fi)).OrderBy(d => d.PostDate).ToList();
                    foreach (var VARIABLE in fPosts2)
                    {
                        friendPosts2.Add(VARIABLE);
                    }
                }

                foreach (var VARIABLE in friendPosts2)
                {
                    myPosts.Add(VARIABLE);
                }

                var allPost = myPosts.OrderByDescending(d => d.PostDate).Distinct();

                var postIdList = db.Likes.Select(d => d.PostId).Distinct().ToList();
                List<PostLikeComment> likeList = new List<PostLikeComment>();

                foreach (var VARIABLE in postIdList)
                {
                    //var user1 = db.Likes.Where(d => d.PostId == VARIABLE).Select new{(d => d.UserId).ToList();}
                    var user = (from l in db.Likes
                                where l.PostId == VARIABLE
                                select new
                                {
                                    l.PostId,
                                    l.UserId
                                }).ToList();

                    // Total Count
                    int postIdCount = user.Count;
                    int countWithUser = 0;
                    int countWithoutUser = 0;

                    foreach (var v in user)
                    {
                        bool find = v.UserId.Equals(User.Identity.GetUserId());
                        if (find)
                        {
                            countWithUser = 1;
                        }
                    }

                    countWithoutUser = postIdCount - countWithUser;

                    foreach (var v in user)
                    {
                        PostLikeComment aLike = new PostLikeComment();
                        aLike.PostId = v.PostId;
                        aLike.TotalLikeCount = postIdCount;
                        aLike.LikeStatus = countWithUser;
                        likeList.Add(aLike);
                    }
                }
                //End of PostLikeComment

                List<PostLikeComment> PostWithLike = new List<PostLikeComment>();

                foreach (var item in allPost)
                {
                    PostLikeComment aPostLikeComment = new PostLikeComment();

                    aPostLikeComment.PostId = item.PostId;
                    aPostLikeComment.UserId = item.UserId;
                    aPostLikeComment.UserName = item.UserName;
                    aPostLikeComment.TextPost = item.TextPost;
                    aPostLikeComment.ImgPost = item.ImgPost;
                    aPostLikeComment.PostDate = item.PostDate;

                    var aLikes = (from l in db.Likes
                                  where l.PostId == item.PostId
                                  select new
                                  {
                                      l.Status
                                  }).ToList();

                    foreach (var v in aLikes)
                    {
                        aPostLikeComment.TotalLikeCount = aLikes.Count;
                    }

                    string user = User.Identity.GetUserId();

                    var aLikess = (from l in db.Likes
                                   where l.UserId == user && l.PostId == item.PostId
                                   select new
                                   {
                                       l.Status
                                   }).ToList();

                    foreach (var v in aLikes)
                    {
                        aPostLikeComment.LikeStatus = aLikess.Count;
                    }

                    PostWithLike.Add(aPostLikeComment);
                }

                string userId = User.Identity.GetUserId();
                var profilePhoto = db.ProfilePhotoes.Where(s => s.UserId.Contains(userId)).ToList();

                mymodel.PostWithLike = PostWithLike;
                mymodel.profilePhoto = profilePhoto;

                //Load Post Comment
                var postIds = db.PostComments.Select(d => d.PostId).Distinct().ToList();

                var postIdCommentList = db.PostComments.ToList();

                List<ViewPostComment> CommentList = new List<ViewPostComment>();

                foreach (var VARIABLE in postIds)
                {
                    // Total Comment Count           

                    var comments = (from l in db.PostComments
                                    where l.PostId == VARIABLE
                                    select new
                                    {
                                        l.UserComment,
                                        l.UserId
                                    }).ToList();

                    // End of Loading

                    ViewPostComment aComment = new ViewPostComment();

                    // Passing Post Comments to View myModel in Profile Controller
                    aComment.PostId = VARIABLE;
                    List<ViewCommentwithUser> comment = new List<ViewCommentwithUser>();
                    foreach (var v1 in comments)
                    {
                        var userName = (from u in db.Users
                                        where u.Id == v1.UserId
                                        select new { u.Name, u.Id }).ToList();

                        string userNames = "";
                        foreach (var v2 in userName)
                        {

                            userNames = v2.Name;
                        }

                        ViewCommentwithUser commentUser = new ViewCommentwithUser();
                        commentUser.UserName = userNames;
                        commentUser.CommentatorId = v1.UserId;
                        commentUser.Comment = v1.UserComment;
                        comment.Add(commentUser);
                    }

                    aComment.Ucomment = comment;
                    CommentList.Add(aComment);
                }

                List<Post> postList = new List<Post>();
                postList.Add(post);
                mymodel.Post = postList;
                mymodel.PostWithComment = CommentList;

                //Friend Request Count
                var friendIdLists = new List<string>();
                var userIdsss = User.Identity.GetUserId();
                var friendIdQryss = db.Friends.Where(d => d.FriendId == userIdsss && d.Status == "Pending").Select(d => d.UserId);
                friendIdLists.AddRange(friendIdQryss);

                var requestCounts = friendIdLists.Count();
                ViewBag.RequestCount = requestCounts;

                //Notification Count
                string userIdNots = User.Identity.GetUserId();
                var notificationss = db.ContentNotifications.Where(d => d.FriendId == userIdNots).ToList();
                ViewBag.NotificationCount = notificationss.Count();
            }

            return View(mymodel);
        }

        // GET: /Post/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Post/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId,UserId,UserName,TextPost,ImgPost,PostDate")] Post post, HttpPostedFileBase file)
        {

            string userId = User.Identity.GetUserId();
            var user = (from u in db.Users
                        where u.Id == userId
                        select new { u.Name }).ToList();
            string userName = "";
            foreach (var v in user)
            {
                userName = v.Name;
            }

            if (post.TextPost == null && file == null)
            {
                TempData["Msg"] = "Please provide an input";
                return RedirectToAction("Index", "Post");
            }

            else
            {
                if (file != null && file.ContentLength > 0)
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/Images/PostPic"),
                                                  Path.GetFileName(file.FileName));

                        post.ImgPost = "~/Images/PostPic/" + Path.GetFileName(file.FileName);

                        file.SaveAs(path);
                        //ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                post.UserId = User.Identity.GetUserId();
                post.UserName = userName;
                post.PostDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Posts.Add(post);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }

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

            return RedirectToAction("Index", "Post");
        }

        // GET: /Post/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // POST: /Post/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,UserId,UserName,TextPost,ImgPost,PostDate")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: /Post/Delete/5
        public ActionResult Delete(int? id)
        {
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: /Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
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

            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult TimeLinePostDelete(int? id)
        {
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: /Post/Delete/5
        [HttpPost, ActionName("TimeLinePostDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult TimeLinePostDeleteConfirmed(int id)
        {
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

            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Timeline", "Profile");
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
