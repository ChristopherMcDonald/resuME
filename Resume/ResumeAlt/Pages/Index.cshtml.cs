using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Resume.Pages
{
    public class IndexModel : PageModel
    {
        public string UserEmail { get; set; }

        public void OnGet() 
        {
            UserEmail = this.HttpContext.User.Identity.Name;
        }
    }
}
