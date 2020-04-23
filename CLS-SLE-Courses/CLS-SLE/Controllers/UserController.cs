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
using System.Security.Principal;
using System.Threading;
using Microsoft.AspNet.Identity.EntityFramework;

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
            try
            {
                FormsAuthentication.SignOut();
                Session.Abandon();
                return View();
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "Login,Hash")] UserSignIn userSignIn)
        {
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            var user = db.Users.Where(u => u.Login == userSignIn.Login).FirstOrDefault();
                            if (BCrypt.Net.BCrypt.Verify(userSignIn.Hash, user.Hash))
                            {
                                if (user.IsActive)
                                {

                                    FormsAuthentication.SetAuthCookie(user.PersonID.ToString(), false);
                                    Session["personID"] = user.PersonID;
                                    Session["User"] = user;
                                    Session.Timeout = 180;
                                    user.LastLogin = DateTime.Now;
                                    db.SaveChanges();
                                    AuthorizeUser(db, user.Login, System.Web.HttpContext.Current);

                                    if (!user.MustResetPassword)
                                    {
                                        // Passwords match
                                        // authenticate user (Stores the UserID in an encrypted cookie)
                                        // User does not need to reset their password, send them straight to the dashboad

                                        logger.Info("Successful login for " + user.Login + ", loading dashboard");
                                        if (System.Web.HttpContext.Current.User.IsInRole("Faculty"))
                                        {
                                            return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
                                        }
                                        else if (System.Web.HttpContext.Current.User.IsInRole("Administrator"))
                                        {
                                            return RedirectToAction(actionName: "AdminDashboard", controllerName: "Admin");
                                        }
                                        else
                                        {
                                            return RedirectToAction(actionName: "Error", controllerName: "User");
                                        }
                                    }

                                    else
                                    {
                                        // Passwords match
                                        // authenticate user (Stores the UserID in an encrypted cookie)
                                        // User must reset their password, send them to the reset password form

                                        logger.Info("Successful login for " + user.Login + ", user must reset their password");
                                        return RedirectToAction(actionName: "ChangePassword", controllerName: "User");
                                    }
                                }
                                else
                                {
                                    logger.Error("InactiveUser");
                                    return RedirectToAction(actionName: "Error", controllerName: "User");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("Hash", "Username or password invalid");
                                return RedirectToAction(actionName: "Error", controllerName: "User");
                            }
                        }
                        catch
                        {
                            ModelState.AddModelError("Hash", "Username or password invalid");
                            return RedirectToAction(actionName: "Error", controllerName: "User");
                        }

                    }
                    logger.Error("Login failed");
                    return RedirectToAction(actionName: "Error", controllerName: "User");
                } catch (Exception ex)
                {
                    FormsAuthentication.SignOut();
                    Session.Abandon();
                    return RedirectToAction(actionName: "Error", controllerName: "User");
                }
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
                    try
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
                            var url = Request.Url.AbsoluteUri+"Form";

                            msg.From = new MailAddress(CLS_SLE.Properties.Settings.Default.EmailFrom);
                            msg.Subject = (CLS_SLE.Properties.Settings.Default.EmailSubject);
                            msg.IsBodyHtml = true;
                            msg.Body = CLS_SLE.Properties.Settings.Default.EmailBody
                                .Replace("[emailLink]", url)
                                .Replace("[passwordHash]", user.TemporaryPasswordHash);
                            msg.To.Add(user.Email);
                            client.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Password reset email exception: " + ex.Message);
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("Login", "User not found");
                        return View();
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
                    try
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
                                    logger.Info("Password change successful for " + user.Login);
                                    return RedirectToAction(actionName: "SignIn", controllerName: "User");
                                }
                            }
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("Login", "User not found");
                        return View();
                    }
                    return RedirectToAction(actionName: "SignIn", controllerName: "User");
                }
                else
                {
                    logger.Error("User Password Reset Email Recovery Unsuccessful");
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
                    try
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
                    catch
                    {
                        ModelState.AddModelError("Login", "User not found");
                        return View();
                    }
                    
                }
                return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments"); ;
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();
        }

        protected void AuthorizeUser(SLE_TrackingEntities db, String login, HttpContext Context)
        {

            var user = Context.User;

                String[] RolesArray = (from Role in db.Roles
                                       join UserRole in db.UserRoles
                                       on Role.RoleID equals UserRole.RoleID
                                       join User in db.Users
                                       on UserRole.PersonID equals User.PersonID
                                       where User.Login == login
                                       select Role.Name).ToArray();
            var UserIdentity = user.Identity;
            Context.User = new GenericPrincipal(UserIdentity, RolesArray);

            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                                    1,
                                                    login,  //user id
                                                    DateTime.Now,
                                                    DateTime.Now.AddMinutes(20),  // expiry
                                                    false,  //do not remember
                                                    string.Join(",", RolesArray),
                                                    "/");
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                                               FormsAuthentication.Encrypt(authTicket));
            Response.Cookies.Add(cookie);
        }

    }
}