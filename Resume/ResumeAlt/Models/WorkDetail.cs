using System;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public class WorkDetail
    {
        [Key]
        public Guid ID { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        public string Detail { get; set; }
    }
}
