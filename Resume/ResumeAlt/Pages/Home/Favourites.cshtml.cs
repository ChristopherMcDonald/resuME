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

        public Models.User CurrentUser { get; set; }

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

            // TODO can this email be spoofed?
            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }
            else
            {
                this.CurrentUser = _context.Set<User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
                _context.Entry(this.CurrentUser).Collection(u => u.Favorites).Load();
                return Page();
            }
        }

        public async Task<ActionResult> OnPostDelete(string template) 
        {
            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }
            else
            {
                this.CurrentUser = _context.Set<Models.User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
                _context.Entry(this.CurrentUser).Collection(u => u.Favorites).Load();

                Favourite t = this.CurrentUser.Favorites.FirstOrDefault(fav => fav.TemplateId.Equals(new Guid(template)));

                if (t != null)
                {
                    _context.Remove(t);
                    await _context.SaveChangesAsync();
                }
                return Page();
            }
        }

        public string GetTitleFromId(Guid templateId) {
            return _context.Set<Template>().Where(entry => entry.ID.Equals(templateId)).First().Title;
        }
    }
}
