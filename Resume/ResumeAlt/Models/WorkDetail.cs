using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public class WorkDetail : Detail
    {
        public WorkDetail()
        {
            this.Bullets = new HashSet<WorkDetailExtended>();
        }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool EndDateTentative { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        public string Summary { get; set; }

        public HashSet<WorkDetailExtended> Bullets { get; set; }
    }
}
