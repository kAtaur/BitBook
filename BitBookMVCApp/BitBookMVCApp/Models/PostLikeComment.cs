using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitBookMVCApp.Models
{
    public class PostLikeComment
    {
        public string UserId { get; set; }
        public int PostId { get; set; }

        public string UserName { get; set; }
        
        public string TextPost { get; set; }

        public string ImgPost { get; set; }
        public DateTime PostDate { get; set; }

        public int LikeCountWithUser { get; set; }

        public int LikeCountWithoutUser { get; set; }

        public int TotalLikeCount { get; set; }

        public int LikeStatus { get; set; }

        public int LikeCountWithComment { get; set; }

        public int LikeCountWithoutComment { get; set; }
    }
}