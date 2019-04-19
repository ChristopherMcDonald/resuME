using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Resume.Models;
using TemplateEngine.Docx;

namespace Resume.Controllers
{
    public class TemplateFile : IDisposable
    {
        public string LocalFile { get; private set; }

        public static async Task<TemplateFile> Create(AzureFileController azureFileController, User user, string documentLink, Dictionary<string, List<object>> subsetDetails)
        {
            var valuesToFill = new Content(
                new FieldContent("Initials", user.FirstName.ToCharArray()[0].ToString() + user.LastName.ToCharArray()[0].ToString() ),
                new FieldContent("FirstName", user.FirstName),
                new FieldContent("LastName", user.LastName),
                new FieldContent("Email", user.UserInfo?.FirstOrDefault()?.AltEmail ?? user.Email),
                new TableContent("EducationTable"),
                new TableContent("CertTable"),
                new TableContent("WorkTable"),
                new TableContent("ProjectTable"),
                new TableContent("SkillTable"));

            subsetDetails["Edu"].ForEach(obj => 
            {
                EducationDetail detail = (EducationDetail)obj;
                valuesToFill
                    .Tables
                    .FirstOrDefault(table => table.Name.Equals("EducationTable"))
                    .AddRow(new FieldContent("SchoolName", detail.SchoolName),
                            new FieldContent("Achievement", detail.Achievement),
                            new FieldContent("Degree", detail.Degree),
                            new FieldContent("EndDate", detail.EndDateTentative ? "Current" : detail.EndDate?.ToShortDateString()),
                            new FieldContent("StartDate", detail.StartDate.ToShortDateString()),
                            new FieldContent("GPA", detail.GPA.ToString()));
            });

            subsetDetails["Cert"].ForEach(obj =>
            {
                CertDetail detail = (CertDetail)obj;
                valuesToFill
                    .Tables
                    .FirstOrDefault(table => table.Name.Equals("CertTable"))
                    .AddRow(new FieldContent("Issuer", detail.Issuer),
                            new FieldContent("Name", detail.Name),
                            new FieldContent("DateAchieved", detail.DateAchieved.ToShortDateString()));
            });

            subsetDetails["Work"].ForEach(obj =>
            {
                WorkDetail detail = (WorkDetail)obj;
                valuesToFill
                    .Tables
                    .FirstOrDefault(table => table.Name.Equals("WorkTable"))
                    .AddRow(new FieldContent("Company", detail.Company),
                            new FieldContent("Location", detail.Location),
                            new FieldContent("Summary", detail.Summary),
                            new FieldContent("EndDate", detail.EndDateTentative ? "Current" : detail.EndDate?.ToShortDateString()),
                            new FieldContent("StartDate", detail.StartDate.ToShortDateString()),
                            new FieldContent("Title", detail.Title.ToString()));

                // TODO add bullets
            });

            subsetDetails["Project"].ForEach(obj =>
            {
                ProjectDetail detail = (ProjectDetail)obj;
                valuesToFill
                    .Tables
                    .FirstOrDefault(table => table.Name.Equals("ProjectTable"))
                    .AddRow(new FieldContent("Company", detail.Company),
                            new FieldContent("Supervisor", detail.Supervisor),
                            new FieldContent("Summary", detail.Summary),
                            new FieldContent("EndDate", detail.EndDateTentative ? "Current" : detail.EndDate?.ToShortDateString()),
                            new FieldContent("StartDate", detail.StartDate.ToShortDateString()),
                            new FieldContent("Title", detail.Title.ToString()));
            });

            subsetDetails["Skill"].ForEach(obj =>
            {
                SkillDetail detail = (SkillDetail)obj;
                valuesToFill
                    .Tables
                    .FirstOrDefault(table => table.Name.Equals("SkillTable"))
                    .AddRow(new FieldContent("Class", detail.Class),
                            new FieldContent("Name", detail.Name),
                            new FieldContent("Level", detail.Level));
            });

            if (user.UserInfo.Any())
            {
                UserInfo info = user.UserInfo.FirstOrDefault();

                if (!string.IsNullOrEmpty(info.NameExt))
                {
                    valuesToFill.Fields.Add(new FieldContent("NameExt", info.NameExt));
                }

                if (!string.IsNullOrEmpty(info.Summary))
                {
                    valuesToFill.Fields.Add(new FieldContent("Summary", info.Summary));
                }

                if (!string.IsNullOrEmpty(info.PhoneNumber))
                {
                    valuesToFill.Fields.Add(new FieldContent("Phone", info.PhoneNumber));
                }

                if (!string.IsNullOrEmpty(info.Website))
                {
                    valuesToFill.Fields.Add(new FieldContent("Website", info.Website));
                }
            }

            string str = await azureFileController.GetFile(FileType.Templates, documentLink);

            using (var outputDocument = new TemplateProcessor(str).SetRemoveContentControls(true).SetNoticeAboutErrors(false))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }

            return new TemplateFile()
            {
                LocalFile = str
            };
        }

        public void Dispose()
        {
            File.Delete(this.LocalFile);
        }
    }
}
