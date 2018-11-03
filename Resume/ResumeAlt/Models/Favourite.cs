using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resume.Models
{
    public class Favourite
    {
        public int UserId { get; set; }

        public int TemplateId { get; set; }
    }
}
