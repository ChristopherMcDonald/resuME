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

        public User CurrentUser { get; set; }

        public Template Template { get; set; }

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public TemplateUseModel(IOptions<Configuration.CloudStorage> cloudSettings, Models.AppContext app)
        {
            context = app;
            azureFileController = new AzureFileController(cloudSettings.Value.ConnectionString);
        }

        public ActionResult OnGet(string id, string query)
        {
            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create("Home", "/home"));
            if (!string.IsNullOrEmpty(query))
            {
                Breadcrumb.AddLast(Tuple.Create<string, string>("Search", "/search?query=" + query));
            }

            Breadcrumb.AddLast(Tuple.Create<string, string>(id, null));

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
                return Page();
            }
        }

        public async Task<ActionResult> OnPostFavorite(string id) 
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
                this.CurrentUser.Favorites.Add(new Favourite()
                {
                    TemplateId = new Guid(id),
                    UserId = this.CurrentUser.ID
                });

                await context.SaveChangesAsync();
                return OnGet(id, "");
            }
        }

        public ActionResult OnPostUse(string id, List<int> ids)
        {
            foreach(string key in this.Request.Form.Keys)
            {
                // if key is GUID, it is probs a field to include
            }
            return OnGet(id, "");
        }
    }
}
