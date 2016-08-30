using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using BitBookMVCApp.Models;
using Microsoft.AspNet.Identity;

namespace BitBookMVCApp.Controllers
{
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Profile/

        public ActionResult Timeline()
        {       
            ViewBag.currentUser = User.Identity.GetUserId();

            if (User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var posts = db.Posts.ToList();

            List<Post> myPosts = new List<Post>();
            myPosts = posts.Where(t => t.UserId.Contains(User.Identity.GetUserId())).ToList();

           
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

            foreach (var item in myPosts)
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
            mymodel.PostWithLike = PostWithLike.OrderByDescending(d=>d.PostDate).Distinct();
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

            var coverPhoto = db.CoverPhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            mymodel.PostWithComment = CommentList;
            mymodel.coverPhoto = coverPhoto;

            ViewBag.coverPhoto = coverPhoto;
            ViewBag.profilePhoto = profilePhoto;

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

            return View(mymodel);
        }
        public ActionResult BrowseTimeline(string id)
        {
            ViewBag.currentUser = id;
            string userId = id;

            if (id == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var posts = db.Posts.ToList();

            List<Post> myPosts = new List<Post>();
            myPosts = posts.Where(t => t.UserId.Contains(userId)).ToList();

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
                    bool find = v.UserId.Equals(userId);
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

            foreach (var item in myPosts)
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

                string user = userId;

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
      
            var profilePhoto = db.ProfilePhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            dynamic mymodel = new ExpandoObject();
            mymodel.PostWithLike = PostWithLike.OrderByDescending(d => d.PostDate).Distinct();
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

            var coverPhoto = db.CoverPhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            mymodel.PostWithComment = CommentList;
            mymodel.coverPhoto = coverPhoto;

            ViewBag.coverPhoto = coverPhoto;
            ViewBag.profilePhoto = profilePhoto;

            return View(mymodel);
        }

        public ActionResult Index()
        {
            //return View(db.Profiles.ToList());
            dynamic mymodel = new ExpandoObject();
            string userId = User.Identity.GetUserId();

            var profile = db.Profiles.Where(s => s.UserId.Contains(userId)).ToList();
            var educations = db.Educations.Where(s => s.UserId.Contains(userId)).ToList();
            var experiences = db.Experiences.Where(s => s.UserId.Contains(userId)).ToList();
            var coverPhoto = db.CoverPhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            var profilePhoto = db.ProfilePhotoes.Where(s => s.UserId.Contains(userId)).ToList();

            if (profile.Count > 0)
            {
                mymodel.Profile = profile;
                mymodel.Education = educations;
                mymodel.Experience = experiences;
                mymodel.coverPhoto = coverPhoto;
                mymodel.profilePhoto = profilePhoto;
            }
            else
            {

                Profile aprofile = new Profile { ProfileId = 0 };
                profile.Add(aprofile);
                //aprofile.ProfileId = 0;
                mymodel.Profile = profile;
                mymodel.Education = educations;
                mymodel.Experience = experiences;
                mymodel.coverPhoto = coverPhoto;
                mymodel.profilePhoto = profilePhoto;
            }
            ViewBag.coverPhoto = coverPhoto;
            ViewBag.profilePhoto = profilePhoto;

            //Friend Request Count
            var friendIdList = new List<string>();
            var userIds = User.Identity.GetUserId();
            var friendIdQry = db.Friends.Where(d => d.FriendId == userIds && d.Status == "Pending").Select(d => d.UserId);
            friendIdList.AddRange(friendIdQry);

            //Notification Count
            string userIdNot = User.Identity.GetUserId();
            var notifications = db.ContentNotifications.Where(d => d.FriendId == userIdNot).ToList();
            ViewBag.NotificationCount = notifications.Count();

            var requestCount = friendIdList.Count();
            ViewBag.RequestCount = requestCount;

            return View(mymodel);
        }

        public ActionResult BrowseProfile(string id)
        {

            ViewBag.browseId = id;
            string userId = id;

            dynamic mymodel = new ExpandoObject();
            var profile = db.Profiles.Where(s => s.UserId.Contains(userId)).ToList();
            var educations = db.Educations.Where(s => s.UserId.Contains(userId)).ToList();
            var experiences = db.Experiences.Where(s => s.UserId.Contains(userId)).ToList();
            var coverPhoto = db.CoverPhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            var profilePhoto = db.ProfilePhotoes.Where(s => s.UserId.Contains(userId)).ToList();

            if (profile.Count > 0)
            {
                mymodel.Profile = profile;
                mymodel.Education = educations;
                mymodel.Experience = experiences;
                mymodel.coverPhoto = coverPhoto;
                mymodel.profilePhoto = profilePhoto;
            }
            else
            {

                Profile aprofile = new Profile { ProfileId = 0 };
                profile.Add(aprofile);
                //aprofile.ProfileId = 0;
                mymodel.Profile = profile;
                mymodel.Education = educations;
                mymodel.Experience = experiences;
                mymodel.coverPhoto = coverPhoto;
                mymodel.profilePhoto = profilePhoto;
            }

            ViewBag.coverPhoto = coverPhoto;
            ViewBag.profilePhoto = profilePhoto;

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

            return View(mymodel);
        }

        public ActionResult ViewFriendList()
        {
            var friendIdLst = new List<string>();
            var userIds = User.Identity.GetUserId();
            var friendIdQry = db.Friends.Where(d => d.UserId == userIds && d.Status == "True").Select(d => d.FriendId);
            friendIdLst.AddRange(friendIdQry);

            var friendIdLst2 = new List<string>();
            var userIds2 = User.Identity.GetUserId();
            var friendIdQry2 = db.Friends.Where(d => d.FriendId == userIds2 && d.Status == "True").Select(d => d.UserId);
            friendIdLst2.AddRange(friendIdQry2);

            List<SearchFriend> friendList = new List<SearchFriend>();

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

            foreach (var fr in friendIdLst)
            {
                var user = users.Where(s => s.Id.Contains(fr)).ToList();

                foreach (var v in user)
                {
                    SearchFriend sf = new SearchFriend();
                    sf.UserId = v.Id;
                    sf.UserName = v.Name;
                    sf.ProfilePhoto = v.ProfileImage;
                    friendList.Add(sf);
                }
            }

            foreach (var fr in friendIdLst2)
            {
                var user = users.Where(s => s.Id.Contains(fr)).ToList();

                foreach (var v in user)
                {
                    SearchFriend sf = new SearchFriend();
                    sf.UserId = v.Id;
                    sf.UserName = v.Name;
                    sf.ProfilePhoto = v.ProfileImage;
                    friendList.Add(sf);
                }
            }

            dynamic mymodel = new ExpandoObject();
            string userId = User.Identity.GetUserId();
            var coverPhoto = db.CoverPhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            var profilePhoto = db.ProfilePhotoes.Where(s => s.UserId.Contains(userId)).ToList();

            mymodel.coverPhoto = coverPhoto;
            mymodel.profilePhoto = profilePhoto;
            mymodel.friendList = friendList.Distinct();

            ViewBag.coverPhoto = coverPhoto;
            ViewBag.profilePhoto = profilePhoto;

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

            return View(mymodel);
        }

        public ActionResult BrowseFriendList(string id)
        {
            ViewBag.browseId = id;
            var userId = id;
            var friendIdLst = new List<string>();
            var friendIdQry = db.Friends.Where(d => d.UserId == userId && d.Status == "True").Select(d => d.FriendId);
            friendIdLst.AddRange(friendIdQry);

            var friendIdLst2 = new List<string>();
            var friendIdQry2 = db.Friends.Where(d => d.FriendId == userId && d.Status == "True").Select(d => d.UserId);
            friendIdLst2.AddRange(friendIdQry2);


            List<SearchFriend> friendList = new List<SearchFriend>();

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

            foreach (var fr in friendIdLst)
            {
                var user = users.Where(s => s.Id.Contains(fr)).ToList();

                foreach (var v in user)
                {
                    SearchFriend sf = new SearchFriend();
                    sf.UserId = v.Id;
                    sf.UserName = v.Name;
                    sf.ProfilePhoto = v.ProfileImage;
                    friendList.Add(sf);
                }
            }


            foreach (var fr in friendIdLst2)
            {
                var user = users.Where(s => s.Id.Contains(fr)).ToList();

                foreach (var v in user)
                {
                    SearchFriend sf = new SearchFriend();
                    sf.UserId = v.Id;
                    sf.UserName = v.Name;
                    sf.ProfilePhoto = v.ProfileImage;
                    friendList.Add(sf);
                }
            }

            dynamic mymodel = new ExpandoObject();
            var coverPhoto = db.CoverPhotoes.Where(s => s.UserId.Contains(userId)).ToList();
            var profilePhoto = db.ProfilePhotoes.Where(s => s.UserId.Contains(userId)).ToList();

            mymodel.coverPhoto = coverPhoto;
            mymodel.profilePhoto = profilePhoto;
            mymodel.friendList = friendList.Distinct();

            ViewBag.coverPhoto = coverPhoto;
            ViewBag.profilePhoto = profilePhoto;

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

            return View(mymodel);
        }

        public ActionResult SearchFreind(string searchFriend)
        {
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

            if (!String.IsNullOrWhiteSpace(searchFriend))
            {
                users = users.Where(n => n.Name.ToLower().Contains(searchFriend)).ToList();
            }

            List<SearchFriend> friendList = new List<SearchFriend>();
            foreach (var v in users)
            {
                SearchFriend sf = new SearchFriend();
                sf.UserId = v.Id;
                sf.UserName = v.Name;
                sf.ProfilePhoto = v.ProfileImage;
                friendList.Add(sf);
            }

            var addedfriendList = new List<SearchFriend>();
            var pendingFriendList = new List<SearchFriend>();
            var allUser = new List<SearchFriend>();

            string userId = User.Identity.GetUserId();

            // Friend Added
            var friendIdLst = new List<string>();
            var friendIdQry = db.Friends.Where(d => d.UserId == userId && d.Status == "True").Select(d => d.FriendId);
            friendIdLst.AddRange(friendIdQry);

            var friendIdLst2 = new List<string>();
            var friendIdQry2 = db.Friends.Where(d => d.FriendId == userId && d.Status == "True").Select(d => d.UserId);
            friendIdLst2.AddRange(friendIdQry2);

            var totalFriendList = new List<string>();

            foreach (var VARIABLE in friendIdLst)
            {
                totalFriendList.Add(VARIABLE);
            }

            foreach (var VARIABLE in friendIdLst2)
            {
                totalFriendList.Add(VARIABLE);
            }
            // End

            // Pending Friend
            var pendingFriendIdLst = new List<string>();
            var pendingFriendIdQry = db.Friends.Where(d => d.UserId == userId && d.Status == "Pending").Select(d => d.FriendId);
            pendingFriendIdLst.AddRange(pendingFriendIdQry);

            var pendingFriendIdLst2 = new List<string>();
            var pendingFriendIdQry2 = db.Friends.Where(d => d.FriendId == userId && d.Status == "Pending").Select(d => d.UserId);
            pendingFriendIdLst2.AddRange(pendingFriendIdQry2);

            var totalPendingFriendList = new List<string>();

            foreach (var VARIABLE in pendingFriendIdLst)
            {
                totalPendingFriendList.Add(VARIABLE);
            }

            foreach (var VARIABLE in pendingFriendIdLst2)
            {
                totalPendingFriendList.Add(VARIABLE);
            }
            // End

            foreach (var v in friendList)
            {
                bool exist = false;
                foreach (var item in totalFriendList)
                {
                    if (v.UserId == item)
                    {
                        addedfriendList.Add(v);
                        exist = true;
                    }
                }

                foreach (var item in totalPendingFriendList)
                {
                    if (v.UserId == item)
                    {
                        pendingFriendList.Add(v);
                        exist = true;
                    }
                }

                if (exist == false)
                {
                    allUser.Add(v);
                }
            }

            dynamic mymodel = new ExpandoObject();
            mymodel.AddedfriendList = addedfriendList;
            mymodel.PendingFriendList = pendingFriendList;
            mymodel.AllUser = allUser;

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

            return View(mymodel);
        }

        public JsonResult GetMP(string term)
        {
            List<string> result = (from sf in db.Users
                                   where sf.Name.ToLower().Contains(term)
                                   select sf.Name
                                   ).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ViewFriendRequest()
        {

            var friendIdList = new List<string>();
            var userIds = User.Identity.GetUserId();
            var friendIdQry = db.Friends.Where(d => d.FriendId == userIds && d.Status == "Pending").Select(d => d.UserId);
            friendIdList.AddRange(friendIdQry);

            List<SearchFriend> friendList = new List<SearchFriend>();

            var userList = (from u in db.Users
                            join pf in db.ProfilePhotoes
                            on u.Id equals pf.UserId into uGroup
                            from pf in uGroup.DefaultIfEmpty()
                            select new
                            {
                                u.Id,
                                u.Name,
                                pf.ProfileImage

                            }).ToList();

            foreach (string fr in friendIdList)
            {
                var user = userList.Where(s => s.Id.Contains(fr)).ToList();

                foreach (var v in user)
                {
                    SearchFriend sf = new SearchFriend();
                    sf.UserId = v.Id;
                    sf.UserName = v.Name;
                    sf.ProfilePhoto = v.ProfileImage;
                    friendList.Add(sf);
                }

            }
            dynamic mymodel = new ExpandoObject();
            mymodel.friendList = friendList.Distinct();

            //Friend Request Count
            var friendIdLists = new List<string>();
            var userIdss = User.Identity.GetUserId();
            var friendIdQrys = db.Friends.Where(d => d.FriendId == userIdss && d.Status == "Pending").Select(d => d.UserId);
            friendIdLists.AddRange(friendIdQrys);

            //Notification Count
            string userIdNot = User.Identity.GetUserId();
            var notifications = db.ContentNotifications.Where(d => d.FriendId == userIdNot).ToList();
            ViewBag.NotificationCount = notifications.Count();

            var requestCount = friendIdLists.Count();
            ViewBag.RequestCount = requestCount;

            return View(mymodel);
        }


        public ActionResult Notification()
        {
            string userId = User.Identity.GetUserId();
            var notifications = db.ContentNotifications.Where(d => d.FriendId == userId).ToList();

            dynamic mymodel = new ExpandoObject();
            mymodel.Notification = notifications.OrderByDescending(u => u.RequestDateTime).ToList();

            //Friend Request Count
            var friendIdLists = new List<string>();
            var userIdss = User.Identity.GetUserId();
            var friendIdQrys = db.Friends.Where(d => d.FriendId == userIdss && d.Status == "Pending").Select(d => d.UserId);
            friendIdLists.AddRange(friendIdQrys);

            //Notification Count
            string userIdNot = User.Identity.GetUserId();
            var notification = db.ContentNotifications.Where(d => d.FriendId == userIdNot).ToList();
            ViewBag.NotificationCount = notification.Count();

            var requestCount = friendIdLists.Count();
            ViewBag.RequestCount = requestCount;

            return View(mymodel);
        }



        // GET: /Profile/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // GET: /Profile/Create
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
            var friendIdLists = new List<string>();
            var userIdss = User.Identity.GetUserId();
            var friendIdQrys = db.Friends.Where(d => d.FriendId == userIdss && d.Status == "Pending").Select(d => d.UserId);
            friendIdLists.AddRange(friendIdQrys);

            //Notification Count
            string userIdNot = User.Identity.GetUserId();
            var notifications = db.ContentNotifications.Where(d => d.FriendId == userIdNot).ToList();
            ViewBag.NotificationCount = notifications.Count();

            var requestCount = friendIdLists.Count();
            ViewBag.RequestCount = requestCount;

            return View();
        }

        // POST: /Profile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProfileId,UserId,Name,Email,Gender,Contact,City,Country,AreaOfInterest,ProfilePhoto,CoverPhoto")] Profile profile)
        {
            profile.UserId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                db.Profiles.Add(profile);
                db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }

            return View(profile);
        }

        // GET: /Profile/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
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
            var friendIdLists = new List<string>();
            var userIdss = User.Identity.GetUserId();
            var friendIdQrys = db.Friends.Where(d => d.FriendId == userIdss && d.Status == "Pending").Select(d => d.UserId);
            friendIdLists.AddRange(friendIdQrys);

            //Notification Count
            string userIdNot = User.Identity.GetUserId();
            var notifications = db.ContentNotifications.Where(d => d.FriendId == userIdNot).ToList();
            ViewBag.NotificationCount = notifications.Count();

            var requestCount = friendIdLists.Count();
            ViewBag.RequestCount = requestCount;

            return View(profile);
        }

        // POST: /Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProfileId,UserId,Name,Email,Gender,Contact,City,Country,AreaOfInterest,ProfilePhoto,CoverPhoto")] Profile profile)
        {

            if (ModelState.IsValid)
            {
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profile);
        }

        // GET: /Profile/Delete/5
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
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: /Profile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Friend Request Count
            var friendIdLists = new List<string>();
            var userIdss = User.Identity.GetUserId();
            var friendIdQrys = db.Friends.Where(d => d.FriendId == userIdss && d.Status == "Pending").Select(d => d.UserId);
            friendIdLists.AddRange(friendIdQrys);

            //Notification Count
            string userIdNot = User.Identity.GetUserId();
            var notifications = db.ContentNotifications.Where(d => d.FriendId == userIdNot).ToList();
            ViewBag.NotificationCount = notifications.Count();

            var requestCount = friendIdLists.Count();
            ViewBag.RequestCount = requestCount;

            Profile profile = db.Profiles.Find(id);
            db.Profiles.Remove(profile);
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
