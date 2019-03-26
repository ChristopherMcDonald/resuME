using System;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public class WorkDetailExtended
    {
        [Key]
        public Guid ID { get; set; }
        public Guid DetailId { get; set; }
        public string Item { get; set; }
    }
}
