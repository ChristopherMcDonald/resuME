using System;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public class CertDetail : Detail
    {
        public DateTime DateAchieved { get; set; }
        public string Name { get; set; }
        public string Issuer { get; set; }
    }
}
