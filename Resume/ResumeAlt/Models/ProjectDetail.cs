using System;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public class ProjectDetail : Detail
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool EndDateTentative { get; set; }
        public string Title { get; set; }
        public string Supervisor { get; set; }
        public string Company { get; set; }
        public string Summary { get; set; }
    }
}
