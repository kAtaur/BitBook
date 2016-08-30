using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitBookMVCApp.Models
{
    public class ViewPostComment
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string Name { get; set; }
        public int PostId { get; set; }
        public List<ViewCommentwithUser> Ucomment { get; set; }
        public int TotalComment { get; set; }
    }
}