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
    public class UserViewModel : PageModel
    {
        private readonly Models.AppContext context;

        public Resume.Models.User CurrentUser { get; set; }

        public string Error { get; set; }

        public string Success { get; set; }

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
            this.Success = string.Empty;
            this.Error = string.Empty;
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

        public async Task<ActionResult> OnPost(UserChangeRequest changeRequest)
        {
            this.Success = string.Empty;
            this.Error = string.Empty;
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

                string res = string.Empty;

                if (!string.IsNullOrEmpty(changeRequest.AltEmail) && !DataValidator.Email(changeRequest.AltEmail, out res))
                {
                    this.Error = res;
                    return Page();
                }

                if (!DataValidator.FirstName(changeRequest.FirstName, out res))
                {
                    this.Error = res;
                    return Page();
                }

                if (!DataValidator.LastName(changeRequest.LastName, out res))
                {
                    this.Error = res;
                    return Page();
                }

                if (!string.IsNullOrEmpty(changeRequest.Phone) && !DataValidator.PhoneNumber(changeRequest.Phone, out res))
                {
                    this.Error = res;
                    return Page();
                }

                if (!string.IsNullOrEmpty(changeRequest.Web) && !DataValidator.Website(changeRequest.Web, out res))
                {
                    this.Error = res;
                    return Page();
                }

                if (!string.IsNullOrEmpty(changeRequest.NewPassword))
                {
                    if (!DataValidator.Password(changeRequest.NewPassword, out res))
                    {
                        this.Error = res;
                        return Page();
                    }

                    if (PasswordHelper.Check(this.CurrentUser, changeRequest.OldPassword))
                    {
                        this.CurrentUser.PasswordHash = PasswordHelper.Hash(changeRequest.NewPassword);
                    }
                    else
                    {
                        this.Error = "Old password provided is not correct.";
                        return Page();
                    }
                }

                this.CurrentUser.FirstName = changeRequest.FirstName;
                this.CurrentUser.LastName = changeRequest.LastName;

                if (!this.CurrentUser.UserInfo.Any())
                {
                    UserInfo info = new UserInfo()
                    {
                        Id = Guid.NewGuid(),
                        UserId = this.CurrentUser.ID,
                        AltEmail = changeRequest.AltEmail,
                        NameExt = changeRequest.NameExt,
                        PhoneNumber = changeRequest.Phone,
                        Summary = changeRequest.Summary,
                        Website = changeRequest.Web
                    };

                    this.CurrentUser.UserInfo.Add(info);
                }
                else
                {
                    this.CurrentUser.UserInfo.First().AltEmail = changeRequest.AltEmail;
                    this.CurrentUser.UserInfo.First().NameExt = changeRequest.NameExt;
                    this.CurrentUser.UserInfo.First().PhoneNumber = changeRequest.Phone;
                    this.CurrentUser.UserInfo.First().Summary = changeRequest.Summary;
                    this.CurrentUser.UserInfo.First().Website = changeRequest.Web;
                }

                this.Success = "Changes have been save successfully.";
                await context.SaveChangesAsync();
                return Page();
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
