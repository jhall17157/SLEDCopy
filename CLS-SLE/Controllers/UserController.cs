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
using NLog;

namespace CLS_SLE.Controllers
{
    public class UserController : Controller
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [HttpGet]
        public ActionResult SignIn()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "Login,Hash")] UserSignIn userSignIn)
        {
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
            {
                if (ModelState.IsValid)
                {
                    var user = db.Users.Where(u => u.Login == userSignIn.Login).FirstOrDefault();
                    if (BCrypt.Net.BCrypt.Verify(userSignIn.Hash, user.Hash))
                    {
                        if (!user.MustResetPassword)
                        {
                            // Passwords match
                            // authenticate user (Stores the UserID in an encrypted cookie)
                            FormsAuthentication.SetAuthCookie(user.PersonID.ToString(), false);
                            Session["personID"] = user.PersonID;
                            Session["User"] = user;
                            Session.Timeout = 180;
                            user.LastLogin = DateTime.Now;
                            db.SaveChanges();
                            logger.Info("Successful login for user: " + user.Login + ", loading dashboard");
                            return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
                        }
                        else
                        {
                            // Passwords match
                            // authenticate user (Stores the UserID in an encrypted cookie)
                            FormsAuthentication.SetAuthCookie(user.PersonID.ToString(), false);
                            Session["personID"] = user.PersonID;
                            Session["User"] = user;
                            Session.Timeout = 180;
                            user.LastLogin = DateTime.Now;
                            db.SaveChanges();
                            logger.Info("Successful login for user: " + user.Login + ", user must reset their password");
                            return RedirectToAction(actionName: "ChangePassword", controllerName: "User");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Hash", "Current password is Incorrect");
                        return View();
                    }
                }
                logger.Error("Login failed");
                return View();
            }
        }

        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        [HttpGet]
        public ActionResult PasswordReset()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordReset([Bind(Include = "Login")] PasswordReset pwReset)
        {
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
                if (ModelState.IsValid)
                {
                    User user = db.Users.Where(u => u.Login == pwReset.Login).FirstOrDefault();
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
                        msg.From = new MailAddress("NOREPLY@wctc.edu");
                        msg.Subject = "PASSWORD RESET";
                        msg.IsBodyHtml = true;
                        msg.Body = "Click the link below and enter the code to reset your password for SLE Assessment Login. <br> " +
                                   "<a href = 'https://sle-dev.wctc.edu/User/PasswordResetForm'>Link</a>" + "<br> Your unique code:" +
                                   "<br><strong>" + user.TemporaryPasswordHash + "</strong>";
                        msg.To.Add(user.Email);
                        client.Send(msg);
                    }
                    catch (Exception ex)
                    {

                    }
                    return RedirectToAction(actionName: "CheckEmail", controllerName: "Home");
                }
                else
                {
                    return View();
                }
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

        [HttpPost]
        public ActionResult PasswordResetForm([Bind(Include = "Login,PWResetKey,Hash,SecondHash")] PasswordResetEdit pwEdit)
        {
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
                if (ModelState.IsValid)
                {
                    User user = db.Users.Where(u => u.Login == pwEdit.Login).FirstOrDefault();
                    if (user.TemporaryPasswordHash == pwEdit.PWResetKey &&
                        (DateTime.Now - user.TemporaryPasswordIssued) < TimeSpan.Parse("00:30:00.0000000"))
                    {
                        if (user.Login == pwEdit.Login && user.TemporaryPasswordHash == pwEdit.PWResetKey && pwEdit.Hash == pwEdit.SecondHash)
                        {
                            if (pwEdit.Hash != pwEdit.Login && pwEdit.Hash != "Password" && pwEdit.Hash != "Test")
                            {
                                user.Hash = BCrypt.Net.BCrypt.HashPassword(pwEdit.Hash);
                                db.SaveChanges();
                                return RedirectToAction(actionName: "SignIn", controllerName: "User");
                            }
                        }
                    }
                    return RedirectToAction(actionName: "SignIn", controllerName: "User");
                }
                else
                {
                    return View();
                }
        }

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
                    if (BCrypt.Net.BCrypt.Verify(cPassword.Hash, user.Hash))
                    {
                        user.Hash = BCrypt.Net.BCrypt.HashPassword(cPassword.NewHash);
                        user.MustResetPassword = false;
                        db.SaveChanges();
                        Session["user"] = user;
                        logger.Info("User " + user.Login + " successfully changed their password");
                    }
                    else
                    {
                        ModelState.AddModelError("Hash", "Current password is Incorrect");
                        logger.Error("User " + user.Login + " attempted to change their password but the inputed current password did not match.");
                        return View();
                    }
                }
                return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments"); ;
            }
            return View();
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