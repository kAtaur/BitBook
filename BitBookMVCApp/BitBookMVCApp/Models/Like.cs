using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitBookMVCApp.Models
{
    public class Like
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public int Status { get; set; } 
        
    }
}