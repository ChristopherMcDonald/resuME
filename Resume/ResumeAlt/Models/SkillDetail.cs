using System;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public class SkillDetail
    {
        [Key]
        public int ID { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Class { get; set; }
    }
}
