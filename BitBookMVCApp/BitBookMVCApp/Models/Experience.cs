using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BitBookMVCApp.Models
{
    public class Experience
    {
        public int ExperienceId { get; set; }

        public string UserId { get; set; }
        [Required(ErrorMessage = "Please provide Designation")]
        public string ExpDesignation { get; set; }
        [Required(ErrorMessage = "Please provide Organization")]
        public string ExpCompany { get; set; }
        [Required(ErrorMessage = "Please provide Experience")]
        public string ExpYear { get; set; }
    }
}