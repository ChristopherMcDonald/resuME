using System;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    public partial class UserInfo
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string AltEmail { get; set; }
        public string Website { get; set; }
        public string Summary { get; set; }
        public string NameExt { get; set; }
    }
}
