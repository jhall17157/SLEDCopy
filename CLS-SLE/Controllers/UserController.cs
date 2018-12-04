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
            FormsAuthentication.SignOut();
            Session.Abandon();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "Login,Password")] UserSignIn userSignIn)
        {
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
            {
                if (ModelState.IsValid)
                {
                    User user = db.Users.Where(u => u.Login == userSignIn.Login).FirstOrDefault();
                    
                    // hash & salt the posted password
                    bool bcb = BCrypt.Net.BCrypt.Verify(userSignIn.Password, user.Hash);
                    // Compared posted Hash to customer password
                    if (bcb)
                    {
                        if (!user.MustResetPassword)
                        {
                            // Passwords match
                            // authenticate user (Stores the UserID in an encrypted cookie)
                            FormsAuthentication.SetAuthCookie(user.PersonID.ToString(), false);
                            Session["personID"] = user.PersonID;
                            Session["User"] = user;
                            user.LastLogin = DateTime.Now;
                            db.SaveChanges();
                            return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
                        }
                        else
                        {
                            // Passwords match
                            // authenticate user (Stores the UserID in an encrypted cookie)
                            FormsAuthentication.SetAuthCookie(user.PersonID.ToString(), false);
                            Session["personID"] = user.PersonID;
                            Session["User"] = user;
                            user.LastLogin = DateTime.Now;
                            db.SaveChanges();
                            return RedirectToAction(actionName: "ChangePassword", controllerName: "User");
                        }
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

        [Authorize]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        [HttpGet]
        public ActionResult PasswordReset()
        {
            return View();
        }

        //// POST: User/PasswordReset
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult PasswordReset([Bind(Include = "Email")] PasswordReset pwReset)
        //{
        //    using (SLE_TrackingEntities db = new SLE_TrackingEntities())
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            User user = db.Users.Where(u => u.Email == pwReset.Email).FirstOrDefault();

        //            if (user.Email == pwReset.Email)
        //            {
        //                string alpha = "ABCDEFGHIJKLMNOPQRSTUWXYZ";
        //                string rndChars = "";
        //                Random rnd = new Random();
        //                for (int i = 1; i <= 6; i++)
        //                {
        //                    rndChars += alpha[rnd.Next(alpha.Length)];
        //                }
        //                // reset key + time
        //                user.TemporaryPasswordIssued = DateTime.Now;
        //                user.TemporaryPasswordHash = rndChars;
        //                db.SaveChanges();
        //                // Send email
        //                MailMessage msg = new MailMessage();
        //                SmtpClient client = new SmtpClient();
        //                try
        //                {
        //                    msg.Subject = "PASSWORD RESET";
        //                    msg.Body = msg.Body = "Click the link below and enter the code to reset your password for SLE Assessment Login. <br> " +
        //                               "https:/sle-dev.wctc.edu/User/PasswordResetForm/" + user.Login + "<br> Your unique code:" +
        //                               "<br><strong>" + user.TemporaryPasswordHash + "</strong>";
        //                    msg.From = new MailAddress("NoReply@wctc.edu");
        //                    msg.To.Add(pwReset.Email);
        //                    msg.IsBodyHtml = true;
        //                    client.Send(msg);
        //                }
        //                catch (Exception ex)
        //                {

        //                }
        //            }
        //            else
        //            {
        //                return RedirectToAction(actionName: "CheckEmail", controllerName: "Home");
        //            }
        //        }
        //    }
        //    return View();
        //}

        [Authorize]
        public ActionResult ChangePassword()
        {
            if (Session["User"] != null)
            {
                return View();
            }
            return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(UserChangePassword cPassword)
        {
            if (ModelState.IsValid)
            {
                using (SLE_TrackingEntities db = new SLE_TrackingEntities())
                {
                    String userLogin = ((User)Session["User"]).Login;
                    User user = db.Users.Where(u => u.Login == userLogin).FirstOrDefault();

                    
                    if (BCrypt.Net.BCrypt.Verify(cPassword.Password, user.Hash))
                    {
                        if (cPassword.NewPassword.Equals(cPassword.ConfirmPassword))
                        {
                            String hash = BCrypt.Net.BCrypt.HashPassword(cPassword.NewPassword);

                            user.Hash = hash;
                            user.MustResetPassword = false;
                            db.SaveChanges();
                            Session["user"] = user;
                        }
                        else
                        {
                            ModelState.AddModelError("ConfirmPassword", "New passwords do not match");
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Current password is Incorrect");
                        return View();
                    }
                    
                }
                return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments"); ;
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordReset([Bind(Include = "Email")] PasswordReset pwReset)
        {
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
            {
                if (ModelState.IsValid)
                {
                    User user = db.Users.Where(u => u.Login == "yoda").FirstOrDefault();
                    //if (user.Email == pwReset.Email)
                    //{
                    string alpha = "ABCDEFGHIJKLMNOPQRSTUWXYZ";
                    string rndChars = "";
                    Random rnd = new Random();
                    for (int i = 1; i <= 6; i++)
                    {
                        rndChars += alpha[rnd.Next(alpha.Length)];
                    }
                    // reset key + time
                    user.TemporaryPasswordIssued = DateTime.Now;
                    user.TemporaryPasswordHash = rndChars;
                    db.SaveChanges();
                    // Send email
                    MailMessage msg = new MailMessage();
                    SmtpClient client = new SmtpClient();
                    try
                    {
                        msg.From = new MailAddress("NoReply@wctc.edu");
                        msg.Subject = "PASSWORD RESET";
                        msg.IsBodyHtml = true;
                        msg.Body = "Click the link below and enter the code to reset your password for SLE Assessment Login. <br> " +
                                   "<a href = localhost:64901/User/PasswordResetForm>Link</a>" + "<br> Your unique code:" +
                                   "<br><strong>" + user.TemporaryPasswordHash + "</strong>";
                        msg.To.Add(pwReset.Email);
                        client.Send(msg);
                    }
                    catch (Exception ex)
                    {

                    }
                    //}
                    //else
                    //{
                    // Redirect

                    return RedirectToAction(actionName: "CheckEmail", controllerName: "Home");
                    //}
                }
            }
            return View();
        }


        // GET: User/PasswordResetForm
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        [HttpGet]
        public ActionResult PasswordResetForm()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return View();
        }

        // POST
        [HttpPost]
        public ActionResult PasswordResetForm([Bind(Include = "Login,Email,PWResetKey,Hash,SecondHash")] PasswordResetEdit pwEdit)
        {
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
                if (ModelState.IsValid)
                {
                    User user = db.Users.Where(u => u.Login == pwEdit.Login).FirstOrDefault();
                    if (user.TemporaryPasswordHash == pwEdit.PWResetKey &&
                        (DateTime.Now - user.TemporaryPasswordIssued) < TimeSpan.Parse("00:15:00.0000000"))
                    {
                        if (user.Login == pwEdit.Login && user.Email == pwEdit.Email && user.TemporaryPasswordHash == pwEdit.PWResetKey && pwEdit.Hash == pwEdit.SecondHash)
                        {
                            if (pwEdit.Hash != pwEdit.Login && pwEdit.Hash != "Password" && pwEdit.Hash != "Test")
                            {
                                user.Hash = BCrypt.Net.BCrypt.HashPassword(pwEdit.Hash);
                                db.SaveChanges();
                                return RedirectToAction(actionName: "GoodPass", controllerName: "User");
                            }
                            else
                            {
                                return RedirectToAction(actionName: "BadPass", controllerName: "User");
                            }
                        }
                        else
                        {
                            return RedirectToAction(actionName: "BadPass", controllerName: "User");
                        }
                    }
                    else if((DateTime.Now - user.TemporaryPasswordIssued) > TimeSpan.Parse("00:15:00.0000000"))
                    {
                        return RedirectToAction(actionName: "BadCode", controllerName: "User");
                    }
                    return RedirectToAction(actionName: "GoodPass", controllerName: "User");
                }
                else
                {
                    return RedirectToAction(actionName: "BadCode", controllerName: "User");
                }
        }

        public ActionResult BadCode()
        {
            return View();
        }
        public ActionResult BadEmail()
        {
            return View();
        }
        public ActionResult GoodPass()
        {
            return View();
        }
        public ActionResult BadPass()
        {
            return View();
        }
    }
}