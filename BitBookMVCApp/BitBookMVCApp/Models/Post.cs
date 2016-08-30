using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BitBookMVCApp.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        
        public string TextPost { get; set; }

        public string ImgPost { get; set; }
        public DateTime PostDate { get; set; }
    }
}