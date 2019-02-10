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
                if (!acceptableImageExtensions.Contains(Path.GetExtension(image.FileName)))
                {
                    this.Error = "Only JPG and PNG images can be uploaded at this time.";
                    return Page();
                } 

                if (!acceptableTemplateExtensions.Contains(Path.GetExtension(template.FileName)))
                {
                    this.Error = "Only DOCX files can be uploaded at this time.";
                    return Page();
                }

                Template toAdd = new Template()
                {
                    UploadDate = DateTime.UtcNow,
                    Title = title,
                    UserID = this.CurrentUser.ID,
                    Keywords = keywords,
                    Description = description
                };

                LocalFile localFile = LocalFile.Create(template);
                try
                {
                    await azureFileController.UploadFile(FileType.Templates, localFile.LocalPath, localFile.Stream);
                }
                catch (Exception e)
                {
                    this.Error = "Something bad happened during uploading... Please try again later.";
                    System.Diagnostics.Trace.TraceError("Error during uploading:" + e.ToString());
                    await azureFileController.DeleteFile(FileType.Templates, Path.GetFileName(localFile.LocalPath));
                    return Page();
                }
                toAdd.DocumentLink = Path.GetFileName(localFile.LocalPath);
                
                LocalFile localFile2 = LocalFile.Create(image);
                try
                {
                    await azureFileController.UploadFile(FileType.Images, localFile2.LocalPath, localFile2.Stream);
                }
                catch (Exception e)
                {
                    this.Error = "Something bad happened during uploading... Please try again later.";
                    System.Diagnostics.Trace.TraceError("Error during uploading:" + e.ToString());

                    // won't save record, need to clean up AzureFileStorage
                    await azureFileController.DeleteFile(FileType.Images, Path.GetFileName(localFile2.LocalPath));
                    await azureFileController.DeleteFile(FileType.Templates, toAdd.DocumentLink);

                    return Page();
                }
                toAdd.PreviewImageLink = Path.GetFileName(localFile2.LocalPath);

                this.CurrentUser.Templates.Add(toAdd);
                _context.SaveChangesAsync().Wait();

                return Redirect("~/templates");
            }
        }

        public async Task<IActionResult> OnPostDelete(string template) 
        {
            Guid templateId = new Guid(template);
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
