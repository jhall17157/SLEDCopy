using CLS_SLE.Models;
using CLS_SLE.Utility;
using CLS_SLE.Utility.SAML;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace CLS_SLE.Controllers
{
    public abstract class SLEControllerBase : Controller
    {
        private SLEConfigurationInfo _sleConfig;
        private Response _SAMLResponse;
        private AuthUserData _userData;

        public SLEControllerBase()
        {

        }

        #region << Properties >>

        protected AuthUserData UserData
        {
            get
            {
                if (_userData == null)
                {
                    HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                    if (authCookie != null)
                    {
                        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                        var serializer = new JavaScriptSerializer();
                        _userData = (Controllers.AuthUserData)serializer.Deserialize(authTicket.UserData, typeof(Controllers.AuthUserData));
                    }
                }
                return _userData;
            }
        }

        #endregion

        #region << SAML Methods >>

        protected Response SAMLResponse
        {
            get
            {
                if (_SAMLResponse == null)
                {
                    var resp = Request.Form["SAMLResponse"];
                    if (resp != null)
                    {
                        _SAMLResponse = new Response(SLEConfig.SSOConfig.Certificate, resp);
                    }
                    else
                    {
                        _SAMLResponse = null;
                    }
                }

                return _SAMLResponse;
            }
        }

        protected bool HasSAMLResponse
        {
            get
            {
                return (this.SAMLResponse != null);
            }
        }

        protected SLEConfigurationInfo SLEConfig
        {
            get
            {
                if (_sleConfig == null)
                {
                    _sleConfig = new SLEConfigurationInfo();
                }
                return _sleConfig;
            }
        }

        protected ActionResult SignOut()
        {
            SignOutWeb();

            return SignOutSSO();

        }

        protected ActionResult SignIn()
        {
            var request = new AuthRequest(
                SLEConfig.SSOConfig.IssuerApplicationName, //put your app's "unique ID" here
                SLEConfig.SSOConfig.AssertionConsumerServiceURL //assertion Consumer Url - the redirect URL where the provider will send authenticated users
            );

            //generate the provider URL
            string url = request.GetRedirectUrl(SLEConfig.SSOConfig.LoginURL);
            //url = SLEConfig.SSOConfig.LogoutURL;

            //then redirect your user to the above "url" var
            //for example, like this:
            //Response.Redirect(url);


            return Redirect(url);
        }

        #endregion

        #region << Redirect Methods >>

        protected ActionResult GetRedirectToInstructorAssessmentDashboard()
        {
            return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
        }

        protected ActionResult GetRedirectToAdminDashboard()
        {
            return RedirectToAction(actionName: "AdminDashboard", controllerName: "Admin");
        }

        protected ActionResult GetRedirectToErrorDisplay(SLEError customErrorMessage = SLEError.Undefined)
        {
            if (customErrorMessage == SLEError.Undefined)
            {
                return RedirectToAction(actionName: "Error", controllerName: "User");
            }

            return RedirectToAction(actionName: "Error", controllerName: "User", routeValues: new ErrorMessage {SLEError = customErrorMessage });
        }

        #endregion

        #region << Private Methods >>

        private void SignOutWeb()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
        }

        private ActionResult SignOutSSO()
        {
            var request = new AuthRequest(
                SLEConfig.SSOConfig.IssuerApplicationName, //put your app's "unique ID" here
                SLEConfig.SSOConfig.AssertionConsumerServiceURL //assertion Consumer Url - the redirect URL where the provider will send authenticated users
            );

            //generate the provider URL
            string url = request.GetRedirectUrl(SLEConfig.SSOConfig.LogoutURL);
            //url = SLEConfig.SSOConfig.LogoutURL;

            //then redirect your user to the above "url" var
            //for example, like this:
            //Response.Redirect(url);


            return Redirect(url);
        }

        #endregion
    }

    public class AuthUserData
    {
        public int PersonId { get; set; }
        public string[] UserRoles { get; set; }
    }
    public enum SLEError
    {
        Undefined,
        NotConfiguredForSLE
    }

}