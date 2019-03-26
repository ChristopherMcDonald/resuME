using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public class EducationDetail : Detail
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool EndDateTentative { get; set; }
        public string SchoolName { get; set; }
        public string Degree { get; set; }
        public string Achievement { get; set; }
        public float GPA { get; set; }
    }
}
