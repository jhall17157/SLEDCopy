using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.Models;
using CLS_SLE.Security;
using System.Web.Security;
using System.Net.Mail;

namespace CLS_SLE.Controllers
{
    public class UserController : Controller
    {
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "PersonID,Hash")] UserSignIn userSignIn, string ReturnUrl)
        {
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
            {
                if (ModelState.IsValid)
                {
                    // find customer by CustomerId
                    User user = db.Users.Find(userSignIn.PersonID);
                    // hash & salt the posted password
                    string str = UserAccount.HashSHA512(userSignIn.Hash + user.PersonID);
                    // Compared posted Password to customer password
                    if (str == userSignIn.Hash)
                    {
                        // Passwords match
                        // authenticate user (this stores the CustomerID in an encrypted cookie)
                        // normally, you would require HTTPS
                        FormsAuthentication.SetAuthCookie(user.PersonID.ToString(), false);

                        // send a cookie to the client to indicate that this is a customer
                        HttpCookie myCookie = new HttpCookie("role");
                        myCookie.Value = "user";
                        Response.Cookies.Add(myCookie);

                        // if there is a return url, redirect to the url
                        if (ReturnUrl != null)
                        {
                            return Redirect(ReturnUrl);
                        }

                        // Redirect to Home page
                        return RedirectToAction(actionName: "Index", controllerName: "Home");
                    }
                    else
                    {
                        // Passwords do not match
                        ModelState.AddModelError("Password", "Incorrect password");
                    }
                }
                return View();
            }
        }

        public ActionResult PasswordReset()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordReset([Bind(Include = "PersonId, Hash ,Email")] UserPasswordReset pwReset)
        {
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
            {
                if (ModelState.IsValid)
                {
                    // Check if the Email from PasswordReset is equal to the Email in the DB for the CustomerId
                    User user = db.Users.Find(pwReset.Email);
                    if (user.Email == pwReset.Email)
                    {
                        // Send email
                        MailMessage msg = new MailMessage();
                        System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                        try
                        {
                            msg.Subject = "Add Subject";
                            msg.Body = "Add Email Body Part";
                            msg.From = new MailAddress("billdelarosa218@gmail.com");
                            msg.To.Add("mfleming10@my.wctc.edu");
                            msg.IsBodyHtml = true;
                            client.Host = "smtp.gmail.com";
                            System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("billdelarosa218@gmail.com", "D3lar0sa");
                            client.Port = int.Parse("587");
                            client.EnableSsl = true;
                            client.UseDefaultCredentials = false;
                            client.Credentials = basicauthenticationinfo;
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                            client.Send(msg);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        // Redirect

                        return RedirectToAction(actionName: "CheckEmail", controllerName: "Home");
                    }
                }
            }
            return View();
        }
    }
}