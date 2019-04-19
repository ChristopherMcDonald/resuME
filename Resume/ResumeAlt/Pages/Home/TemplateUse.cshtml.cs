using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Resume.Controllers;
using Resume.Models;
using TemplateEngine.Docx;

namespace Resume.Pages.Home
{
    public class TemplateUseModel : PageModel
    {
        private readonly Models.AppContext context;
        public AzureFileController azureFileController;

        public Models.User CurrentUser { get; set; }

        public Template Template { get; set; }

        /// <summary>
        /// List of (Links,LinkText)
        /// </summary>
        public LinkedList<Tuple<string, string>> Breadcrumb;

        public TemplateUseModel(IOptions<Configuration.CloudStorage> cloudSettings, Models.AppContext app)
        {
            context = app;
            azureFileController = new AzureFileController(cloudSettings.Value.ConnectionString, cloudSettings.Value.ReadString);
        }

        public ActionResult OnGet(string id, string query)
        {
            this.Breadcrumb = new LinkedList<Tuple<string, string>>();
            Breadcrumb.AddLast(Tuple.Create("Home", "/home"));
            if (!string.IsNullOrEmpty(query))
            {
                Breadcrumb.AddLast(Tuple.Create<string, string>("Search", "/search?query=" + query));
            }

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
                context.Entry(this.CurrentUser).Collection(u => u.Favorites).Load();
                context.Entry(this.CurrentUser).Collection(u => u.CertDetails).Load();
                context.Entry(this.CurrentUser).Collection(u => u.EducationDetails).Load();
                context.Entry(this.CurrentUser).Collection(u => u.ProjectDetails).Load();
                context.Entry(this.CurrentUser).Collection(u => u.SkillDetails).Load();
                context.Entry(this.CurrentUser).Collection(u => u.WorkDetails).Load();

                this.Template = context.Set<Template>().Where(t => t.ID.Equals(new Guid(id))).FirstOrDefault();
                Breadcrumb.AddLast(Tuple.Create<string, string>(this.Template.Title, null));
                return Page();
            }
        }

        public async Task<ActionResult> OnPostFavorite(string id, string query) 
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
                context.Entry(this.CurrentUser).Collection(u => u.Favorites).Load();
                if (this.CurrentUser.Favorites.Any(fav => fav.TemplateId.Equals(new Guid(id))))
                {
                    // remove it
                    this.CurrentUser.Favorites = this.CurrentUser.Favorites.Where(fav => !fav.TemplateId.Equals(new Guid(id))).ToList();
                    context.Update(this.CurrentUser);
                }
                else
                {
                    // add it
                    this.CurrentUser.Favorites.Add(new Favourite()
                    {
                        TemplateId = new Guid(id),
                        UserId = this.CurrentUser.ID
                    });
                }

                await context.SaveChangesAsync();

                return OnGet(id, query);
            }
        }

        public async Task<ActionResult> OnPostUse()
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

            Dictionary<string, List<object>> subsetDetails = new Dictionary<string, List<object>>()
            {
                {"Edu", new List<object>()},
                {"Cert", new List<object>()},
                {"Work", new List<object>()},
                {"Project", new List<object>()},
                {"Skill", new List<object>()},
            };

            context.Entry(this.CurrentUser).Collection(u => u.UserInfo).Load();
            context.Entry(this.CurrentUser).Collection(u => u.CertDetails).Load();
            context.Entry(this.CurrentUser).Collection(u => u.EducationDetails).Load();
            context.Entry(this.CurrentUser).Collection(u => u.ProjectDetails).Load();
            context.Entry(this.CurrentUser).Collection(u => u.SkillDetails).Load();
            context.Entry(this.CurrentUser).Collection(u => u.WorkDetails).Load();

            foreach (string type in subsetDetails.Keys)
            {
                int i = 0;
                while (true)
                {
                    // this will be null or GUID for type "type"
                    var str = this.Request.Form[type + i].ToString();

                    if (string.IsNullOrEmpty(str))
                    {
                        break;
                    }
                    else
                    {
                        foreach (string s in str.Split(","))
                        {
                            object addedDetail = null;
                            switch (type)
                            {
                                case "Edu":
                                    addedDetail = this.CurrentUser.EducationDetails.FirstOrDefault(detail => detail.ID.ToString().Equals(s));
                                    break;
                                case "Cert":
                                    addedDetail = this.CurrentUser.CertDetails.FirstOrDefault(detail => detail.ID.ToString().Equals(s));
                                    break;
                                case "Work":
                                    addedDetail = this.CurrentUser.WorkDetails.FirstOrDefault(detail => detail.ID.ToString().Equals(s));
                                    break;
                                case "Project":
                                    addedDetail = this.CurrentUser.ProjectDetails.FirstOrDefault(detail => detail.ID.ToString().Equals(s));
                                    break;
                                case "Skill":
                                    addedDetail = this.CurrentUser.SkillDetails.FirstOrDefault(detail => detail.ID.ToString().Equals(s));
                                    break;
                            }

                            if (addedDetail != null)
                            {
                                subsetDetails[type].Add(addedDetail);
                            }
                        }
                    }
                    i++;
                }
            }

            Template template = context
                .Set<Models.Template>()
                .Where(temp => temp.ID.ToString().Equals(this.Request.Form["templateId"].ToString()))
                .FirstOrDefault();

            if (template == null)
            {
                return Redirect("~/home");
            }

            using (TemplateFile tf = await TemplateFile.Create(this.azureFileController, this.CurrentUser, template.DocumentLink, subsetDetails))
            {
                // TODO make async, upload to Azure
                // TODO write to TemplateUse table
                return File(System.IO.File.ReadAllBytes(tf.LocalFile), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Template.docx");
            }
        }
    }
}
