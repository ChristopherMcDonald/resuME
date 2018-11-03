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
using Resume.Models;

namespace Resume.Pages
{
    public class LoginModel : PageModel
    {
        private readonly Models.AppContext _context;

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
                User user = _context.Set<User>().Where(entry => entry.Email.Equals(request.Email)).First();

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
                    if (hashBytes[i + 16] != hash[i])
                    {
                        this.Error = "Wrong email and password combination.";
                        return Page();
                    }


                var claims = new List<Claim>
                {
                    // name is accessible via this.HttpContext.User.Identity.Name
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
                Console.WriteLine("Exception thrown during login: " + e.Message);
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
