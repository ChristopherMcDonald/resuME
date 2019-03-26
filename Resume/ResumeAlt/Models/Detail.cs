using System;
using System.ComponentModel.DataAnnotations;

namespace Resume.Models
{
    /// <summary>
    /// Functions simply as a superclass for all other detail
    /// </summary>
    public abstract class Detail 
    {
        [Key]
        public Guid ID { get; set; }
        public Guid UserId { get; set; }
    }
}
