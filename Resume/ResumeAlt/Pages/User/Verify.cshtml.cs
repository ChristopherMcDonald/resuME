using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Resume.Models;

namespace Resume.Pages
{
    public class VerifyModel : PageModel
    {
        private readonly Models.AppContext _context;

        public VerifyModel(Models.AppContext app)
        {
            _context = app;
        }

        [HttpGet]
        public async Task<IActionResult> OnGet(string email, string guid) 
        {
            if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                return Redirect("/Home");
            }

            Models.User user = _context.Set<Models.User>().Where(entry => entry.Email.Equals(email)).FirstOrDefault();

            if (user != null && user.VerifyString.Equals(new Guid(guid)))
            {
                user.Verified = true;
                _context.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToPage("/user/login");
            }
            else
            {
                // TODO make bad page
                return RedirectToPage("");
            }
        }
    }
}
