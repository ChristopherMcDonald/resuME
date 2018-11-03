using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resume.Models
{
    public class TemplateHistory
    {
        private static TemplateHistory MinTime => new TemplateHistory() { UseDate = DateTime.MinValue };

        [Key]
        public int TemplateUseId { get; set; }
        public int TemplateId { get; set; }
        public int UserId { get; set; }
        public DateTime UseDate { get; set; }

        public static TemplateHistory MinOr(TemplateHistory that) => (that ?? MinTime);
    }
}
