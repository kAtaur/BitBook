using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitBookMVCApp.Models
{
    public class ContentNotification
    {
        public int ContentNotificationId { get; set; }

        public string PostId { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }

        public string TextPost { get; set; }

        public string ImagePost { get; set; }

        public string UserName { get; set; }

        public string Notification { get; set; }
        public string Content { get; set; }
        public DateTime RequestDateTime { get; set; } 
    }
}