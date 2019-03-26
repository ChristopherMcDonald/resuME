using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Resume;

namespace Resume.Pages
{
    public class LoginModel : PageModel
    {
        private readonly Models.AppContext _context;

        public string Email { get; set; }

        public string Error { get; set; }

        public LoginModel(Models.AppContext app) {
            _context = app;
        }

        public ActionResult OnGet() 
        {
            if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name)) {
                return Redirect("/Home");
            }

            return Page();
        }

        [HttpPost]
        public async Task<ActionResult> OnPost(LoginRequest request) 
        {
            try {
                this.Email = request.Email;
                Models.User user = _context.Set<Models.User>().Where(entry => entry.Email.Equals(request.Email)).FirstOrDefault();

                if (user == null || request.Password == null || request.Password.Length < 6)
                {
                    this.Error = "Wrong email and password combination, try again.";
                    return Page();
                }

                // Extract the bytes
                byte[] hashBytes = Convert.FromBase64String(user.PasswordHash);

                // Get the salt
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                // Compute the hash on the password the user entered
                var pbkdf2 = new Rfc2898DeriveBytes(request.Password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);

                // Compare the results
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        this.Error = "Wrong email and password combination, try again.";
                        return Page();
                    }
                }

                // check if verified AFTER password to make sure hacker doesn't churn through emails to find users
                if (!user.Verified)
                {
                    this.Error = "Check your email! We sent you one to verify your email.";
                    return Page();
                }

                var claims = new List<Claim>
                {
                    // name is accessible via this.HttpContext.User.Identity.Name, so we store Email in it
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("FullName", user.FirstName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1440), // TODO make smaller
                    IsPersistent = true,
                    IssuedUtc = DateTime.UtcNow
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Good Login
                return Redirect("/Home");
            }
            catch (Exception e) 
            {
                System.Diagnostics.Trace.TraceError(string.Format("Exception thrown during Login. Exception: {0}, Parameters: {1}", e.ToString(), request.Email));
                this.Error = "Something went wrong...";
                return Page();
            }

        }

        public class LoginRequest 
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
