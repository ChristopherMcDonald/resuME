using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Resume.Controllers
{
    public static class DataValidator
    {
        private static EmailAddressAttribute EmailValidator => new EmailAddressAttribute();

        public static bool Validate(DataType type, string input, out string res)
        {
            res = string.Empty;
            switch(type)
            {
                case DataType.Email:
                    return Email(input, out res);
                case DataType.FirstName:
                    return FirstName(input, out res);
                case DataType.LastName:
                    return LastName(input, out res);
                case DataType.Password:
                    return Password(input, out res);
                case DataType.PhoneNumber:
                    return PhoneNumber(input, out res);
                case DataType.Website:
                    return Website(input, out res);
                default:
                    return true;
            }
        }

        public static bool Password(string pass, out string res) 
        {
            if (!new Regex(@"[0-9]+").IsMatch(pass) || !new Regex(@"[A-Z]+").IsMatch(pass) || !new Regex(@".{6,}").IsMatch(pass))
            {
                res = "Password must have 6 characters, an upper case letter and a number.";
                return false;
            }

            res = string.Empty;
            return true;
        }

        public static bool Website(string web, out string res)
        {
            if (string.IsNullOrEmpty(web) || !new Regex(@"(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?").IsMatch(web))
            {
                res = "Website must be entered properly (e.g. google.com, http://www.google.com, ... etc).";
                return false;
            }

            res = string.Empty;
            return true;
        }

        public static bool PhoneNumber(string number, out string res)
        {
            if (string.IsNullOrEmpty(number) || !new Regex(@"\D?(\d{3})\D?\D?(\d{3})\D?(\d{4})").IsMatch(number))
            {
                res = "Phone numbers must contain 10 digits and be delimited properly.";
                return false;
            }

            res = string.Empty;
            return true;
        }

        public static bool FirstName(string name, out string res)
        {
            if (string.IsNullOrEmpty(name))
            {
                res = "First name is empty.";
            }

            res = string.Empty;
            return true;
        }

        public static bool LastName(string name, out string res)
        {
            if (string.IsNullOrEmpty(name))
            {
                res = "Last name is empty.";
            }

            res = string.Empty;
            return true;
        }

        public static bool Email(string email, out string res)
        {
            if (!EmailValidator.IsValid(email))
            {
                res = "Email is an incorrect form.";
                return false;
            }

            res = string.Empty;
            return true;
        }
    }
}
