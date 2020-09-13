using CLS_SLE.Utility.SAML;
using System.Web.Mvc;
using System.Web.Security;

namespace CLS_SLE.Controllers
{
    [RequireHttps]
    public class HomeController : SLEControllerBase
    {

        public ActionResult Index()
        {
            return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
        }

        [AllowAnonymous]
        [HttpGet]
        //GET: Home/SignOut
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            //return RedirectToAction(actionName: "SignIn", controllerName: "User");


            //return Redirect(string.Format("{0}?post_logout_redirect_uri={1}", SLEConfig.SSOConfig.LogoutURL, "https://localhost:44340/Home/SignOut"));

            var request = new AuthRequest(
                SLEConfig.SSOConfig.IssuerApplicationName, //put your app's "unique ID" here
                SLEConfig.SSOConfig.AssertionConsumerServiceURL //assertion Consumer Url - the redirect URL where the provider will send authenticated users
            );


            //generate the provider URL
            //string url = request.GetRedirectUrl(SLEConfig.SSOConfig.LogoutURL, returnToURL: "https://localhost:44340/User/SignIn");
            //string url = request.GetRedirectUrl(SLEConfig.SSOConfig.LogoutURL);.
            string url = SLEConfig.SSOConfig.LogoutURL;

            return Redirect(url);


        }


        [AllowAnonymous]
        [HttpPost]
        //GET: Home/SignOutConsume
        public ActionResult SignOutConsume()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            //return RedirectToAction(actionName: "SignIn", controllerName: "User");


            //return Redirect(string.Format("{0}?post_logout_redirect_uri={1}", SLEConfig.SSOConfig.LogoutURL, "https://localhost:44340/Home/SignOut"));

            //var request = new AuthRequest(
            //    SLEConfig.SSOConfig.IssuerApplicationName, //put your app's "unique ID" here
            //    SLEConfig.SSOConfig.AssertionConsumerServiceURL //assertion Consumer Url - the redirect URL where the provider will send authenticated users
            //);


            ////generate the provider URL
            //string url = request.GetRedirectUrl(SLEConfig.SSOConfig.LogoutURL, returnToURL: "https://localhost:44340/User/SignIn");

            return View();


        }


        //GET: Home/CheckEmail
        public ActionResult CheckEmail()
        {
            return View();
        }
    }
}