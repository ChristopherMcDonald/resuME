using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Resume.Pages
{
    public class LogoutModel : PageModel
    {
        public ActionResult OnGet()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
    }
}
