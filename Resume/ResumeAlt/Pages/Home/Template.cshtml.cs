using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Resume.Controllers;
using Resume.Models;

namespace Resume.Pages.Home
{
    public class TemplateModel : PageModel
    {
        private readonly Models.AppContext context;
        private readonly AzureFileController azureFileController;

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public Template Template { get; set; }

        public Models.User CurrentUser { get; private set; }

        public string GetPreviewImageLink() => azureFileController.GetShareableImageLink(this.Template.PreviewImageLink);

        public TemplateModel(IOptions<Configuration.CloudStorage> cloudSettings, Models.AppContext app) {
            this.context = app;
            azureFileController = new AzureFileController(cloudSettings.Value.ConnectionString, cloudSettings.Value.ReadString);

            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create<string, string>("Home", "/home"));
            Breadcrumb.AddLast(Tuple.Create<string, string>("Templates", "/templates"));
        }

        public IActionResult OnGet(string id)
        {
            string userEmail = this.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Redirect("~/login");
            }
            else
            {
                this.CurrentUser = context.Set<Models.User>().Where(entry => entry.Email.Equals(userEmail)).FirstOrDefault();
                context.Entry(this.CurrentUser).Collection(u => u.Templates).Load();

                Template template = this.context.Set<Template>().Where(temp => temp.ID.Equals(new Guid(id))).FirstOrDefault();
                if (template == null || !this.CurrentUser.Templates.Any(t => t.ID.Equals(template.ID)))
                {
                    return Redirect("/templates");
                }


                this.Template = template;
                Breadcrumb.AddLast(Tuple.Create<string, string>(template.Title, null));
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(IFormFile image, IFormFile template, string guid, string title, string keywords, string description)
        {
            Template temp = this.context.Set<Template>().Where(t => t.DocumentLink.Equals(guid)).FirstOrDefault();
            if (temp == null)
            {
                return Redirect("/templates");
            }

            string toDeleteImage = null, toDeleteDocument = null;

            if(image != null) {
                    LocalFile localFile = LocalFile.Create(image);
                    await azureFileController.UploadFile(FileType.Images, localFile.LocalPath, localFile.Stream);
                    toDeleteImage = temp.PreviewImageLink;
                    temp.PreviewImageLink = Path.GetFileName(localFile.LocalPath);
            }

            if (template != null)
            {
                    LocalFile localFile = LocalFile.Create(template);
                    await azureFileController.UploadFile(FileType.Templates, localFile.LocalPath, localFile.Stream);
                    toDeleteDocument = temp.DocumentLink;
                    temp.DocumentLink = Path.GetFileName(localFile.LocalPath);
            }

            temp.Title = title;
            temp.Description = description;
            temp.Keywords = keywords;
            await context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(toDeleteDocument))
            {
                try
                {
                    await azureFileController.DeleteFile(FileType.Templates, toDeleteDocument);
                } catch (Exception e) {
                    System.Diagnostics.Trace.TraceError(string.Format("Deleting old Document failed. Guid: {0}. Stack: {1}", toDeleteDocument, e.StackTrace));
                }
            }

            if (!string.IsNullOrEmpty(toDeleteImage))
            {
                try
                {
                    await azureFileController.DeleteFile(FileType.Images, toDeleteImage);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.TraceError(string.Format("Deleting old Image failed. Guid: {0}. Stack: {1}", toDeleteImage, e.StackTrace));
                }
            }

            return OnGet(temp.ID.ToString());
        }
    }
}
