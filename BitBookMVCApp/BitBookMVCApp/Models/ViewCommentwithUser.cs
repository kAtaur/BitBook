using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitBookMVCApp.Models
{
    public class ViewCommentwithUser
    {
        public string CommentatorId  { get; set; }
        public string UserName { get; set; }

        public string Comment{ get; set; }
    }
}