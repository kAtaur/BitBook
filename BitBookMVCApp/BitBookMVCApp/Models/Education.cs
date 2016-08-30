using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BitBookMVCApp.Models
{
    public class Education
    {
        public int EducationId { get; set; }

        public string UserId { get; set; }
        [Required(ErrorMessage = "Please provide Educatoin Title")]
        public string EduTitle { get; set; }
        [Required(ErrorMessage = "Please provide Institute")]

        public string EduInstitute { get; set; }
        [Required(ErrorMessage = "Please provide Passing Year")]
        public string EduYear { get; set; }
    }
}