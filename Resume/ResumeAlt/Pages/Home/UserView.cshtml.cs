using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Resume.Models;

namespace Resume.Pages.Home
{
    public class UserViewModel : PageModel
    {
        private readonly Models.AppContext context;

        public User CurrentUser { get; set; }

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public UserViewModel(Models.AppContext app)
        {
            context = app;
            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create("Home", "/home"));
        }

        public ActionResult OnGet()
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
                context.Entry(this.CurrentUser).Collection(u => u.UserInfo).Load();
                return Page();
            }
        }

        public ActionResult OnPost(UserChangeRequest changeRequest)
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
                context.Entry(this.CurrentUser).Collection(u => u.UserInfo).Load();
                return OnGet();
            }
        }
    }

    public class UserChangeRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string AltEmail { get; set; }
        public string Phone { get; set; }
        public string Web { get; set; }
        public string NameExt { get; set; }
        public string Summary { get; set; }
    }
}
