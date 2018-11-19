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
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
            {
                if (ModelState.IsValid)
                {
                    User user = db.User.Where(u => u.Login == userSignIn.Login).FirstOrDefault();
                    
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
                        // send a cookie to the client to indicate user
                        //HttpCookie myCookie = new HttpCookie("role");
                        //myCookie.Value = "user";
                        //Response.Cookies.Add(myCookie);
                        //Sql "SELECT \r\n    [Extent1].[Login] AS [Login], \r\n    [Extent1].[Hash] AS [Hash], \r\n    [Extent1].[FirstName] AS [FirstName], \r\n    [Extent1].[LastName] AS [LastName]\r\n    FROM [dbo].[Person] AS [Extent1]"   string

                        // if there is a return url, redirect to the url
                        if (ReturnUrl != null)
                        {
                            //return Redirect(ReturnUrl);
                            return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");

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