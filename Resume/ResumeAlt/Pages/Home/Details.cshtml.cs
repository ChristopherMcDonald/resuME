using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Resume.Models;

namespace Resume.Pages.Home
{
    public class DetailsModel : PageModel
    {
        private readonly Models.AppContext _context;

        public User CurrentUser { get; set; }

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public DetailsModel(Models.AppContext app) {
            this._context = app;

            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create<string, string>("Home", "/home"));
            Breadcrumb.AddLast(Tuple.Create<string, string>("Details", null));
        }

        public ActionResult OnGet()
        {
            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }
            else
            {
                // TODO does cookie have data integrity?
                this.CurrentUser = this.GetUser(HttpContext.User.Identity.Name);
                _context.Entry(this.CurrentUser).Collection(u => u.CertDetails).Load();
                _context.Entry(this.CurrentUser).Collection(u => u.SkillDetails).Load();
                _context.Entry(this.CurrentUser).Collection(u => u.WorkDetails).Load();
                _context.Entry(this.CurrentUser).Collection(u => u.ProjectDetails).Load();
                _context.Entry(this.CurrentUser).Collection(u => u.EducationDetails).Load();
                return Page();
            }
        }

        public async Task<ActionResult> OnPostProject(string name, string supervisor, string company, DateTime start, DateTime end, string detail) 
        {
            // TODO vlaidate and error handle
            this.CurrentUser = this.GetUser(HttpContext.User.Identity.Name);
            this.CurrentUser.ProjectDetails.Add(new ProjectDetail() {
                Company = company,
                Supervisor = supervisor,
                Title = name,
                StartDate = start,
                EndDate = end,
                Detail = detail
            });
            await _context.SaveChangesAsync();
            return Redirect("~/details");
        }

        public async Task<ActionResult> OnPostCert(string name, string issuer, DateTime start)
        {
            this.CurrentUser = this.GetUser(HttpContext.User.Identity.Name);
            this.CurrentUser.CertDetails.Add(new CertDetail()
            {
                Name = name,
                Issuer = issuer,
                DateAchieved = start,
            });
            await _context.SaveChangesAsync();
            return Redirect("~/details");
        }

        public async Task<ActionResult> OnPostWork(string title, string location, string company, DateTime start, DateTime end, string detail)
        {
            this.CurrentUser = this.GetUser(HttpContext.User.Identity.Name);
            this.CurrentUser.WorkDetails.Add(new WorkDetail()
            {
                Company = company,
                Location = location,
                Title = title,
                StartDate = start,
                EndDate = end,
                Detail = detail
            });
            await _context.SaveChangesAsync();
            return Redirect("~/details");
        }

        [HttpPost]
        public async Task<ActionResult> OnPostSkill(string name, string level, string skillClass)
        {
            this.CurrentUser = this.GetUser(HttpContext.User.Identity.Name);
            this.CurrentUser.SkillDetails.Add(new SkillDetail()
            {
                Name = name,
                Level = level,
                Class = skillClass
            });
            await _context.SaveChangesAsync();
            return Redirect("~/details");
        }

        [HttpPost]
        [ActionName("Education")]
        public async Task<ActionResult> OnPostEducation(string name, string degree, DateTime end, DateTime start, string achv)
        {
            this.CurrentUser = this.GetUser(HttpContext.User.Identity.Name);
            this.CurrentUser.EducationDetails.Add(new EducationDetail()
            {
                Name = name,
                Degree = degree,
                Achievements = achv,
                StartDate = start,
                EndDate = end,
            });
            await _context.SaveChangesAsync();
            return Redirect("~/details");
        }

        private Models.User GetUser(string userEmail) {
            return _context.Set<User>().Where(entry => entry.Email.Equals(userEmail)).First();
        }
    }
}
