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
                string[] keys = (string.IsNullOrEmpty(query) ? new string[0] : query.Split(" "));

                List<KeyValuePair<int, Template>> rank = new List<KeyValuePair<int, Template>>();
                foreach(Template t in context.Set<Template>())
                {
                    rank.Add(new KeyValuePair<int, Template>(keys.Sum(
                            key => (t.Keywords.ToLower().Contains(key.ToLower()) ? 1 : 0)
                                    + (t.Description.ToLower().Contains(key.ToLower()) ? 1 : 0)
                                    + (t.Title.ToLower().Contains(key.ToLower()) ? 1 : 0)), t));
                }

                rank.Sort((x, y) => y.Key.CompareTo(x.Key));
                this.Templates = rank.Where(l => l.Key > 0).Select(l => l.Value).ToList();

                return Page();
            }
        }
    }
}
