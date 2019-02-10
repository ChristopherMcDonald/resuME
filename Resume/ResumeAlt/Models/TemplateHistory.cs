using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resume.Models
{
    public class TemplateHistory
    {
        private static TemplateHistory MinTime => new TemplateHistory() { UseDate = DateTime.MinValue };

        [Key]
        public Guid TemplateUseId { get; set; }
        public Guid TemplateId { get; set; }
        public Guid UserId { get; set; }
        public DateTime UseDate { get; set; }
        public string GeneratedLink { get; set; }

        public static TemplateHistory MinOr(TemplateHistory that) => (that ?? MinTime);
    }
}
