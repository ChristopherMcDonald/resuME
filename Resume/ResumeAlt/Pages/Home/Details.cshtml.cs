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

        public string Error { get; set; }

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

        public async Task<ActionResult> OnPostProject(string name, string supervisor, string company, DateTime start, DateTime end, string summary) 
        {
            this.Error = null;
            if(string.IsNullOrEmpty(name))
            {
                this.Error = "Project Name can't be empty.";
            }

            if (string.IsNullOrEmpty(supervisor))
            {
                this.Error = "Project Supervisor can't be empty.";
            }

            if (string.IsNullOrEmpty(company))
            {
                this.Error = "Project Company can't be empty.";
            }

            if (start.Equals(DateTime.MinValue))
            {
                this.Error = "Start Date is invalid.";
            }

            if (!DateTime.MinValue.Equals(end) && end.CompareTo(start) < 0)
            {
                this.Error = "End Date cannot be after Start Date.";
            }

            if (string.IsNullOrEmpty(summary))
            {
                this.Error = "Project Detail can't be empty.";
            }

            if (!string.IsNullOrEmpty(this.Error))
            {
                return OnGet();
            }

            this.CurrentUser = this.GetUser(HttpContext.User.Identity.Name);
            ProjectDetail projectDetail = new ProjectDetail()
            {
                ID = Guid.NewGuid(),
                Company = company,
                Supervisor = supervisor,
                Title = name,
                StartDate = start,
                Summary = summary
            };

            if (!DateTime.MinValue.Equals(end))
            {
                projectDetail.EndDate = end;
            }

            this.CurrentUser.ProjectDetails.Add(projectDetail);
            await _context.SaveChangesAsync();
            return Redirect("~/details");
        }

        public async Task<ActionResult> OnPostCert(string name, string issuer, DateTime start)
        {
            this.Error = null;
            if (string.IsNullOrEmpty(name))
            {
                this.Error = "Certificate Name can't be empty.";
            }

            if (string.IsNullOrEmpty(issuer))
            {
                this.Error = "Certificate Issuer can't be empty.";
            }

            if (start == null || start.CompareTo(DateTime.UtcNow) > 0)
            {
                this.Error = "Start Date is invalid.";
            }

            if (!string.IsNullOrEmpty(this.Error))
            {
                return OnGet();
            }

            this.CurrentUser = this.GetUser(HttpContext.User.Identity.Name);
            this.CurrentUser.CertDetails.Add(new CertDetail()
            {
                ID = Guid.NewGuid(),
                Name = name,
                Issuer = issuer,
                DateAchieved = start,
            });
            await _context.SaveChangesAsync();
            return Redirect("~/details");
        }

        public async Task<ActionResult> OnPostWork(string title, string location, string company, DateTime start, DateTime end, string summary)
        {
            this.Error = null;
            if (string.IsNullOrEmpty(title))
            {
                this.Error = "Work Title can't be empty.";
            }

            if (string.IsNullOrEmpty(location))
            {
                this.Error = "Work Location can't be empty.";
            }

            if (string.IsNullOrEmpty(company))
            {
                this.Error = "Project Company can't be empty.";
            }

            if (start.Equals(DateTime.MinValue))
            {
                this.Error = "Start Date is invalid.";
            }

            if (!DateTime.MinValue.Equals(end) && end.CompareTo(start) < 0)
            {
                this.Error = "End Date cannot be after Start Date.";
            }

            if (string.IsNullOrEmpty(summary))
            {
                this.Error = "Work summary can't be empty.";
            }

            if (!string.IsNullOrEmpty(this.Error))
            {
                return OnGet();
            }

            this.CurrentUser = this.GetUser(HttpContext.User.Identity.Name);
            WorkDetail workDetail = new WorkDetail()
            {
                Company = company,
                Location = location,
                Title = title,
                StartDate = start,
                Summary = summary
            };

            if (!DateTime.MinValue.Equals(end))
            {
                workDetail.EndDate = end;
            }

            this.CurrentUser.WorkDetails.Add(workDetail);
            await _context.SaveChangesAsync();
            return Redirect("~/details");
        }

        [HttpPost]
        public async Task<ActionResult> OnPostSkill(string name, string level, string skillClass)
        {
            this.Error = null;
            if (string.IsNullOrEmpty(name))
            {
                this.Error = "Skill Name can't be empty.";
            }

            if (string.IsNullOrEmpty(level))
            {
                this.Error = "Skill Level can't be empty.";
            }

            if (string.IsNullOrEmpty(skillClass))
            {
                this.Error = "Skill Class can't be empty.";
            }

            if (!string.IsNullOrEmpty(this.Error))
            {
                return OnGet();
            }

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
        public async Task<ActionResult> OnPostEducation(string schoolname, string degree, DateTime start, DateTime end, string achv, string gpa)
        {
            this.Error = null;
            if (string.IsNullOrEmpty(schoolname))
            {
                this.Error = "Education Name can't be empty.";
            }

            if (string.IsNullOrEmpty(degree))
            {
                this.Error = "Education Degree can't be empty.";
            }

            if (start.Equals(DateTime.MinValue))
            {
                this.Error = "Start Date is invalid.";
            }

            if (!DateTime.MinValue.Equals(end) && end.CompareTo(start) < 0)
            {
                this.Error = "End Date cannot be after Start Date.";
            }

            if (!float.TryParse(gpa, out float realGpa) || realGpa < 0)
            {
                this.Error = "GPA is invalid (only positive numbers are accepted)";
            }

            if (!string.IsNullOrEmpty(this.Error))
            {
                return OnGet();
            }

            this.CurrentUser = this.GetUser(HttpContext.User.Identity.Name);
            EducationDetail educationDetail = new EducationDetail()
            {
                SchoolName = schoolname,
                Degree = degree,
                Achievement = achv,
                StartDate = start,
                GPA = realGpa
            };

            if (!DateTime.MinValue.Equals(end))
            {
                educationDetail.EndDate = end;
            }

            this.CurrentUser.EducationDetails.Add(educationDetail);
            await _context.SaveChangesAsync();
            return Redirect("~/details");
        }

        private Models.User GetUser(string userEmail) {
            return _context.Set<User>().Where(entry => entry.Email.Equals(userEmail)).First();
        }
    }
}
