using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitBookMVCApp.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }

        public string Contact{ get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string AreaOfInterest { get; set; }

        public string ProfilePhoto { get; set; }

        public string CoverPhoto { get; set; }

    }
}