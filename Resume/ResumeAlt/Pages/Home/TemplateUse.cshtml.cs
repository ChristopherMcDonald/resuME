using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Resume.Controllers;
using Resume.Models;

namespace Resume.Pages.Home
{
    public class TemplateUseModel : PageModel
    {
        private readonly Models.AppContext context;
        public AzureFileController azureFileController;

        public Models.User CurrentUser { get; set; }

        public Template Template { get; set; }

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public TemplateUseModel(IOptions<Configuration.CloudStorage> cloudSettings, Models.AppContext app)
        {
            context = app;
            azureFileController = new AzureFileController(cloudSettings.Value.ConnectionString, cloudSettings.Value.ReadString);
        }

        public ActionResult OnGet(string id, string query)
        {
            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create("Home", "/home"));
            if (!string.IsNullOrEmpty(query))
            {
                Breadcrumb.AddLast(Tuple.Create<string, string>("Search", "/search?query=" + query));
            }

            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }

            this.CurrentUser = context.Set<Models.User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
            if (this.CurrentUser == null)
            {
                return Redirect("~/login");
            }
            else
            {
                context.Entry(this.CurrentUser).Collection(u => u.Favorites).Load();
                context.Entry(this.CurrentUser).Collection(u => u.CertDetails).Load();
                context.Entry(this.CurrentUser).Collection(u => u.EducationDetails).Load();
                context.Entry(this.CurrentUser).Collection(u => u.ProjectDetails).Load();
                context.Entry(this.CurrentUser).Collection(u => u.SkillDetails).Load();
                context.Entry(this.CurrentUser).Collection(u => u.WorkDetails).Load();

                this.Template = context.Set<Template>().Where(t => t.ID.Equals(new Guid(id))).FirstOrDefault();
                Breadcrumb.AddLast(Tuple.Create<string, string>(this.Template.Title, null));
                return Page();
            }
        }

        public async Task<ActionResult> OnPostFavorite(string id, string query) 
        {
            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }

            this.CurrentUser = context.Set<Models.User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
            if (this.CurrentUser == null)
            {
                return Redirect("~/login");
            }
            else
            {
                context.Entry(this.CurrentUser).Collection(u => u.Favorites).Load();
                if (this.CurrentUser.Favorites.Any(fav => fav.TemplateId.Equals(new Guid(id))))
                {
                    // remove it
                    this.CurrentUser.Favorites = this.CurrentUser.Favorites.Where(fav => !fav.TemplateId.Equals(new Guid(id))).ToList();
                    context.Update(this.CurrentUser);
                }
                else
                {
                    // add it
                    this.CurrentUser.Favorites.Add(new Favourite()
                    {
                        TemplateId = new Guid(id),
                        UserId = this.CurrentUser.ID
                    });
                }

                await context.SaveChangesAsync();

                return OnGet(id, query);
            }
        }

        public ActionResult OnPostUse()
        {
            Dictionary<string, List<Detail>> details = new Dictionary<string, List<Detail>>() 
            {
                {"Edu", new List<Detail>()},
                {"Cert", new List<Detail>()},
                {"Work", new List<Detail>()},
                {"Project", new List<Detail>()},
                {"Skill", new List<Detail>()},
            };

            context.Entry(this.CurrentUser).Collection(u => u.UserInfo).Load();
            context.Entry(this.CurrentUser).Collection(u => u.CertDetails).Load();
            context.Entry(this.CurrentUser).Collection(u => u.EducationDetails).Load();
            context.Entry(this.CurrentUser).Collection(u => u.ProjectDetails).Load();
            context.Entry(this.CurrentUser).Collection(u => u.SkillDetails).Load();
            context.Entry(this.CurrentUser).Collection(u => u.WorkDetails).Load();

            Dictionary<string, object> userDetails = new Dictionary<string, object>()
            {
                {"Edu", this.CurrentUser.EducationDetails},
                {"Cert", this.CurrentUser.CertDetails},
                {"Work", this.CurrentUser.WorkDetails},
                {"Project", this.CurrentUser.ProjectDetails},
                {"Skill", this.CurrentUser.SkillDetails},
            };

            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }

            this.CurrentUser = context.Set<Models.User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
            if (this.CurrentUser == null)
            {
                return Redirect("~/login");
            }

            foreach (string type in details.Keys)
            {
                while(true)
                {
                    int i = 0;
                    this.Request.Form[type + i];
                    i++;
                }
            }
            // use this.CurrentUser and info to generate and download template

            return OnGet(this.CurrentUser.ID.ToString(), string.Empty);
        }
    }
}
