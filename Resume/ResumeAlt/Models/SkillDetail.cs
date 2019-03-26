using System;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public class SkillDetail : Detail
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public string Class { get; set; }
    }
}
