using System;
namespace Resume.Models
{
    public class UserDetail
    {
        public UserDetail()
        {
        }

        public int ID { get; set;}
        public int UserID { get; set; }
        public string Type { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Detail { get; set; }
    }
}
