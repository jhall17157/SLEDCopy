using CLS_SLE.Models;
using CLS_SLE.Utility.SAML;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace CLS_SLE.Controllers
{
    [RequireHttps]
    public class UserController : SLEControllerBase
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
                //return View();
            }
            catch (Exception ex)
            {
                // return View();
            }

            var request = new AuthRequest(
                SLEConfig.SSOConfig.IssuerApplicationName, //put your app's "unique ID" here
                SLEConfig.SSOConfig.AssertionConsumerServiceURL //assertion Consumer Url - the redirect URL where the provider will send authenticated users
            );

            string url = request.GetRedirectUrl(SLEConfig.SSOConfig.LoginURL);

            return Redirect(url);
        }

        [AllowAnonymous]
        //[HttpPost]
        public ActionResult Consume()
        {
            // replace with an instance of the users account.


            if (base.HasSAMLResponse && base.SAMLResponse.IsValid())
            {
                using (SLE_TrackingEntities db = new SLE_TrackingEntities())
                {
                    try
                    {
                        try
                        {
                            var ssoNameId = SAMLResponse.GetNameID();
                            var user = db.Users.Where(u => u.Login == ssoNameId).FirstOrDefault();

                            if (user is null)
                            {
                                logger.Error("UserNotFound");
                                ModelState.AddModelError("Hash", "User not setup in SLE");

                                return base.GetRedirectToErrorDisplay(SLEError.NotConfiguredForSLE);
                            }

                            if (!user.IsActive)
                            {
                                logger.Error("InactiveUser");
                                return base.GetRedirectToErrorDisplay();
                            }


                            // Valid login processing

                            FormsAuthentication.SetAuthCookie(user.PersonID.ToString(), false);

                            Session.Timeout = 180;

                            user.LastLogin = DateTime.Now;
                            db.SaveChanges();

                            AuthorizeUser(user);

                            logger.Info("Successful login for " + user.Login + ", loading dashboard");
                            if (System.Web.HttpContext.Current.User.IsInRole("Faculty"))
                            {
                                return base.GetRedirectToInstructorAssessmentDashboard();
                            }
                            else if (System.Web.HttpContext.Current.User.IsInRole("Administrator"))
                            {
                                return base.GetRedirectToAdminDashboard();
                            }
                            else
                            {
                                return base.GetRedirectToErrorDisplay();
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("Hash", "Username or password invalid");
                            return base.GetRedirectToErrorDisplay();
                        }
                    }
                    catch (Exception ex)
                    {
                        FormsAuthentication.SignOut();
                        Session.Abandon();
                        return base.GetRedirectToErrorDisplay();
                    }
                }




            }
            else
            {

                try
                {
                    FormsAuthentication.SignOut();
                    Session.Abandon();
                    //return View();
                }
                catch (Exception ex)
                {
                    // return View();
                }

                var request = new AuthRequest(
                    SLEConfig.SSOConfig.IssuerApplicationName, //put your app's "unique ID" here
                    SLEConfig.SSOConfig.AssertionConsumerServiceURL //assertion Consumer Url - the redirect URL where the provider will send authenticated users
                );

                string url = request.GetRedirectUrl(SLEConfig.SSOConfig.LoginURL);

                return Redirect(url);
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult Error(ErrorMessage errorMessage)
        {
            errorMessage.LogoutURL = SLEConfig.SSOConfig.LogoutURL;
            return View(errorMessage);
        }

        protected void AuthorizeUser(User login)
        {
            var user = System.Web.HttpContext.Current.User;


            var rolePermissions = login.UserRoles.Select(x => x.Role.RolePermissions).ToArray();
            var permissions = new List<string>();
            foreach (var item in rolePermissions)
            {
                foreach (var perm in item)
                {
                    RolePermission rp = perm;
                    permissions.Add(rp.Permission.Name);
                }
            }
            String[] RolesArray = permissions.ToArray();
            var UserIdentity = user.Identity;


            System.Web.HttpContext.Current.User = new GenericPrincipal(UserIdentity, RolesArray);

            var userData = new AuthUserData { PersonId = login.PersonID, UserRoles = RolesArray };

            var serializer = new JavaScriptSerializer();


            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                                    1,
                                                    login.Login,  //user id
                                                    DateTime.Now,
                                                    DateTime.Now.AddMinutes(20),  // expiry
                                                    false,  //do not remember
                                                    serializer.Serialize(userData),
                                                    "/");

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                                               FormsAuthentication.Encrypt(authTicket));
            Response.Cookies.Add(cookie);
        }

    }
}