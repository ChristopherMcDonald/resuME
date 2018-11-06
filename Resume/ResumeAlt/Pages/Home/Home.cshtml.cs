using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Resume.Models;

namespace Resume.Pages.Home
{
    public class HomeModel : PageModel
    {
        private readonly Models.AppContext _context;

        public User CurrentUser { get; set; }

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public HomeModel(Models.AppContext app) {
            this._context = app;
        }

        public ActionResult OnGet()
        {
            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create<string,string>("Home", null));

            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail)) {
                return Redirect("~/login");
            } else {
                this.CurrentUser = _context.Set<User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
                return Page();
            }
        }

        public string GetTitleFromId(int templateId)
        {
            return _context.Set<Template>().Where(entry => entry.ID.Equals(templateId)).First().Title;
        }
    }
}
