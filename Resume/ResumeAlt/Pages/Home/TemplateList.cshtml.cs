using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Resume.Controllers;
using Resume.Models;

namespace Resume.Pages.Home
{
    public class TemplateListModel : PageModel
    {
        private readonly Models.AppContext _context;
        private readonly AzureFileController azureFileController;

        private readonly List<string> acceptableImageExtensions = new List<string>()
        {
            ".png",
            ".jpg"
        };

        private readonly List<string> acceptableTemplateExtensions = new List<string>()
        {
            ".docx"
        };

        public User CurrentUser { get; set; }

        public string Error { get; set; }

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public TemplateListModel(IOptions<Configuration.CloudStorage> cloudSettings, Models.AppContext app) {
            _context = app;
            azureFileController = new AzureFileController(cloudSettings.Value.ConnectionString);
        }

        public ActionResult OnGet()
        {
            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create("Home", "/home"));
            Breadcrumb.AddLast(Tuple.Create<string, string>("Templates", null));

            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }
                
            this.CurrentUser = _context.Set<User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
            if (this.CurrentUser == null)
            {
                return Redirect("~/login");
            }
            else
            {
                _context.Entry(this.CurrentUser).Collection(u => u.Templates).Load();
                return Page();
            }
        }

        public async Task<IActionResult> OnPost(IFormFile image, IFormFile template, string title, string keywords, string description) {
            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }

            this.CurrentUser = _context.Set<User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
            if (this.CurrentUser == null)
            {
                return Redirect("~/login");
            }
            else
            {
                if (!acceptableImageExtensions.Contains(Path.GetExtension(image.FileName))) {
                    // TODO set and use error
                    this.Error = "";
                    return Page();
                } 

                if (!acceptableTemplateExtensions.Contains(Path.GetExtension(template.FileName)))
                {
                    // TODO set error
                    this.Error = "";
                    return Page();
                }

                // TODO catch errors, return useful error
                Template toAdd = new Template()
                {
                    UploadDate = DateTime.UtcNow,
                    Title = title,
                    UserID = this.CurrentUser.ID,
                    Keywords = keywords,
                    Description = description
                };

                using (LocalFile localFile = await LocalFile.Create(template)) {
                    // TODO try catch this
                    await azureFileController.UploadFile(FileType.Templates, localFile.LocalPath);
                    toAdd.DocumentLink = Path.GetFileName(localFile.LocalPath);
                }

                using (LocalFile localFile = await LocalFile.Create(image))
                {
                    // TODO try catch this
                    await azureFileController.UploadFile(FileType.Images, localFile.LocalPath);
                    toAdd.PreviewImageLink = Path.GetFileName(localFile.LocalPath);
                }

                this.CurrentUser.Templates.Add(toAdd);
                _context.SaveChangesAsync().Wait();

                // TODO return status
                return Redirect("~/templates");
            }
        }

        public async Task<IActionResult> OnPostDelete(string template) 
        {
            int templateId = int.Parse(template);
            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }

            this.CurrentUser = _context.Set<User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
            _context.Entry(this.CurrentUser).Collection(u => u.Templates).Load();

            Template toDelete = this.CurrentUser.Templates.FirstOrDefault(temp => temp.ID.Equals(templateId));
            if (toDelete != null) {
                this.CurrentUser.Templates.Remove(toDelete);
                int n = await _context.SaveChangesAsync();
                if (n > 0) {
                    await azureFileController.DeleteFile(FileType.Images, toDelete.PreviewImageLink);
                    await azureFileController.DeleteFile(FileType.Templates, toDelete.DocumentLink);
                }
            }

            return Redirect("~/templates");
        }

        public async Task<IActionResult> OnPostDownload(string link) 
        {
            Stream s = await azureFileController.GetFileAsStream(FileType.Templates, link);
            return File(s, "application/octet-stream", string.Format("Template-{0}.docx", link));
        }
    }
}
