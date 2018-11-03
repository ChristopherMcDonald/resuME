using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Resume.Models
{
    public partial class User
    {
        public User()
        {
            this.Templates = new HashSet<Template>();
            this.Details = new HashSet<UserDetail>();
            this.Transactions = new HashSet<CashHistory>();
            this.Favorites = new HashSet<Favourite>();
        }

        [Key]
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public double AvailableCash { get; set; }
        public string PasswordHash { get; set; }
        public string VerifyString { get; set; }
        public bool Verified { get; set; }

        public ICollection<Template> Templates { get; set; }
        public ICollection<UserDetail> Details { get; set; }
        public ICollection<CashHistory> Transactions { get; set; }
        public ICollection<Favourite> Favorites { get; set; }
    }
}
