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
            this.UserInfo = new HashSet<UserInfo>();

            this.Templates = new HashSet<Template>();
            this.Transactions = new HashSet<CashHistory>();
            this.Favorites = new HashSet<Favourite>();
            this.TemplateUseHistory = new HashSet<TemplateHistory>();

            // User Details
            this.WorkDetails = new HashSet<WorkDetail>();
            this.ProjectDetails = new HashSet<ProjectDetail>();
            this.CertDetails = new HashSet<CertDetail>();
            this.EducationDetails = new HashSet<EducationDetail>();
            this.SkillDetails = new HashSet<SkillDetail>();
        }

        [Key]
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public double AvailableCash { get; set; }
        public string PasswordHash { get; set; }
        public Guid VerifyString { get; set; }
        public bool Verified { get; set; }

        public ICollection<UserInfo> UserInfo { get; set; }

        public ICollection<Template> Templates { get; set; }
        public ICollection<CashHistory> Transactions { get; set; }
        public ICollection<Favourite> Favorites { get; set; }
        public ICollection<TemplateHistory> TemplateUseHistory { get; set; }

        // User Details
        public ICollection<WorkDetail> WorkDetails { get; set; }
        public ICollection<ProjectDetail> ProjectDetails { get; set; }
        public ICollection<CertDetail> CertDetails { get; set; }
        public ICollection<EducationDetail> EducationDetails { get; set; }
        public ICollection<SkillDetail> SkillDetails { get; set; }
    }
}
