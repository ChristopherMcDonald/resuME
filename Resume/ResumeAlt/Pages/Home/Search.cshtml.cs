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
    public class SearchModel : PageModel
    {
        private readonly Models.AppContext context;
        public AzureFileController azureFileController;

        public User CurrentUser { get; set; }

        public List<Template> Templates { get; set; }

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public SearchModel(IOptions<Configuration.CloudStorage> cloudSettings, Models.AppContext app)
        {
            context = app;
            azureFileController = new AzureFileController(cloudSettings.Value.ConnectionString);
        }

        public ActionResult OnGet(string query)
        {
            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create("Home", "/home"));
            Breadcrumb.AddLast(Tuple.Create<string, string>("Search", null));

            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }

            this.CurrentUser = context.Set<User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
            if (this.CurrentUser == null)
            {
                return Redirect("~/login");
            }
            else 
            {
                string[] keys = query.Split(" ");
                // searches whether anyword
                // TODO make search better
                this.Templates = context.Set<Template>()
                                        .Where(t => 
                                               keys.Select(key => t.Keywords.Contains(key)).Any()
                                                || keys.Select(key => t.Description.Contains(key)).Any()
                                                || keys.Select(key => t.Title.Contains(key)).Any()
                                              ).ToList();
                return Page();
            }
        }
    }
}
