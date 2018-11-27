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
        public string GetPreviewImageLink() => azureFileController.GetShareableImageLink(this.Template.PreviewImageLink);

        public TemplateModel(IOptions<Configuration.CloudStorage> cloudSettings, Models.AppContext app) {
            this.context = app;
            azureFileController = new AzureFileController(cloudSettings.Value.ConnectionString);

            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create<string, string>("Home", "/home"));
            Breadcrumb.AddLast(Tuple.Create<string, string>("Templates", "/templates"));
        }

        public IActionResult OnGet(int id)
        {
            Template template = this.context.Set<Template>().Where(temp => temp.ID.Equals(id)).FirstOrDefault();
            if (template == null) {
                return Redirect("/templates");
            }

            this.Template = template;
            Breadcrumb.AddLast(Tuple.Create<string, string>(template.Title, null));
            return Page();
        }

        public async Task<IActionResult> OnPost(IFormFile image, IFormFile template, string guid, string title, string keywords, string description)
        {
            Template temp = this.context.Set<Template>().Where(t => t.DocumentLink.Equals(guid)).FirstOrDefault();
            if (temp == null)
            {
                return Redirect("/templates");
            }

            if(image != null) {
                using(LocalFile localFile = await LocalFile.Create(image)) {
                    await azureFileController.UploadFile(FileType.Images, localFile.LocalPath);
                    temp.PreviewImageLink = Path.GetFileName(localFile.LocalPath);
                }
            }

            if (template != null)
            {
                using (LocalFile localFile = await LocalFile.Create(template))
                {
                    await azureFileController.UploadFile(FileType.Templates, localFile.LocalPath);
                    temp.DocumentLink = Path.GetFileName(localFile.LocalPath);
                }
            }

            temp.Title = title;
            temp.Description = description;
            temp.Keywords = keywords;
            await context.SaveChangesAsync();
            return OnGet(temp.ID);
        }
    }
}
