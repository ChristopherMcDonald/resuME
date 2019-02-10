using System;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public class CertDetail
    {
        [Key]
        public Guid ID { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateAchieved { get; set; }
        public string Name { get; set; }
        public string Issuer { get; set; }
    }
}
