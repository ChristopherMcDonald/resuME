using System;
namespace Resume.Models
{
    public class CashHistory
    {
        public CashHistory()
        {
        }

        public int ID { get; set; }
        public int UserId { get; set; }
        public DateTime Event { get; set; }
        public int TemplateUseId { get; set; }
        public string Action { get; set; }
        public float Amount { get; set; }
    }
}
