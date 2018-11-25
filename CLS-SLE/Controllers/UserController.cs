using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using CLS_SLE.Models;
using System.Web.Security;
using System.Net.Mail;
using BCrypt.Net;

namespace CLS_SLE.Controllers
{
    public class UserController : Controller
    {
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "Login,Password")] UserSignIn userSignIn, string ReturnUrl)
        {
            using (SLE_DB_ db = new SLE_DB_())
            {
                if (ModelState.IsValid)
                {
                    User user = db.Users.Where(u => u.Login == userSignIn.Login).FirstOrDefault();
                    
                    // hash & salt the posted password
                    string str = BCrypt.Net.BCrypt.HashString(userSignIn.Password, 10);
                    bool bcb = BCrypt.Net.BCrypt.Verify(userSignIn.Password, user.Hash);
                    // Compared posted Hash to customer password
                    if (bcb == true)
                    {
                        // Passwords match
                        // authenticate user (Stores the UserID in an encrypted cookie)
                        // normally, you would require HTTPS
                        FormsAuthentication.SetAuthCookie(user.PersonID.ToString(), false);
                        Session["personID"] = user.PersonID;
                        user.LastLogin = DateTime.Now;
                        db.SaveChanges();

                        // if there is a return url, redirect to the url
                        if (ReturnUrl != null)
                        {
                            return Redirect(ReturnUrl);
                        }
                        // Redirect to Home page
                        return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
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
    }
}