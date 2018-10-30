using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Security;

namespace CLS_SLE.Security
{
    public class UserAccount
    {
        public static string HashSHA512(string value)
        {
            var sha512 = System.Security.Cryptography.SHA512.Create();
            var inputBytes = Encoding.ASCII.GetBytes(value);
            var hash = sha512.ComputeHash(inputBytes);
            //var sb = new StringBuilder();
            //for (var i = 0; i < hash.Length; i++)
            //{
            //    sb.Append(hash[i].ToString("X2"));
            //}
            return hash.ToString();
        }

        public static Int32 GetUserID()
        {
            HttpCookie authCookie =
            HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            return Convert.ToInt32(ticket.Name);
        }
    }
}