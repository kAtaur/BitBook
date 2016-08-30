using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitBookMVCApp.Models
{
    public class SearchFriend
    {
        public int SearchFriendId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string ProfilePhoto { get; set; }

    }
}