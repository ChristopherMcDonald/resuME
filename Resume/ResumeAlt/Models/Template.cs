using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Resume.Models
{
    public class Template
    {
        public Template()
        {
            this.History = new HashSet<TemplateHistory>();
            this.Favourites = new HashSet<Favourite>();
        }

        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string DocumentLink { get; set; }
        public string Title { get; set; }
        public string PreviewImageLink { get; set; }
        public DateTime UploadDate { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }

        public ICollection<TemplateHistory> History { get; set; }
        public ICollection<Favourite> Favourites { get; set; }
 
        [NotMapped]
        public int Uses => History.Count;

        [NotMapped]
        public DateTime LastUsed => TemplateHistory.MinOr(History.OrderBy((arg) => arg.UseDate).LastOrDefault()).UseDate;
    }
}
