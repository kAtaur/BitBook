
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitBookMVCApp.Models
{
    public class PostComment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public string UserComment { get; set; }
        //public DateTime CommentDate { get; set; }
    }
}