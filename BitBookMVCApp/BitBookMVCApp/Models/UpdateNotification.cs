using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitBookMVCApp.Models
{
    public class UpdateNotification
    {
        public int UpdateNotificationId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime ContentDateTime { get; set; }

    }
}