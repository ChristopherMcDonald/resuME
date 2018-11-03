﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Resume.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Resume.Pages
{
    public class SignupModel : PageModel
    {
        private readonly Models.AppContext _context;

        // TODO expand this for many APIs?
        private readonly IOptions<Configuration.SendGrid> _sendGrid;

        public string Email { get; private set; }

        public string Error { get; private set; }

        public SignupModel(IOptions<Configuration.SendGrid> sendGrid, Models.AppContext app) {
            _context = app;
            _sendGrid = sendGrid;
        }

        public ActionResult OnGet()
        {
            this.Email = Request.Query.ContainsKey("email") ? Request.Query["email"].First() : string.Empty;

            if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                return Redirect("/Home");
            }

            return Page();
        }

        /// <summary>
        /// Ons the post.
        /// </summary>
        /// <returns>The post.</returns>
        /// <param name="user">User.</param>
        [HttpPost]
        public async Task<IActionResult> OnPost(UserRequest user)
        {
            if (!ValidateUserRequest(user, out string res)) {
                this.Error = res;
            }

            User toAdd = new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvailableCash = 0,
                Email = user.Email,
                PasswordHash = Hash(user.Password),
                VerifyString = Guid.NewGuid().ToString(),
                Verified = false
            };

            _context.Add(toAdd);
            await _context.SaveChangesAsync();

            Response response = await this.SendEmail(toAdd);
            return RedirectToPage("/user/checkemail");
        }

        private bool ValidateUserRequest(UserRequest user, out string res)
        {
            res = "";
            var validator = new EmailAddressAttribute();

            if (string.IsNullOrEmpty(user.FirstName)) {
                res = "First Name cannot be empty.";
            } 
            else if (string.IsNullOrEmpty(user.LastName))
            {
                res = "Last Name cannot be empty.";
            }
            else if (!validator.IsValid(user.Email))
            {
                res = "Email is not in correct form.";
            }
            else if (!new Regex(@"[0-9]+").IsMatch(user.Password) || !new Regex(@"[A-Z]+").IsMatch(user.Password) || !new Regex(@".{6,}").IsMatch(user.Password)) 
            {
                res = "Password must have 6 characters, have an upper case letter and a number";
            }

            return string.Empty.Equals(res);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <returns>The SendGrid response</returns>
        /// <param name="user">User to send email to</param>
        private async Task<Response> SendEmail(User user) {
            // TODO make better email
            var apiKey = this._sendGrid.Value.Key;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("chris@christophermcdonald.me", "Christopher McDonald");
            var subject = "Welcome to ResuME! Confirm your email here.";
            var to = new EmailAddress(user.Email, user.FirstName + " " + user.LastName);
            var link = string.Format(
                "{0}://{1}/user/verify?email={2}&guid={3}", 
                this.Request.Scheme, 
                this.Request.Host, 
                user.Email, 
                user.VerifyString);
            var plainTextContent = "Please follow this link to verify your account: " + link;
            var htmlContent = string.Format("<strong>Please follow this link to verify your account: <a href=\"{0}\">ResuME</a></strong>", link);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            return await client.SendEmailAsync(msg);
        }

        /// <summary>
        /// Hash the specified password.
        /// </summary>
        /// <returns>The hash</returns>
        /// <param name="password">Password</param>
        private static string Hash(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Request object to make new User
        /// </summary>
        public class UserRequest 
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}