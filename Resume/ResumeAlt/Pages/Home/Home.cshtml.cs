using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Resume.Pages.Home
{
    public class HomeModel : PageModel
    {
        public string UserEmail { get; set; }

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public ActionResult OnGet()
        {
            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create<string,string>("Home", null));

            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail)) {
                return Redirect("~/login");
            } else {
                this.UserEmail = userEmail;
                return Page();
            }
        }
    }
}
