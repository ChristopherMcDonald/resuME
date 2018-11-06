using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Resume.Models;

namespace Resume.Pages.Home
{
    public class FavouritesModel : PageModel
    {
        private readonly Models.AppContext _context;

        public User CurrentUser { get; set; }

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public FavouritesModel(Models.AppContext app)
        {
            this._context = app;
        }

        public ActionResult OnGet()
        {
            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create<string, string>("Home", "/home"));
            Breadcrumb.AddLast(Tuple.Create<string, string>("Favourites", null));

            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }
            else
            {
                // TODO does cookie have data integrity?
                this.CurrentUser = _context.Set<User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
                return Page();
            }
        }

        public ActionResult OnPostDelete(string template) 
        {
            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }
            else
            {
                this.CurrentUser = _context.Set<User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
                this.CurrentUser.Favorites = this.CurrentUser.Favorites.Where(fav => fav.TemplateId.Equals(Int32.Parse(template))).ToList();
                return Page();
            }
        }

        public string GetTitleFromId(int templateId) {
            return _context.Set<Template>().Where(entry => entry.ID.Equals(templateId)).First().Title;
        }
    }
}
