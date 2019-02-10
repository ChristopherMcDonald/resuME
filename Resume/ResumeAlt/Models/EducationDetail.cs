using System;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public class EducationDetail
    {
        [Key]
        public Guid ID { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string Degree { get; set; }
        public string Achievements { get; set; }
    }
}
